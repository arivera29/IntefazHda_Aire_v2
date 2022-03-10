using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaInformacionOPEN
{
    class Consultar
    {
        private Datos conexion { set; get; }

        public Consultar(Datos conexion)
        {
            this.conexion = conexion;
        }

        public void Start()
        {
            DateTime hoy = DateTime.Now;
            LOG("Fecha de consulta: " + hoy);
            if (hoy.Hour >= 5 && hoy.Hour <= 22)
            {
                LOG("Actualizando información de resolución de orden de servicio");
                this.ActualizarEstadoOrdenServicio();
                LOG("Actualizando información tarifas");
                this.ActualizarTarifaOrdenServicio();
                LOG("Actualizando información de consumos");
                this.ActualizarConsumosOrdenServicio();
                LOG("Proceso finalizado");
            }
            else
            {
                LOG("Horario restringido para consulta de WS a OPEN");
            }

        }
        public void ActualizarEstadoOrdenServicio()
        {
            try
            {

                if (conexion != null)
                {
                    List<Int32> lista = new List<Int32>();
                    String sql = "SELECT _number,nic,_clientCloseTs,protocolo,Delegacion "
                        + " FROM Actas with(nolock)"
                        + " WHERE OsResuelta=0 "
                        + " AND EstadoActa=1 "
                        + " AND conAnomalia=1 "
                        + " ORDER BY _clientCloseTs";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LOG("Consultando WS Acta " + reader.GetInt32(0));
                                WSOrdenes ws = new WSOrdenes();
                                ws.Nic = reader.GetString(1);
                                ws.OrdenServicio = reader.GetInt32(0).ToString();
                                DateTime fecha = reader.GetDateTime(2);
                                ws.fecha = fecha.Year.ToString() + fecha.Month.ToString().PadLeft(2, '0') + fecha.Day.ToString().PadLeft(2, '0');
                                ws.CallWebService();

                                if (ws.Resuelta)
                                {
                                    LOG("Acta " + reader.GetInt32(0) + " Resuelta");
                                    lista.Add(reader.GetInt32(0));
                                }
                                else
                                {
                                    LOG("Acta " + reader.GetInt32(0) + " NO Resuelta");
                                    LOG("Consultando OS resuelta con fecha Actual. Acta " + reader.GetInt32(0));
                                    ws.fecha = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                                    ws.CallWebService();
                                    if (ws.Resuelta)
                                    {
                                        LOG("Acta " + reader.GetInt32(0) + " Resuelta");
                                        lista.Add(reader.GetInt32(0));
                                    }
                                    else
                                    {
                                        LOG("Acta " + reader.GetInt32(0) + " NO Resuelta");
                                    }
                                }

                            }
                        }

                    }


                    if (lista.Count > 0)
                    {
                        foreach (Int32 acta in lista)
                        {
                            LOG("Cambiabdo estado orden RESUELTA acta " + acta);
                            sql = "UPDATE Actas SET "
                                + " OsResuelta= 1, fechaOrden=SYSDATETIME() "
                                + " WHERE _number=@acta "
                                + " AND estadoActa=1 "
                                + " AND osResuelta='0'";

                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();
                                cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                                cmd.Prepare();
                                cmd.ExecuteNonQuery();

                            }
                        }
                    }

                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        public void ActualizarTarifaOrdenServicio()
        {
            try
            {
                if (conexion != null)
                {
                    List<OrdenServicio> lista = new List<OrdenServicio>();
                    String sql = "SELECT _number,nic,fechaCarga "
                        + " FROM Actas with(nolock)"
                        + " WHERE ValorTarifa=0 "
                        + " ORDER BY fechaCarga";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LOG("Consultando tarifa acta " + reader.GetInt32(0));

                                WSTarifa ws = new WSTarifa();
                                ws.nic = reader.GetString(1);
                                DateTime fecha = reader.GetDateTime(2);
                                ws.fecha = fecha.Year.ToString() + fecha.Month.ToString().PadLeft(2, '0') + fecha.Day.ToString().PadLeft(2, '0');
                                ws.CallWebService();
                                if (ws.Tarifa != null)
                                {
                                    LOG("Respuesta WS Eca " + ws.Tarifa.ValorTarifa);
                                    if (!ws.Tarifa.ValorTarifa.Equals("0"))
                                    {
                                        OrdenServicio t = new OrdenServicio();
                                        t.acta = reader.GetInt32(0);
                                        t.tarifa = "0";
                                        try
                                        {
                                            double d = double.Parse(ws.Tarifa.ValorTarifa);
                                            t.tarifa = ws.Tarifa.ValorTarifa;

                                        }
                                        catch (Exception e)
                                        {

                                        }


                                        lista.Add(t);
                                    }
                                }
                                else
                                {
                                    LOG("Respuesta WS Eca NO VALIDA");
                                }
                            }
                        }

                    }

                    LOG("Iniciando proceso de Actualizacion de Tarifas");
                    if (lista.Count > 0)
                    {
                        LOG("Total actas a actualizar: " + lista.Count);
                        foreach (OrdenServicio t in lista)
                        {
                            LOG("Actualizando tarifa acta " + t.acta + " a " + t.tarifa);
                            sql = "UPDATE Actas SET ValorTarifa= @tarifa WHERE _number=@acta";
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();
                                cmd.Parameters.Add("@tarifa", SqlDbType.VarChar, 20).Value = t.tarifa;
                                cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = t.acta;
                                cmd.Prepare();
                                cmd.ExecuteNonQuery();
                                LOG("Tarifa actualizada, acta: " + t.acta + " Tarifa: " + t.tarifa);
                            }
                        }
                    }

                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }


        public void LOG(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\CONSULTA_WS_OPEN_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            Console.Write(cadena);
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

            //listBox2.Items.Add(fecha + " " + log);
        }

        public void ActualizarConsumosOrdenServicio()
        {
            try
            {
                if (conexion != null)
                {
                    List<OrdenServicio> lista = new List<OrdenServicio>();
                    String sql = "SELECT _number,nic,fechaCarga,_clientCloseTS "
                        + " FROM Actas with(nolock) "
                        + " WHERE (SELECT COUNT(*) FROM Consumo WHERE ConsActa = Actas._number) = 0 "
                        + " AND estadoActa IN (1,2)"
                        + " ORDER BY fechaCarga";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrdenServicio t = new OrdenServicio();
                                t.acta = reader.GetInt32(0);
                                t.nic = reader.GetString(1);
                                t.fechaLevantamientoActa = reader.GetDateTime(3);
                                lista.Add(t);
                            }
                        }

                    }

                    if (lista.Count > 0)
                    {
                        foreach (OrdenServicio t in lista)
                        {
                            LOG("Consultando consumos acta: " + t.acta.ToString());
                            ConsultarConsumos(t.acta.ToString(), t.nic, t.fechaLevantamientoActa);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        private void ConsultarConsumos(String acta, String nic, DateTime fecha)
        {
            WSConsumo eca = new WSConsumo();
            eca.nic = nic;
            eca.fecha = fecha.Year.ToString() + fecha.Month.ToString().PadLeft(2, '0') + fecha.Day.ToString().PadLeft(2, '0');
            eca.CallWebService();
            LOG("Consumos recibidos de OPEN: " + eca.ListaConsumos.Count);
            if (eca.ListaConsumos.Count > 0)
            {

                for (int x = 0; x < eca.ListaConsumos.Count; x++)
                {
                    int contador = 0;
                    try
                    {
                        String sql = "INSERT INTO Consumo (ConsActa,ConsFech,ConsValo) " +
                            " VALUES (@acta,@fecha, @valor)";

                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();

                            cmd.Parameters.Add("@acta", SqlDbType.VarChar, 50).Value = acta;
                            cmd.Parameters.Add("@fecha", SqlDbType.VarChar, 50).Value = eca.ListaConsumos[x].fecha;
                            cmd.Parameters.Add("@valor", SqlDbType.VarChar, 50).Value = eca.ListaConsumos[x].consumo;

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                // Se guardó el registro
                                contador++;
                            }
                            else
                            {
                                LOG("Error al guardar el registro de consumos: " + acta);
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
                    }
                    LOG("Consumos registrados para la acta " + acta + " " + contador);
                }
            }
            else
            {
                LOG("No se recibe registro de consumos del OPEN. acta " + acta);
                LOG("Respuesta WS: " + eca.Respuesta);
            }
        }

        private class OrdenServicio
        {
            public Int32 acta { set; get; }
            public String nic { set; get; }
            public String tarifa { set; get; }
            public String medidor { set; get; }
            public DateTime fechaLevantamientoActa { set; get; }

        }

    }
}
