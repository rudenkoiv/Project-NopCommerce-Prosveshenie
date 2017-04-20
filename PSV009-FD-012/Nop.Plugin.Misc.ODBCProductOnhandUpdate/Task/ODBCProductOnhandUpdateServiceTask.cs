using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ODBCCore;
using Nop.Plugin.Misc.ODBCCore.Core;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nop.Plugin.Misc.ODBCProductOnhandUpdate
{
    class ODBCProductOnhandUpdateServiceTask : ITask
    {
        private ILogger _logger;
        private ODBCCoreConnector _odbcConnector;
        private ODBCCoreSettings _odbcSettings;
        private IProductService _productService;
        
        public ODBCProductOnhandUpdateServiceTask(ILogger          logger,
                                                 ODBCCoreSettings odbcSettings)
        {
            this._logger = logger;
            this._odbcSettings = odbcSettings;
            this._productService = EngineContext.Current.Resolve<IProductService>();
            this._odbcConnector = new ODBCCoreConnector(_odbcSettings.ODBCName,
                                                        _odbcSettings.Username,
                                                        _odbcSettings.Password);
        }

        public void updateProductStockVal(string msgId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT InventoryStock.ItemId, InventoryStock.AvailQty, InventoryStock.ExpectedReceiptDate from InventoryStock ");
            strSql.AppendFormat("where InventoryStock.MsgId='{0}'", msgId);

            List<Product> listProductsForUpdate = new List<Product>();

            var messageStatus = true;
            var errorCount = 0;
            var totalCount = 0;

            foreach (DataRow row in _odbcConnector.readData(strSql.ToString()).Tables[0].Rows)
            {
                var product = _productService.GetProductBySku(row.ItemArray[0].ToString());

                if (product == null)
                {
                    messageStatus = false;
                    errorCount++;
                }
                else
                {
                   var qty = Convert.ToInt32(row.ItemArray[1]);

                    if (qty != 0)
                    {
                        product.StockQuantity = qty;
                    }
                    else
                    {
                        //Нет такого поля в БД которое указано в ФД. Необходимо разобраться че это за поле
                        //product.ExpectedReceiptDate = Convert.ToDateTime(row.ItemArray[2]);
                    }

                    listProductsForUpdate.Add(product);
                }

                totalCount++;
            }

            if (listProductsForUpdate.Count != 0)
            {
                _productService.UpdateProducts(listProductsForUpdate);
            }

            InsertLog(messageStatus, msgId, totalCount, errorCount);
            UpdateMsgStatus(msgId, messageStatus);


        }

        public void Execute()
        {
            var strSql = new StringBuilder();

            strSql.Append("SELECT MsgTable.MsgId,MsgTable.MsgType From MsgTable ");
            strSql.Append("where (MsgTable.MsgType='InventoryFullEC' OR MsgTable.MsgType='InventoryPartEC')");
            strSql.Append("AND MsgTable.Source='1C' AND MsgTable.Destination='EC' ");
            strSql.Append("AND (MsgTable.Status='Exported' OR MsgTable.Status='ImportedError')");
            strSql.Append("order by MsgTable.MsgId asc");
           
            IEnumerable<DataRow> q = from p in _odbcConnector.readData(strSql.ToString()).Tables[0].AsEnumerable() select p ;
            IEnumerable<DataRow> f = q.Where(p => p.ItemArray[1].ToString() == "InventoryFullEC").OrderBy(p => p.ItemArray[0]);
            int msgId = 0;

            if (f != null)
            {
                msgId = Convert.ToInt32(f.Max(p => p.ItemArray[0]));
                IEnumerable<DataRow> l = q.Where(p => Convert.ToInt32(p.ItemArray[0]) < msgId);
                l.ToList().ForEach(p => UpdateMsgStatus(p.ItemArray[0].ToString(), true));
                clearStockValue();
            }

            IEnumerable<DataRow> r = q.Where(p => Convert.ToInt32(p.ItemArray[0]) >= msgId);
            r.ToList().ForEach(p => updateProductStockVal(p.ItemArray[0].ToString()));
        }

        private void InsertLog(Boolean messageStatus, string msgId, int totalRecords, int errorRecors = 0)
        {
            var logLevel = messageStatus ? Core.Domain.Logging.LogLevel.Information : Core.Domain.Logging.LogLevel.Error ;
            var shortMessage = messageStatus ? "Success product onhand importing" : "Error on product onhand importing";
            var fullMessage = new StringBuilder();
            fullMessage.AppendFormat("Total records for upload {0}.", totalRecords);
            fullMessage.AppendFormat("Upload records {0}.", totalRecords - errorRecors);
            fullMessage.AppendFormat("Records with error {0}.", errorRecors);
            _logger.InsertLog(logLevel, shortMessage, fullMessage.ToString());
        }

        private void UpdateMsgStatus(string msgId, bool messageStatus)
        {
            var strSql = new StringBuilder();
            strSql.Append("UPDATE MsgTable ");
            strSql.AppendFormat("SET Status  = '{0}' ", messageStatus ? "ImportedOk" : "ImportedError");
            strSql.AppendFormat("WHERE MsgTable.MsgId='{0}'", msgId);
            _odbcConnector.writeData(strSql.ToString());

        }

        private void clearStockValue()
        {
            foreach (var product in _productService.SearchProducts())
            {
                product.StockQuantity = 0;
                EngineContext.Current.Resolve<IProductService>().UpdateProduct(product);
            }    
        }
    }
}
