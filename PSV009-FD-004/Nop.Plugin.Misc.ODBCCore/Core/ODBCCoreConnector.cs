using Nop.Data;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;
using System.Linq;
using System;
using System.Data;

namespace Nop.Plugin.Misc.ODBCCore.Core
{
    public class ODBCCoreConnector
    {
        private string _dsn;
        private string _userName;
        private string _password;

        public ODBCCoreConnector(string dsn,
                                 string username,
                                 string password)
        {
            this._dsn = dsn;
            this._userName = username;
            this._password = password;
        }


        public DataSet readData(string queryString)
        {
            OdbcCommand command = new OdbcCommand(queryString);
            DataSet dataSet = new DataSet();
            using (OdbcConnection connection = new OdbcConnection(appendStringConnection()))
            {
                command.Connection = connection;
                connection.Open();

                OdbcDataAdapter adpter = new OdbcDataAdapter(queryString, connection);
                adpter.Fill(dataSet);
            }

            return dataSet;
        }

        private string appendStringConnection()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Dsn={0};" + "Uid={1};" + "Pwd={2};", _dsn, _userName, _password);
            return sb.ToString();
        }

        public void writeData(string queryString)
        {
          
            using (OdbcConnection connection = new OdbcConnection(appendStringConnection()))
            {
                connection.Open();

                using (OdbcCommand command = connection.CreateCommand())
                {
                    command.CommandText = queryString;
                    command.ExecuteNonQuery();
                }

            }
        }
    }
}
