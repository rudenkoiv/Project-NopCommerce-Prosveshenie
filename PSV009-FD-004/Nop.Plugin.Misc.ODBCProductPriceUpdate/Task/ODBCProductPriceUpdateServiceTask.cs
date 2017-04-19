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
using System.Text;

namespace Nop.Plugin.Misc.ODBCProductPriceUpdate
{
    class ODBCProductPriceUpdateServiceTask : ITask
    {
        private ILogger _logger;
        private ODBCCoreConnector _odbcConnector;
        private ODBCCoreSettings _odbcSettings;
        private IProductService _productService;
        
        public ODBCProductPriceUpdateServiceTask(ILogger          logger,
                                                 ODBCCoreSettings odbcSettings)
        {
            this._logger = logger;
            this._odbcSettings = odbcSettings;
            this._productService = EngineContext.Current.Resolve<IProductService>();
            this._odbcConnector = new ODBCCoreConnector(_odbcSettings.ODBCName,
                                                        _odbcSettings.Username,
                                                        _odbcSettings.Password);
        }

        public void Execute()
        {

            var strSql = new StringBuilder();

            strSql.Append("SELECT MsgTable.MsgId AS MsgId From MsgTable ");
            strSql.Append("where MsgTable.MsgType='PriceEC' ");
            strSql.Append("AND MsgTable.Source='1C' AND MsgTable.Destination='EC' ");
            strSql.Append("AND (MsgTable.Status='Exported' OR MsgTable.Status='ImportedError') ");
            strSql.Append("order by MsgTable.MsgId asc");

            var productFullList = _productService.SearchProducts();


            var dataTable = _odbcConnector.readData(strSql.ToString()).Tables[0];

            foreach (DataRow dr in _odbcConnector.readData(strSql.ToString()).Tables[0].Rows)
            {
                var msgId = dr.ItemArray[0].ToString();

                strSql.Clear();
                strSql.Append("SELECT PriceTable.ItemId, PriceTable.Price From PriceTable ");
                strSql.AppendFormat("where PriceTable.MsgId = '{0}'", msgId);

                var messageStatus = true;
                var errorCount = 0;
                var totalCount = 0;

                List<Product> listProductsForUpdate = new List<Product>();

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
                        product.Price = Convert.ToDecimal(row.ItemArray[1]);
                        product.Published = true;
                        listProductsForUpdate.Add(product);
                        productFullList.Remove(product);
                    }

                    totalCount++;

                }

                if (listProductsForUpdate.Count != 0)
                {
                    _productService.UpdateProducts(listProductsForUpdate);
                }

                if (productFullList.Count != 0)
                {
                    UnpublishedProducts(productFullList);
                }

                InsertLog(messageStatus, msgId, totalCount, errorCount);
                UpdateMsgStatus(msgId, messageStatus);

                if (!messageStatus)
                {
                    var error = new StringBuilder();
                    error.AppendFormat("Error on ODBC product price updating from message {0}", msgId);
                    throw new Exception(error.ToString());
                }
               
            }

        }
        private void InsertLog(Boolean messageStatus, string msgId, int totalRecords, int errorRecors = 0)
        {
            var logLevel = messageStatus ? Core.Domain.Logging.LogLevel.Information : Core.Domain.Logging.LogLevel.Error ;
            var shortMessage = messageStatus ? "Success product price importing" : "Error on product price importing";
            var fullMessage = new StringBuilder();
            fullMessage.AppendFormat("Total records for upload {0}.", totalRecords);
            fullMessage.AppendFormat("Upload records {0}.", totalRecords - errorRecors);
            fullMessage.AppendFormat("Records with error {0}.", errorRecors);
            _logger.InsertLog(logLevel, shortMessage, fullMessage.ToString());
        }

        private void UpdateMsgStatus(string msgId, Boolean messageStatus)
        {
            var strSql = new StringBuilder();
            strSql.Append("UPDATE MsgTable ");
            strSql.AppendFormat("SET Status  = '{0}' ", messageStatus ? "ImportedOk" : "ImportedError");
            strSql.AppendFormat("WHERE MsgTable.MsgId='{0}'", msgId);
            _odbcConnector.writeData(strSql.ToString());

        }

        private void UnpublishedProducts(IList<Product> productList)
        {
            foreach (var product in productList)
            {
               product.Published = false;
               EngineContext.Current.Resolve<IProductService>().UpdateProduct(product);
            }
           
        }
    }
}
