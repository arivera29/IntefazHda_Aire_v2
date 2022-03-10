using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace RutaDocumentos
{
    class Datos
    {
        SqlConnection conn = null;
        public String Error = "";
        SqlTransaction transaction;

        public Datos()
        {
            this.Connect();
        }

        private void Connect()
        {
            try
            {
                conn = new SqlConnection();
                conn.ConnectionString = ConfigVars.UrlConexionBdHGI2();
                conn.Open();
            }
            catch (Exception ex) {
                System.Console.WriteLine("Error al conectarse con el servidor. " + ex.Message);
            }
        }

        public System.Data.SqlClient.SqlConnection getConection()
        {
            return conn;
        }

        public bool ExecuteNonQuery(String sql, bool commit)
        {
            SqlCommand myCommand = new SqlCommand(sql, conn);
            if (commit)
            {
                BeginTransaction();
                myCommand.Transaction = this.transaction;
            }
            if (myCommand.ExecuteNonQuery() > 0)
            {
                if (commit)
                {
                    transaction.Commit();
                }
                return true;
            }

            return false;
        }

        public void BeginTransaction()
        {
            transaction = conn.BeginTransaction();
        }
        
        public void Commit()
        {
            transaction.Commit();
        }
        public void Rollback()
        {
            transaction.Rollback();
        }

        public void Close()
        {
            conn.Close();
        }

        public SqlTransaction  getTransaction()
        {
            return this.transaction;
        }
    }
}
