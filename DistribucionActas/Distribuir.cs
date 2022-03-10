using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribucionActas
{
    class Distribuir
    {
        public const string ESTADO_PENDIENTE_ASIGNAR = "1";
        public const string ESTADO_REVISION_LIQUIDACION = "2";
        public const string ESTADO_PENDIENTE_DE_PROCESO = "3";
        public const string ESTADO_COLOCAR_AL_COBRO = "4";
        public const string ESTADO_EN_MENSAJERIA = "11";
        public const string ESTADO_CONFIRMAR_LIQUIDACION_ANTICIPADA = "6";
        public const string ESTADO_SIN_ANOMALIA = "15";
        public const string ESTADO_SELECCIONAR_PROCESO = "16";
        public const string BANDEJA_ADMIN = "0";
        public const int PROCESO_EVIDENTE = 1;
        public const int PROCESO_ANTICIPAR = 2;

        public Datos conexion { set; get; }
        DataTable dt;

        public Distribuir()
        {
            dt = new DataTable();
            dt.Columns.Add("acta", typeof(Int32));
            dt.Columns.Add("delegacion", typeof(String));
            dt.Columns.Add("tarifa", typeof(String));

        }

        public void UpdateDelegacionContrata()
        {
            String sql = "EXEC UpdateDelegacionContrata;";
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Connection = conexion.getConection();

                if (cmd.ExecuteNonQuery() > 0)
                {
                    // Registro guardado
                }

            }

        }

        public void DistribuirActas()
        {
            try
            {
                if (conexion != null)
                {
                    dt.Rows.Clear();
                    GestionBandeja gb = new GestionBandeja();
                    gb.conexion = conexion;

                    String sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock)"
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo in(0,2) "
                        + " AND EstadoActa=1 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND subnormal = 0 "
                        + " AND ActaManual = 0 "
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);
                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["Delegacion"], GestionBandeja.BANDEJA_PROCESO, true))
                            {

                                LOG("Distribuyendo acta " + (Int32)row["acta"] + " a la bandeja " + gb.CodigoBandeja);
                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa=1,"
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior= Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa=1";

                                using (SqlCommand cmd = new SqlCommand(sql))
                                {

                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_REVISION_LIQUIDACION;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de proceso disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE PROCESO DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Proceso' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        LOG("No hay actas para distribuir");
                    }

                    LOG("Distribuyendo Actas Pendientes de protocolo con Anomalia Evidente");

                    dt.Rows.Clear();
                    sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock)"
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo = 1 "
                        + " AND EstadoActa=1 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND subnormal = 0 "
                        + " AND ActaManual = 0 "
                        + " AND (select count(*) From Anomalias, TipoAnomalia where Anomalias.AcAnNuAc = Actas._number and Anomalias.AcAnCodi = TipoAnomalia.TiAnCodi And TipoAnomalia.TiAnAcEv = 1) > 0 "
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);
                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["Delegacion"], GestionBandeja.BANDEJA_PROCESO, true))
                            {
                                LOG("Distribuyendo acta " + (Int32)row["acta"] + " a la bandeja " + gb.CodigoBandeja);
                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa=1,"
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior= Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa=1";

                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_REVISION_LIQUIDACION;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de proceso disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE PROCESO DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Proceso' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }
                                }

                            }

                        }

                    }
                    else
                    {
                        LOG("No hay actas para distribuir");
                    }


                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        public void DistribuirActasSubnormal()
        {
            try
            {
                if (conexion != null)
                {
                    dt.Rows.Clear();
                    GestionBandeja gb = new GestionBandeja();
                    gb.conexion = conexion;

                    String sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock)"
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo in(0,2) "
                        + " AND EstadoActa=1 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND subnormal= 1"
                        + " AND Confirmada = 1"
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);
                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["Delegacion"], GestionBandeja.BANDEJA_PROCESO, true))
                            {
                                LOG("Distribuyendo acta " + (Int32)row["acta"] + " a la bandeja " + gb.CodigoBandeja);
                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa=1,"
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior= Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa=1";

                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_SELECCIONAR_PROCESO;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_SELECCIONAR_PROCESO, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de proceso disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE PROCESO DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Proceso' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        LOG("No hay actas para distribuir");
                    }

                    LOG("Distribuyendo Actas Subnormales Pendientes de protocolo con Anomalia Evidente");

                    dt.Rows.Clear();
                    sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock)"
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo = 1 "
                        + " AND EstadoActa=1 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND subnormal = 1 "
                        + " AND ActaManual = 0 "
                        + " AND Confirmada = 1 "
                        + " AND (select count(*) From Anomalias, TipoAnomalia where Anomalias.AcAnNuAc = Actas._number and Anomalias.AcAnCodi = TipoAnomalia.TiAnCodi And TipoAnomalia.TiAnAcEv = 1) > 0 "
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);
                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["Delegacion"], GestionBandeja.BANDEJA_PROCESO, true))
                            {
                                LOG("Distribuyendo acta " + (Int32)row["acta"] + " a la bandeja " + gb.CodigoBandeja);
                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa=1,"
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior= Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa=1";

                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_SELECCIONAR_PROCESO;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_SELECCIONAR_PROCESO, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de proceso disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE PROCESO DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Proceso' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }
                                }

                            }

                        }

                    }
                    else
                    {
                        LOG("No hay actas para distribuir");
                    }
                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        public void DistribuirActasSinAnomalia()
        {
            try
            {
                if (conexion != null)
                {
                    dt.Rows.Clear();
                    GestionBandeja gb = new GestionBandeja();
                    gb.conexion = conexion;

                    String sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock)"
                        + " WHERE EstadoActa=1 "
                        + " AND conAnomalia = 0 "
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);
                            }
                        }
                    }
                    LOG("Actas pendientes de distribuir: " + dt.Rows.Count);
                    int contador = 0;

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["Delegacion"], GestionBandeja.BANDEJA_SIN_ANOMLIA, true))
                            {
                                LOG("Distribuyendo acta " + (Int32)row["acta"] + " a la bandeja " + gb.CodigoBandeja);
                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa=1,"
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior= Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa=1";

                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_SIN_ANOMALIA;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            contador++;
                                            AgregarBitacora((int)row["acta"], ESTADO_SIN_ANOMALIA, "1");
                                        }
                                    }
                                    catch (SqlException ex)
                                    {
                                        LOG("Error: " + ex.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de proceso disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE PROCESO DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Sin Anomalia' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException ex)
                                    {
                                        LOG("Error: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        LOG("No hay actas para distribuir");
                    }
                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        public void DistribuirActasManuales()
        {
            try
            {
                if (conexion != null)
                {
                    dt.Rows.Clear();
                    GestionBandeja gb = new GestionBandeja();
                    gb.conexion = conexion;

                    String sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock)"
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo in(0,2) "
                        + " AND EstadoActa=1 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND ActaManual = 1 "
                        + " AND Confirmada = 1"
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);
                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["Delegacion"], GestionBandeja.BANDEJA_PROCESO, true))
                            {
                                LOG("Distribuyendo acta " + (Int32)row["acta"] + " a la bandeja " + gb.CodigoBandeja);
                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa=1,"
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior= Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa=1";

                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_SELECCIONAR_PROCESO;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();

                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_SELECCIONAR_PROCESO, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de proceso disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE PROCESO DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Proceso' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        LOG("No hay actas para distribuir");
                    }

                    LOG("Distribuyendo Actas Manuales Pendientes de protocolo con Anomalia Evidente");

                    dt.Rows.Clear();
                    sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock)"
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo = 1 "
                        + " AND EstadoActa=1 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND subnormal = 0 "
                        + " AND ActaManual = 1 "
                        + " AND Confirmada = 1 "
                        + " AND (select count(*) From Anomalias, TipoAnomalia where Anomalias.AcAnNuAc = Actas._number and Anomalias.AcAnCodi = TipoAnomalia.TiAnCodi And TipoAnomalia.TiAnAcEv = 1) > 0 "
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);
                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["Delegacion"], GestionBandeja.BANDEJA_PROCESO, true))
                            {
                                LOG("Distribuyendo acta " + (Int32)row["acta"] + " a la bandeja " + gb.CodigoBandeja);
                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa=1,"
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior= Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa=1";

                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_SELECCIONAR_PROCESO;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_SELECCIONAR_PROCESO, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de proceso disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE PROCESO DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Proceso' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }
                                }

                            }

                        }

                    }
                    else
                    {
                        LOG("No hay actas para distribuir");
                    }
                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        public void DistribuirLiquidacionAnticipadaActas()
        {
            try
            {
                LOG("6.Anticipando Energia");
                AnticipardaActas();
                if (conexion != null)
                {

                    dt.Rows.Clear();
                    GestionBandeja gb = new GestionBandeja();
                    gb.conexion = conexion;

                    String sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock) "
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo=1 "
                        + " AND EstadoActa=1 "
                        + " AND EstadoAnteriorActa != 6 "
                        + " AND FRAnticipado IS NULL "
                        + " AND EnergiaAnticipada > 0 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND subnormal = 0 "
                        + " AND ActaManual = 0 "
                        + " AND (select count(*) From Anomalias, TipoAnomalia where Anomalias.AcAnNuAc = Actas._number and Anomalias.AcAnCodi = TipoAnomalia.TiAnCodi And TipoAnomalia.TiAnAcEv = 1) = 0"
                        + " AND (select count(*) From Anomalias, TipoAnomalia where Anomalias.AcAnNuAc = Actas._number and Anomalias.AcAnCodi = TipoAnomalia.TiAnCodi And TipoAnomalia.TiAnAnAc = 1) > 0"
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);


                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["delegacion"], GestionBandeja.BANDEJA_LIQUIDACION_ANTICIPADA, false))
                            {
                                LOG("Asignando acta " + (Int32)row["acta"] + " a bandeja de LIQUIDACION ANTICIPADA " + gb.CodigoBandeja);

                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa='1' , "
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior=Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND conAnomalia = 1 "
                                    + " AND estadoActa='1'";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 110).Value = ESTADO_CONFIRMAR_LIQUIDACION_ANTICIPADA;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_CONFIRMAR_LIQUIDACION_ANTICIPADA, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de Liquidación anticipada disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE LIQ. ANTICIPADA DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Anticipada' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        LOG("No hay actas para disptribuir.");
                    }

                    sql = "SELECT _number,nic,fechaCarga,protocolo,Delegacion "
                        + " FROM Actas with(nolock) "
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo=1 "
                        + " AND EstadoActa=1 "
                        + " AND EstadoAnteriorActa != 6 "
                        + " AND FRAnticipado IS NULL "
                        + " AND EnergiaAnticipada > 0 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND subnormal = 0 "
                        + " AND ActaManual = 1 "
                        + " AND Confirmada = 1 "
                        + " AND (select count(*) From Anomalias, TipoAnomalia where Anomalias.AcAnNuAc = Actas._number and Anomalias.AcAnCodi = TipoAnomalia.TiAnCodi And TipoAnomalia.TiAnAcEv = 1) = 0"
                        + " AND (select count(*) From Anomalias, TipoAnomalia where Anomalias.AcAnNuAc = Actas._number and Anomalias.AcAnCodi = TipoAnomalia.TiAnCodi And TipoAnomalia.TiAnAnAc = 1) > 0"
                        + " ORDER BY fechaCarga";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(4);
                                dt.Rows.Add(row);


                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (gb.BuscarBandejaDisponible((string)row["delegacion"], GestionBandeja.BANDEJA_LIQUIDACION_ANTICIPADA, false))
                            {
                                LOG("Asignando acta " + (Int32)row["acta"] + " a bandeja de LIQUIDACION ANTICIPADA " + gb.CodigoBandeja);

                                sql = "UPDATE Actas SET "
                                    + " EstadoAnteriorActa='1' , "
                                    + " estadoActa=@estado, "
                                    + " BandejaAnterior=Bandeja, "
                                    + " Bandeja=@bandeja, "
                                    + " FechaAsignacionBandeja = SYSDATETIME() "
                                    + " WHERE _number=@acta "
                                    + " AND conAnomalia = 1 "
                                    + " AND estadoActa='1'";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 110).Value = ESTADO_CONFIRMAR_LIQUIDACION_ANTICIPADA;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_CONFIRMAR_LIQUIDACION_ANTICIPADA, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado bandeja de Liquidación anticipada disponible con la delegacion " + (string)row["Delegacion"] + " para el acta " + (Int32)row["acta"]);
                                this.AgregarAnotacion((Int32)row["acta"], "NO SE ENCUENTRA BANDEJA DE LIQ. ANTICIPADA DISPONIBLE");

                                sql = "UPDATE Actas SET FechaAsignacionBandeja= SYSDATETIME(), Incidencia='Sin Capacidad Band. Anticipada' WHERE _number = @acta AND FechaAsignacionBandeja IS NOT NULL";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
                                        }
                                    }
                                    catch (SqlException e)
                                    {
                                        LOG("Error. " + e.Message);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        LOG("No hay actas para disptribuir.");
                    }
                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }
        public void AgregarBitacora(int acta, string estadoActual, string estadoAnterior)
        {
            try
            {
                String sql = "INSERT INTO Bitacora (BitaUsua,BitaActa,BitaFeca,BitaEsAc,BitaEsCa,BitaEsta,BitaEsMe,BitaEsAn) "
                        + "VALUES ('interfaz',@acta,GETDATE(),@estadoActual,0,1,0,@estadoAnterior)";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();

                    cmd.Parameters.Add("@acta", SqlDbType.VarChar, 20).Value = acta;
                    cmd.Parameters.Add("@estadoActual", SqlDbType.VarChar, 10).Value = estadoActual;
                    cmd.Parameters.Add("@estadoAnterior", SqlDbType.VarChar, 10).Value = estadoAnterior;

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        // Se guardó el registro
                    }
                    else
                    {
                        LOG("Error al guardar el registro de la Bitacora");
                    }

                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        private void AgregarAnotacion(Int32 _number, String anotacion)
        {
            try
            {
                String sql = "INSERT INTO AnotacionActa (AnotActa,AnotDesc,AnotEsta,AnotUsua,AnotFeSi) "
                            + "VALUES (@_number,@anotacion,@estado,@usuario,SYSDATETIME())";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();

                    cmd.Parameters.Add("@_number", SqlDbType.Int, 11).Value = _number;
                    cmd.Parameters.Add("@anotacion", SqlDbType.VarChar, 200).Value = anotacion;
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = "1";
                    cmd.Parameters.Add("@usuario", SqlDbType.VarChar, 10).Value = "interfaz";

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        // Se guardó el registro
                    }
                    else
                    {
                        LOG("Error al guardar el registro de la anotacion: " + _number);
                    }

                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }
        }

        public void AnticipardaActas()
        {
            try
            {
                if (conexion != null)
                {
                    dt.Rows.Clear();
                    GestionBandeja gb = new GestionBandeja();
                    gb.conexion = conexion;

                    String sql = "SELECT _number,Delegacion,codigoTarifa "
                        + " FROM Actas with(nolock) "
                        + " WHERE OsResuelta=1 "
                        + " AND protocolo IN(1,2) "
                        + " AND EstadoActa=1 "
                        + " AND EnergiaAnticipada <= 0 "
                        + " AND ValorTarifa > 0 "
                        + " AND conAnomalia = 1 "
                        + " AND subnormal = 0"
                        + " ORDER BY fechaCarga";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["acta"] = reader.GetInt32(0);
                                row["delegacion"] = reader.GetString(1);
                                row["tarifa"] = reader.GetString(2);
                                dt.Rows.Add(row);


                            }
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            double liquidacion = LiquidacionAnticipada((string)row["delegacion"], (string)row["tarifa"], conexion);
                            if (liquidacion > 0)
                            {
                                LOG("Obteniendo liquidacion anticipada " + liquidacion + " Acta: " + row["acta"]);
                                sql = "UPDATE Actas SET "
                                    + " EnergiaAnticipada=@liquidacion "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa='1'";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    SqlParameter parameter = new SqlParameter("@liquidacion", SqlDbType.Decimal);
                                    parameter.Value = liquidacion;
                                    parameter.Precision = 12;
                                    parameter.Scale = 2;

                                    cmd.Parameters.Add(parameter);
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();

                                    int record = cmd.ExecuteNonQuery();
                                    if (record > 0)
                                    {
                                        LOG("Actualizando liquidacion anticipada Acta: " + row["acta"]);
                                    }

                                }
                            }
                            else
                            {
                                LOG("No se ha encontrado valor de Liquidación anticipada en la delegacion " + (string)row["Delegacion"] + " y tarifa " + (string)row["tarifa"] + " para el acta " + (Int32)row["acta"]);
                            }
                        }
                    }
                    else
                    {
                        LOG("No hay actas para disptribuir.");
                    }
                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        public void ActualizarActasBrigadaElite()
        {
            LOG("Iniciado proceso de actualización de brigada Elite en HGi2");
            String sql = "Update Actas set usuarioCarga = (select a.bigEliteUser from BrigadaElite a Where a.bigEliteCod = Actas.cedulaOperario),"
                        + " subnormal = 1, bigElite=1, IdTipoActa=4 "
                        + " WHERE Actas.cedulaOperario in (select BrigadaElite.bigEliteCod from BrigadaElite)"
                        + " and convert(date,fechaCarga) >= convert(date,'2018-09-11')"
                        + " and subnormal = 0 and ActaManual = 0 and estadoActa=1;";
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Connection = conexion.getConection();

                if (cmd.ExecuteNonQuery() >= 0)
                {
                    LOG("Actualización de brigadas Elite ejecutada correctamente.");
                }
                

            }

        }

        public double LiquidacionAnticipada(string delegeacion, string tarifa, Datos conexion)
        {
            double valor = -1;
            if (conexion != null)
            {
                String sql = "SELECT DeTaValo FROM DelegacionTarifa with(nolock) WHERE DeTaDele = @delegacion AND DeTaTari = @tarifa";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@delegacion", SqlDbType.VarChar, 100).Value = delegeacion;
                    cmd.Parameters.Add("@tarifa", SqlDbType.VarChar, 100).Value = tarifa;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            valor = (double)reader.GetDecimal(0);
                        }
                    }

                }

            }

            return valor;

        }

        public void EnviarSeleccionProceso()
        {
            try
            {
                Datos conexion = new Datos();
                if (conexion != null)
                {
                    List<Int32> actas = new List<Int32>();

                    LOG("Consultando Actas para transferencia a Seleccion de Proceso por tiempo");
                    String sql = "select A._number from Actas A "
                        + " inner join Mensajeria M on a._number = m.MensActa "
                        + " where A.EstadoActa = 11 "
                        + " AND ((m.EstadoEntrega='E') OR (m.EstadoEntrega='D' AND m.MensCaDe = 4)) "
                        + " AND a.TipoProcesoCobroId = 2 "
                        + " AND DATEDIFF(DAY, m.FechaEntregaExpe, SYSDATETIME()) > 15";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                actas.Add(reader.GetInt32(0));
                                
                            }
                        }

                        LOG("Total actas a transferir: " + actas.Count);
                        foreach (Int32 acta in actas)
                        {
                            LOG("Acta " + acta + " Aplica transferencia por tiempo en proceso PARE");
                            LOG("Transfiriendo acta proceso PARE para estado seleccion de proceso por tiempo.  ACTA: " + acta);
                            sql = "UPDATE Actas SET  estadoActa=16, DevueltaProceso=DevueltaProceso+1 WHERE _number = @acta";
                            using (SqlCommand cmd2 = new SqlCommand(sql))
                            {
                                cmd2.Connection = conexion.getConection();

                                cmd2.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;

                                if (cmd2.ExecuteNonQuery() > 0)
                                {
                                    LOG("Estado del acta actualizada para selección de proceso. PARE. acta: " + acta);
                                }
                                else
                                {
                                    LOG("Error al actualizar estado del acta a seleccion de proceso. PARE. acta: " + acta);
                                }
                            }
                        }

                    }

                    conexion.Close();
                }

            }
            catch (Exception ex)
            {
                LOG(ex.Message);
            }
        }

        public void LOG(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\DISTRIBUCION_HGI2_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            System.Console.Write(cadena);
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

        }
    }
}
