using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribucionActas
{
    class GestionBandeja
    {
        public Datos conexion {set; get;}
        public string CodigoBandeja { set; get; }
        
        public const string BANDEJA_PROCESO = "1";
        public const string BANDEJA_SUPERVISOR = "2";
        public const string BANDEJA_LIQUIDACION_ANTICIPADA = "3";
        public const string BANDEJA_RECHAZO = "4";
        public const string BANDEJA_SIN_ANOMLIA = "5";


        /*  Funcion BuscarBandejaDisponible
         * Parametros:
         * delegacion: Codigo de la delegacion
         * tipo: Tipo de bandeja
         * 
         * Retorna:
         * true si encontro bandeja disponible
         * false si no encontro bandeja disponible
         * 
         */
        public bool BuscarBandejaDisponible(String delegacion, String tipo, bool capacidad)
        {
            bool resultado = false;
            try
            {
                if (conexion != null)
                {


                    String sql = "SELECT TOP 1 A.BandCodi, (SELECT count(_number) total "
                            + " FROM Actas "
                            + " WHERE A.BandCodi = Actas.Bandeja "
                            + " AND ISNULL(Actas.estNovedad,'') IN ('','C') "
                            + " AND Actas.EstadoActa IN('1','2','3','4','6','5','15')) as Total"
                            + " FROM Bandejas A, BandejaZona "
                            + " WHERE A.BandTiBa = @tipo "
                            + " AND A.BandCodi = BandejaZona.BazoBand "
                            + " AND BandejaZona.BazoZona = @delegacion "
                            + " AND A.BandEsta = 1 ";

                    if (capacidad)  // Validar la capacidad de la bandeja
                    {
                        sql += " AND A.BandTope > (SELECT count(_number) total "
                        + " FROM Actas C"
                        + " WHERE A.BandCodi = C.Bandeja"
                        + " AND ISNULL(C.estNovedad,'') IN ('','C') "
                        + " AND C.EstadoActa IN('1','2','3','4','6','5'))";

                    }

                    sql += " ORDER BY Total ASC ";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Parameters.Add("@delegacion", SqlDbType.VarChar, 100).Value = delegacion;
                        cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 100).Value = tipo;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.CodigoBandeja = Convert.ToString(reader.GetInt32(0));
                                resultado = true;
                            }
                        }

                    }

                }
            }
            catch (SqlException ex)
            {
                System.Console.WriteLine("Error: " + ex.Message);
            }
            return resultado;
        }

    }
}
