using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace WPF_DCrypt
{
    public class DB_SQLServer
    {
        
        public SqlConnection? DbConnect()
        {
            string connectionString = BuildConnectionString();
            if (connectionString == null)
            {
                return null;
            }

            var dbConn = new SqlConnection();
            dbConn.ConnectionString = connectionString;
            dbConn.Open();

            return (dbConn);
        }

        public string BuildConnectionString()
        {
            System.Text.StringBuilder sConnection =
                    new System.Text.StringBuilder();

            sConnection.Append("Data Source=SERVER_NAME");
            sConnection.Append(";Database=DATABASE_NAME");
            sConnection.Append(";User Id=USER_NAME");
            sConnection.Append(";Password=PASSWORD");
            sConnection.Append(";TrustServerCertificate=True");


            return sConnection.ToString();
        }
    }
}
