using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ExSql
{
    class Datos
    {
        SqlConnection conn = null;
        public String Error = "";
        SqlTransaction transaction = null;

        public Datos()
        {
            this.Connect();
        }

        private void Connect()
        {
            try
            {
                conn = new SqlConnection();
                // TODO: Modify the connection string and include any
                // additional required properties for your database.
                //conn.ConnectionString = "integrated security=SSPI;data source=PC-AIMER\\SQLEXPRESS;persist security info=False;initial catalog=BDHGI2";
                //conn.ConnectionString = @Properties.Settings.Default.conexion;
                conn.ConnectionString = ConfigVars.UrlConexionBdHGI2();
                //conn.ConnectionString = @"Data Source=.;Initial Catalog=BDHGI2;Integrated Security=True;Pooling=False";
                conn.Open();
                // Insert code to process data.
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
            using (SqlCommand myCommand = new SqlCommand(sql, conn)) {
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
