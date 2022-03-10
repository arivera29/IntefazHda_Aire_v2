﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InterfazHda
{
    class GestionActa
    {
        public Datos conexion { set; get; }
        public const string ESTADO_PENDIENTE_ASIGNAR = "1";
        public const string ESTADO_REVISION_LIQUIDACION = "2";
        public const string ESTADO_PENDIENTE_DE_PROCESO = "3";
        public const string ESTADO_COLOCAR_AL_COBRO = "4";
        public const string ESTADO_EN_MENSAJERIA = "11";
        public const string ESTADO_CONFIRMAR_LIQUIDACION_ANTICIPADA = "6";
        public const string ESTADO_SIN_ANOMALIA = "15";
        public const string BANDEJA_ADMIN = "0";
        public const int PROCESO_EVIDENTE = 1;
        public const int PROCESO_ANTICIPAR = 2;

        DataTable dt;

        public GestionActa()
        {
            dt = new DataTable();
            dt.Columns.Add("acta", typeof(Int32));
            dt.Columns.Add("delegacion", typeof(String));
            dt.Columns.Add("tarifa", typeof(String));
        }

        public bool AgregarActa(Acta acta)
        {


            String sql = String.Format("INSERT INTO Actas " +
                   "(id,_number,numeroLote,codigoEmpresa,tipoOrdenServicio," +
                   "tipoServicio,comentario1,comentario2,direccion,codigoTarifa," +
                   "nic,tipoCliente,estrato,cargaContratada,departamento,municipio," +
                   "localidad,tipoVia,calle,numeroPuerta,duplicador,piso,referenciaDireccion," +
                   "acceso,numeroCircuito,matriculaCT,nombreTitularContrato,apellido1TitularContrato," +
                   "apellido2TitularContrato,cedulaTitularContrato,telefonoFijoTitularContrato,telefonoMovilTitularContrato," +
                   "emailTitularContrato,relacionReceptorVisita,solicitaTecnicoReceptorVisita,aportaTestigo," +
                   "nombreReceptorVisita,apellido1ReceptorVisita,apellido2ReceptorVisita,cedulaReceptorVisita," +
                   "telefonoFijoReceptorVisita,telefonoMovilReceptorVisita,emailReceptorVisita,modoPagoTipo," +
                   "fechaInicioIrregularidad,residuosRecolectados,clasificacionResiduos,ordenAseo," +
                   "recibidoQuejas,atendidoQuejas,observaciones,EstadoActa,Asegurada,nombreTecnico," +
                   "apellido1Tecnico,apellido2Tecnico,cedulaTecnico,comteTecnico,nombreTestigo,apellido1Testigo," +
                   "apellido2Testigo,cedulaTestigo,protocolo,codContrata,usuarioCarga,fechaCarga,ObsAnomalia," +
                   "tipoCenso,censoCargaInstalada,Delegacion,Bandeja,ValorTarifa,OsResuelta,EnergiaAnticipada," +
                   "medidaAnomaliaTipo,medidaAnomaliaVR,medidaAnomaliaVS,medidaAnomaliaVT,medidaAnomaliaIR," +
                   "medidaAnomaliaIS,medidaAnomaliaIT,_clientCloseTs, nombreOperario,apellido1Operario," +
                   "apellido2Operario,cedulaOperario,empresaOperario,fechaOrden,conAnomalia,subnormal,FechaAsignacionBandeja) " +
                   "VALUES (@id,@_number,@numeroLote,@codigoEmpresa,@tipoOrdenServicio," +
                   "@tipoServicio,@comentario1,@comentario2,@direccion,@codigoTarifa," +
                   "@nic,@tipoCliente,@estrato,@cargaContratada,@departamento,@municipio," +
                   "@localidad,@tipoVia,@calle,@numeroPuerta,@duplicador,@piso,@referenciaDireccion," +
                   "@acceso,@numeroCircuito,@matriculaCT,@nombreTitularContrato,@apellido1TitularContrato," +
                   "@apellido2TitularContrato,@cedulaTitularContrato,@telefonoFijoTitularContrato,@telefonoMovilTitularContrato," +
                   "@emailTitularContrato,@relacionReceptorVisita,@solicitaTecnicoReceptorVisita,@aportaTestigo," +
                   "@nombreReceptorVisita,@apellido1ReceptorVisita,@apellido2ReceptorVisita,@cedulaReceptorVisita," +
                   "@telefonoFijoReceptorVisita,@telefonoMovilReceptorVisita,@emailReceptorVisita,@modoPagoTipo," +
                   "@fechaInicioIrregularidad,@residuosRecolectados,@clasificacionResiduos,@ordenAseo," +
                   "@recibidoQuejas,@atendidoQuejas,@observaciones,@EstadoActa,@Asegurada,@nombreTecnico," +
                   "@apellido1Tecnico,@apellido2Tecnico,@cedulaTecnico,@comteTecnico,@nombreTestigo,@apellido1Testigo," +
                   "@apellido2Testigo,@cedulaTestigo,@protocolo,@CodContrata,'interfaz',SYSDATETIME(),@ObsAnomalia," +
                   "@tipoCenso,@censoCargaInstalada,@delegacion,@bandeja,@ValorTarifa,@OsResuelta,@EnergiaAnticipada," +
                   "@medidaAnomaliaTipo,@medidaAnomaliaVR,@medidaAnomaliaVS,@medidaAnomaliaVT,@medidaAnomaliaIR," +
                   "@medidaAnomaliaIS,@medidaAnomaliaIT,@_clientCloseTs,@nombreOperario,@apellido1Operario," +
                   "@apellido2Operario,@cedulaOperario,@empresaOperario,@fechaOrden,@conAnomalia,@subnormal,@FechaAsignacionBandeja)");

            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Connection = conexion.getConection();
                cmd.Parameters.Add("@conAnomalia", SqlDbType.Int, 11).Value = acta.actaSinAnomalia == true ? 0 : 1;
                cmd.Parameters.Add("@subnormal", SqlDbType.Int, 11).Value = 0;
                cmd.Parameters.Add("@_number", SqlDbType.VarChar, 20).Value = acta._number;
                cmd.Parameters.Add("@numeroLote", SqlDbType.VarChar, 50).Value = acta.numeroLote;
                cmd.Parameters.Add("@codigoEmpresa", SqlDbType.VarChar, 50).Value = acta.codigoEmpresa;
                cmd.Parameters.Add("@tipoOrdenServicio", SqlDbType.VarChar, 100).Value = acta.tipoOrdenServicio;
                cmd.Parameters.Add("@tipoServicio", SqlDbType.VarChar, 100).Value = acta.tipoServicio;
                cmd.Parameters.Add("@comentario1", SqlDbType.VarChar, 250).Value = acta.comentario1;
                cmd.Parameters.Add("@comentario2", SqlDbType.VarChar, 250).Value = acta.comentario2;
                cmd.Parameters.Add("@direccion", SqlDbType.VarChar, 150).Value = acta.direccion;
                cmd.Parameters.Add("@codigoTarifa", SqlDbType.VarChar, 5).Value = acta.tarifa;
                cmd.Parameters.Add("@nic", SqlDbType.VarChar, 20).Value = acta.nic;
                cmd.Parameters.Add("@tipoCliente", SqlDbType.VarChar, 50).Value = acta.tipoCliente;
                cmd.Parameters.Add("@estrato", SqlDbType.VarChar, 50).Value = acta.estrato;

                cmd.Parameters.Add("@cargaContratada", SqlDbType.VarChar, 20).Value = acta.cargaContratada;
                cmd.Parameters.Add("@ValorTarifa", SqlDbType.VarChar, 20).Value = 0;

                WSTarifa ws = new WSTarifa();
                ws.nic = acta.nic;

                DateTime fechaCierre = DateTime.Now;
                if (!string.IsNullOrEmpty(acta._clientCloseTs) )
                {
                   fechaCierre =  DateTime.Parse(acta._clientCloseTs.Replace(" COT", ""));
                }
                else
                {
                    LOGActasFechaNull(acta._number);
                }

                ws.fecha = fechaCierre.Year.ToString() + fechaCierre.Month.ToString().PadLeft(2, '0') + fechaCierre.Day.ToString().PadLeft(2, '0');
                try
                {
                    ws.CallWebService();
                    if (ws.Tarifa != null)
                    {
                        cmd.Parameters["@cargaContratada"].Value = Double.Parse(ws.Tarifa.CargaContratada.Replace(".", ",")) * 1000;
                        cmd.Parameters["@ValorTarifa"].Value = ws.Tarifa.ValorTarifa;
                    }
                }
                catch (Exception e)
                {
                    LOG(e.Message);
                }

                cmd.Parameters.Add("@departamento", SqlDbType.VarChar, 50).Value = acta.departamento;
                cmd.Parameters.Add("@municipio", SqlDbType.VarChar, 50).Value = acta.municipio;
                cmd.Parameters.Add("@localidad", SqlDbType.VarChar, 50).Value = acta.localidad;
                cmd.Parameters.Add("@tipoVia", SqlDbType.VarChar, 50).Value = acta.tipoVia;
                cmd.Parameters.Add("@calle", SqlDbType.VarChar, 50).Value = acta.calle;
                cmd.Parameters.Add("@numeroPuerta", SqlDbType.VarChar, 50).Value = acta.numeroPuerta;
                cmd.Parameters.Add("@duplicador", SqlDbType.VarChar, 50).Value = acta.duplicador;
                cmd.Parameters.Add("@piso", SqlDbType.VarChar, 50).Value = acta.piso;
                cmd.Parameters.Add("@referenciaDireccion", SqlDbType.VarChar, 150).Value = acta.referenciaDireccion;
                cmd.Parameters.Add("@acceso", SqlDbType.VarChar, 50).Value = acta.acceso;
                cmd.Parameters.Add("@numeroCircuito", SqlDbType.VarChar, 50).Value = acta.numeroCircuito;
                cmd.Parameters.Add("@matriculaCT", SqlDbType.VarChar, 50).Value = acta.matriculaCT;
                cmd.Parameters.Add("@nombreTitularContrato", SqlDbType.VarChar, 150).Value = acta.nombreTitularContrato;
                cmd.Parameters.Add("@apellido1TitularContrato", SqlDbType.VarChar, 150).Value = acta.apellido1TitularContrato;
                cmd.Parameters.Add("@apellido2TitularContrato", SqlDbType.VarChar, 150).Value = acta.apellido2TitularContrato;
                cmd.Parameters.Add("@cedulaTitularContrato", SqlDbType.VarChar, 20).Value = acta.cedulaTitularContrato;
                cmd.Parameters.Add("@telefonoFijoTitularContrato", SqlDbType.VarChar, 20).Value = acta.telefonoFijoTitularContrato;
                cmd.Parameters.Add("@telefonoMovilTitularContrato", SqlDbType.VarChar, 20).Value = acta.telefonoMovilTitularContrato;
                cmd.Parameters.Add("@emailTitularContrato", SqlDbType.VarChar, 150).Value = acta.emailTitularContrato;
                cmd.Parameters.Add("@relacionReceptorVisita", SqlDbType.VarChar, 50).Value = acta.relacionReceptorVisita;
                cmd.Parameters.Add("@solicitaTecnicoReceptorVisita", SqlDbType.VarChar, 6).Value = acta.solicitaTecnicoReceptorVisita;
                cmd.Parameters.Add("@aportaTestigo", SqlDbType.VarChar, 6).Value = acta.aportaTestigo;
                cmd.Parameters.Add("@nombreReceptorVisita", SqlDbType.VarChar, 150).Value = acta.nombreReceptorVisita;
                cmd.Parameters.Add("@apellido1ReceptorVisita", SqlDbType.VarChar, 150).Value = acta.apellido1ReceptorVisita;
                cmd.Parameters.Add("@apellido2ReceptorVisita", SqlDbType.VarChar, 150).Value = acta.apellido2ReceptorVisita;
                cmd.Parameters.Add("@cedulaReceptorVisita", SqlDbType.VarChar, 20).Value = acta.cedulaReceptorVisita;
                cmd.Parameters.Add("@telefonoFijoReceptorVisita", SqlDbType.VarChar, 20).Value = acta.telefonoFijoReceptorVisita;
                cmd.Parameters.Add("@telefonoMovilReceptorVisita", SqlDbType.VarChar, 20).Value = acta.telefonoMovilReceptorVisita;
                cmd.Parameters.Add("@emailReceptorVisita", SqlDbType.VarChar, 150).Value = acta.emailReceptorVisita;
                cmd.Parameters.Add("@modoPagoTipo", SqlDbType.VarChar, 50).Value = "";
                cmd.Parameters.Add("@fechaInicioIrregularidad", SqlDbType.DateTime, 20).Value = fechaCierre;
                cmd.Parameters.Add("@residuosRecolectados", SqlDbType.VarChar, 20).Value = acta.residuosRecolectados;
                cmd.Parameters.Add("@clasificacionResiduos", SqlDbType.VarChar, 20).Value = acta.clasificacionResiduos;
                cmd.Parameters.Add("@ordenAseo", SqlDbType.VarChar, 20).Value = acta.ordenAseo;
                cmd.Parameters.Add("@recibidoQuejas", SqlDbType.VarChar, 20).Value = acta.recibidoQuejas;
                cmd.Parameters.Add("@atendidoQuejas", SqlDbType.VarChar, 20).Value = acta.atendidoQuejas;
                cmd.Parameters.Add("@observaciones", SqlDbType.VarChar, 300).Value = acta.observaciones;

                cmd.Parameters.Add("@Asegurada", SqlDbType.VarChar, 20).Value = "0";
                cmd.Parameters.Add("@nombreTecnico", SqlDbType.VarChar, 150).Value = acta.nombreTecnico;
                cmd.Parameters.Add("@apellido1Tecnico", SqlDbType.VarChar, 150).Value = acta.apellido1Tecnico;
                cmd.Parameters.Add("@apellido2Tecnico", SqlDbType.VarChar, 150).Value = acta.apellido2Tecnico;
                cmd.Parameters.Add("@cedulaTecnico", SqlDbType.VarChar, 20).Value = acta.cedulaTecnico;
                cmd.Parameters.Add("@comteTecnico", SqlDbType.VarChar, 150).Value = acta.comteTecnico;
                cmd.Parameters.Add("@nombreTestigo", SqlDbType.VarChar, 150).Value = acta.nombreTestigo;
                cmd.Parameters.Add("@apellido1Testigo", SqlDbType.VarChar, 150).Value = acta.apellido1Testigo;
                cmd.Parameters.Add("@apellido2Testigo", SqlDbType.VarChar, 150).Value = acta.apellido2Tecnico;
                cmd.Parameters.Add("@cedulaTestigo", SqlDbType.VarChar, 20).Value = acta.cedulaTestigo;

                cmd.Parameters.Add("@_clientCloseTs", SqlDbType.DateTime, 20).Value = fechaCierre;

                cmd.Parameters.Add("@nombreOperario", SqlDbType.VarChar, 20).Value = acta.nombreOperario;
                cmd.Parameters.Add("@apellido1Operario", SqlDbType.VarChar, 20).Value = acta.apellido1Operario;
                cmd.Parameters.Add("@apellido2Operario", SqlDbType.VarChar, 20).Value = acta.apellido2Operario;
                cmd.Parameters.Add("@cedulaOperario", SqlDbType.VarChar, 20).Value = acta.cedulaOperario;
                cmd.Parameters.Add("@empresaOperario", SqlDbType.VarChar, 20).Value = acta.empresaOperario;


                cmd.Parameters.Add("@protocolo", SqlDbType.Int, 11).Value = acta.protocolo;
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 100).Value = acta.Id;
                if (!acta.codigoEmpresa.Equals(""))
                {
                    acta.CodigoContrata = acta.codigoEmpresa.Substring(1, 6);
                    acta.CodigoContrata = acta.CodigoContrata.TrimStart('0');
                    cmd.Parameters.Add("@codContrata", SqlDbType.VarChar, 100).Value = acta.CodigoContrata;
                }
                else
                {
                    acta.CodigoContrata = "-1";
                    cmd.Parameters.Add("@codContrata", SqlDbType.VarChar, 100).Value = "-1";  // Sin contrata
                }

                cmd.Parameters.Add("@ObsAnomalia", SqlDbType.Text, 300).Value = acta.observacionAnomalia;
                cmd.Parameters.Add("@tipoCenso", SqlDbType.VarChar, 50).Value = acta.tipoCenso;
                cmd.Parameters.Add("@censoCargaInstalada", SqlDbType.Decimal, 12).Value = acta.censoCargaInstalada;

                // Consultando el codigo de la contrata
                string delegacion = this.ConsultaDelegacion(acta.CodigoContrata);

                cmd.Parameters.Add("@delegacion", SqlDbType.VarChar, 20).Value = delegacion;

                String estadoActa = "1"; // Pendiente de asignar
                /* 
                 *   Estados del Acta
                 *   1 - Pendiente de Asignar
                 *   2 - Revisar liquidacion
                 *   3 - Pendiente de proceso
                 *   4 - Colocar al cobro
                 *   5 - Rechazada
                 *   6 - Confirmar Liquidacion Anticipada
                 *   10 - Cerrada
                 *   
                 */




                // Consultando Bandeja

                bool sinDisponibilidadBandeja = false;
                string bandeja = BANDEJA_ADMIN;

                GestionBandeja gb = new GestionBandeja();
                gb.conexion = conexion;

                string OSResuelta = this.OrdeServicioResuelta(acta._number, acta.nic);

                cmd.Parameters.Add("@OsResuelta", SqlDbType.VarChar, 20).Value = OSResuelta;

                double liquidacion = 0;

                if (!acta.actaSinAnomalia) // Acta con Anomalia
                {
                    if (OSResuelta == "1") // SI LA ORDEN SE ENCUENTRA RESUELTA
                    {

                        if (acta.protocolo == 0) // Medidor NO fue enviado a laboratorio
                        {
                            if (gb.BuscarBandejaDisponible(delegacion, GestionBandeja.BANDEJA_PROCESO, true))
                            {
                                bandeja = gb.CodigoBandeja;
                                estadoActa = ESTADO_REVISION_LIQUIDACION;  // Revision Liquidacion
                            }
                            else
                            {
                                bandeja = BANDEJA_ADMIN;
                                estadoActa = ESTADO_PENDIENTE_ASIGNAR;
                                sinDisponibilidadBandeja = true;
                            }
                            //else if (gb.BuscarBandejaDisponible(delegacion, GestionBandeja.BANDEJA_SUPERVISOR, false))
                            //{
                            //    bandeja = gb.CodigoBandeja;
                            //    estadoActa = ESTADO_PENDIENTE_ASIGNAR;
                            //}
                        }
                        else // Medidor fue enviado a laboratorio
                        {

                            liquidacion = LiquidacionAnticipada(delegacion, acta.tarifa, conexion);
                            if (liquidacion == 0)
                            {
                                bandeja = BANDEJA_ADMIN;
                                estadoActa = ESTADO_PENDIENTE_ASIGNAR;

                                //if (gb.BuscarBandejaDisponible(delegacion, GestionBandeja.BANDEJA_SUPERVISOR, false))
                                //{
                                //    bandeja = gb.CodigoBandeja;
                                //    estadoActa = ESTADO_PENDIENTE_ASIGNAR;
                                //}

                            }
                            /*
                        else
                        {
                            // Se liquida anticipadamente
                            // Se envia a una bandeja de liquidación anticipada.

                            if (gb.BuscarBandejaDisponible(delegacion, GestionBandeja.BANDEJA_LIQUIDACION_ANTICIPADA, true))
                            {
                                bandeja = gb.CodigoBandeja;
                                estadoActa = ESTADO_CONFIRMAR_LIQUIDACION_ANTICIPADA;
                            }
                            else
                            {
                                if (gb.BuscarBandejaDisponible(delegacion, GestionBandeja.BANDEJA_SUPERVISOR, false))
                                {
                                    bandeja = gb.CodigoBandeja;
                                    estadoActa = ESTADO_PENDIENTE_ASIGNAR;
                                }
                            }
                        }
                             */

                        }


                    }
                    else
                    {

                        bandeja = BANDEJA_ADMIN;
                        estadoActa = ESTADO_PENDIENTE_ASIGNAR;

                        //if (gb.BuscarBandejaDisponible(delegacion, GestionBandeja.BANDEJA_SUPERVISOR, false))
                        //{
                        //    bandeja = gb.CodigoBandeja;
                        //    estadoActa = ESTADO_PENDIENTE_ASIGNAR;
                        //}
                    }

                }
                else  // Sin Anomalia
                {
                    // RECHAZAR ACTA SIN ANOMALIA
                    

                    if (gb.BuscarBandejaDisponible(delegacion, GestionBandeja.BANDEJA_SIN_ANOMLIA, false))
                    {
                        bandeja = gb.CodigoBandeja;
                        estadoActa = ESTADO_SIN_ANOMALIA;
                    }
                    else
                    {
                        bandeja = BANDEJA_ADMIN;
                        estadoActa = ESTADO_PENDIENTE_ASIGNAR;
                    }
                }

                cmd.Parameters.Add("@EnergiaAnticipada", SqlDbType.Float, 12).Value = liquidacion;


                if (bandeja != BANDEJA_ADMIN)
                {
                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 20).Value = bandeja;
                    cmd.Parameters.Add("@FechaAsignacionBandeja", SqlDbType.DateTime, 20).Value = DateTime.Now;
                }
                else
                {
                    //cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 20).Value = DBNull.Value;
                    cmd.Parameters.Add("@bandeja", SqlDbType.Int, 11).Value = BANDEJA_ADMIN;
                    if (sinDisponibilidadBandeja)  // No se encontró bandeja de proceso para asignar el acta
                    {
                        cmd.Parameters.Add("@FechaAsignacionBandeja", SqlDbType.DateTime, 20).Value = DateTime.Now;
                    }
                    else
                    {
                        cmd.Parameters.Add("@FechaAsignacionBandeja", SqlDbType.DateTime, 20).Value = DBNull.Value;
                    }
                }


                cmd.Parameters.Add("@EstadoActa", SqlDbType.VarChar, 20).Value = estadoActa;



                cmd.Parameters.Add("@medidaAnomaliaIR", SqlDbType.Decimal, 20).Value = acta.medidaAnomaliaIR == "" ? 0 : ConvertDecimal(acta.medidaAnomaliaIR);
                cmd.Parameters.Add("@medidaAnomaliaIS", SqlDbType.Decimal, 20).Value = acta.medidaAnomaliaIS == "" ? 0 : ConvertDecimal(acta.medidaAnomaliaIS);
                cmd.Parameters.Add("@medidaAnomaliaIT", SqlDbType.Decimal, 20).Value = acta.medidaAnomaliaIT == "" ? 0 : ConvertDecimal(acta.medidaAnomaliaIT);
                cmd.Parameters.Add("@medidaAnomaliaTipo", SqlDbType.VarChar, 20).Value = acta.medidaAnomaliaTipo;
                cmd.Parameters.Add("@medidaAnomaliaVR", SqlDbType.Decimal, 12).Value = acta.medidaAnomaliaVR == "" ? 0 : ConvertDecimal(acta.medidaAnomaliaVR);
                cmd.Parameters.Add("@medidaAnomaliaVS", SqlDbType.Decimal, 20).Value = acta.medidaAnomaliaVS == "" ? 0 : ConvertDecimal(acta.medidaAnomaliaVS);
                cmd.Parameters.Add("@medidaAnomaliaVT", SqlDbType.Decimal, 20).Value = acta.medidaAnomaliaVT == "" ? 0 : ConvertDecimal(acta.medidaAnomaliaVT);
                if (OSResuelta == "1")
                {
                    cmd.Parameters.Add("@fechaOrden", SqlDbType.DateTime, 20).Value = DateTime.Now;
                }
                else
                {
                    cmd.Parameters.Add("@fechaOrden", SqlDbType.DateTime, 20).Value = System.Data.SqlTypes.SqlDateTime.Null;
                }

                try
                {
                    // Imprimiendo parametros
                    LOG("Parametros");
                    for (int i = 0; i < cmd.Parameters.Count; i++)
                    {
                        LOG("Parametro: " + cmd.Parameters[i].ParameterName + " Valor: " + cmd.Parameters[i].Value);
                    }

                    int recordsAffected = cmd.ExecuteNonQuery();
                    if (recordsAffected > 0)
                    {

                        this.AgregarBitacora(int.Parse(acta._number), estadoActa, "13");

                        
                        // Consumiendo WS ECA de ConsultaUltimosConsumos
                        LOG("Consumiendo Web Services de ConsultaUltimosConsumos");

                        DateTime fechaLevantamientoActa = fechaCierre;
                        ConsultarConsumos(acta._number, acta.nic, fechaLevantamientoActa);

                        // Registrar anomalias
                        if (acta.actaSinAnomalia == false)
                        {
                            RegistrarAnomalias(acta._number, acta.anomalias);
                        }
                        else
                        {

                        }

                        if (acta.censo.Count > 0)
                        {
                            RegistrarCensoCarga(acta._number, acta.censo);
                        }

                        if (acta.fotos.Count > 0)
                        {
                            RegistrarFotos(acta.fotos, acta._number);
                        }

                        if (acta.medidorExistente.Count > 0)
                        {
                            RegistrarMedidorExistente(acta._number, acta.medidorExistente);
                        }

                        if (acta.acciones.Count > 0)
                        {
                            RegistrarAcciones(acta._number, acta.acciones);
                        }

                        //LOG("Recuperando el archivo PDF de la informacion del acta");
                        //RecuperarArchivoActa(acta.Id, acta._number);

                        if (sinDisponibilidadBandeja)
                        {
                            this.AgregarAnotacion(Int32.Parse(acta._number), "NO SE ENCUENTRA BANDEJA DISPONIBLE");
                        }

                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
                }


            }



            return false;
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

        public double ConvertDecimal(string valor)
        {
            double d = 0;
            try
            {
                d = double.Parse(valor);
            }
            catch (Exception e)
            {

            }

            return d;
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

        private string ConsultaDelegacion(String codigoContrata)
        {
            string delegacion = "-1";
            try
            {
                if (conexion != null)
                {
                    String sql = "SELECT ZoUnZona FROM ZonaUnicom, CodContrataUnicom WHERE ZoUnUnic = Unicom AND CodContrata = @contrata";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Parameters.Add("@contrata", SqlDbType.VarChar, 100).Value = codigoContrata;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                delegacion = reader.GetString(0);
                            }
                        }

                    }

                }

            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

            return delegacion;
        }

        public string OrdeServicioResuelta(string _number, string nic)
        {
            string resultado = "0";
            try
            {
                WSOrdenes wsOrden = new WSOrdenes();

                wsOrden.Nic = nic;
                wsOrden.OrdenServicio = _number;
                wsOrden.fecha = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                LOG("Consultando WS Estado Ordenes...");
                wsOrden.CallWebService();
                if (wsOrden.Resuelta)
                {
                    LOG("Respuesta del WS: RESUELTA ");
                    resultado = "1";
                }
                else
                {
                    LOG("Respuesta del WS: PENDIENTE ");
                }
            }
            catch (Exception e)
            {

            }
            return resultado;
        }

        public void LOGActasFechaNull(string acta)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\ACTAS_FECHA_NULL.txt";
            String cadena = fecha + " " + acta + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

            //listBox2.Items.Add(fecha + " " + log);
        }

        public void LOG(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\INTERFAZ_HDA_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

            //listBox2.Items.Add(fecha + " " + log);
        }

        private bool AnticiparLiquidacion(List<Anomalia> anomalias)
        {
            if (conexion != null)
            {
                foreach (Anomalia anomalia in anomalias)
                {
                    try
                    {
                        String sql = "SELECT TiIreCodi FROM TiposIrregularidades,Anomalias WHERE TiIreCodi = @codigo";
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 100).Value = anomalia.Id;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    return true;
                                }
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
                    }
                }

            }

            return false;
        }

        public String ConsultarTarifa(string nic)
        {
            LOG("Consumiendo Web Services de ConsultaTarifaCUContratada");
            String ValorTarifa = "0";
            try
            {
                WSTarifa ws = new WSTarifa();
                ws.nic = nic;
                ws.fecha = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                ws.CallWebService();
                if (ws.Tarifa != null)
                {
                    ValorTarifa = ws.Tarifa.ValorTarifa;
                    LOG("Valor tarifa Respuesta WS " + ValorTarifa);
                }
            }
            catch (Exception ex)
            {
                LOG(ex.Message + " Line Number: " + " Trace: " + ex.StackTrace);
            }

            return ValorTarifa;
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

        private void RecuperarArchivoActa(String id, String _number)
        {
            if (!File.Exists(@Properties.Settings.Default.dir_imagenes + "\\" + id + ".pdf"))
            {
                String url = @Properties.Settings.Default.url_hda_docu + id + "?format=pdf";
                LOG("Descargado Acta, url: " + url);
                try
                {
                    WebClient webClient = new WebClient();
                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Properties.Settings.Default.user_hda + ":" + Properties.Settings.Default.pass_hda));
                    webClient.Headers.Add("Authorization", "Basic " + credentials);
                    webClient.DownloadFile(url, @"C:\inetpub\wwwroot\HGI\File\Documentos\" + id + ".pdf");

                    String sql = "INSERT INTO Documentos (DocuActa,DocuTiDo,DocuUrRe,DocuUsCa,DocuFeCa,DocuUrLo,DocuSincro,DocuVeri,DocuUsVe,DocuFeVe) "
                        + "VALUES (@orden,'15',@url_remoto,'interfaz',SYSDATETIME(),@url_local,0,0,'',NULL)";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        cmd.Parameters.Add("@orden", SqlDbType.VarChar, 20).Value = _number;
                        cmd.Parameters.Add("@url_remoto", SqlDbType.VarChar, 200).Value = url;
                        cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = "File/Documentos/" + id + ".pdf";

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            // Se guardó el registro
                        }
                        else
                        {
                            LOG("Error al guardar el registro del archivo en el gestor documental");
                        }

                    }
                }
                catch (Exception ex)
                {
                    LOG("Problem: " + ex.Message);
                }
            }
            else
            {
                LOG("Error: Archivo de acta  " + id + " ya existe");
            }
        }

        private bool RecuperarArchivoFoto(String id, string url)
        {
            if (!File.Exists(@Properties.Settings.Default.dir_imagenes + "\\" + id + ".jpg"))
            {
                LOG("Descargado Foto, url: " + url);
                try
                {
                    String filename = @Properties.Settings.Default.dir_imagenes + id + ".jpg";
                    WebClient webClient = new WebClient();
                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Properties.Settings.Default.user_hda + ":" + Properties.Settings.Default.pass_hda));
                    webClient.Headers.Add("Authorization", "Basic " + credentials);
                    webClient.DownloadFile(url, filename);

                    if (File.Exists(filename))
                    {
                        FileInfo f = new FileInfo(filename);
                        if (f.Length > 0)
                        {
                            return true;
                        }
                        else
                        {
                            LOG("Archivo de foto url " + url + " size=0");
                        }
                    }
                    else
                    {
                        LOG("Archivo de foto url " + url + " no creado");
                    }

                    

                }
                catch (Exception ex)
                {
                    LOG("Problem: " + ex.Message);
                }
            }
            else
            {
                LOG("Error: Archivo de acta  " + id + " ya existe");
                return true;
            }

            return false;
        }

        private bool RecuperarArchivoFirma(string acta, string campo, string url)
        {
            String file = @Properties.Settings.Default.dir_imagenes + "\\" + acta + "_" + campo + ".jpg";
            if (!File.Exists(file))
            {
                LOG("Descargado archivo Firma, url: " + url);
                try
                {
                    WebClient webClient = new WebClient();
                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Properties.Settings.Default.user_hda + ":" + Properties.Settings.Default.pass_hda));
                    webClient.Headers.Add("Authorization", "Basic " + credentials);
                    webClient.DownloadFile(url, file);
                    if (File.Exists(file))
                    {
                        FileInfo f = new FileInfo(file);
                        if (f.Length > 0)
                        {
                            return true;
                        }
                        else
                        {
                            LOG("Archivo de foto url  " + url + " size=0");
                        }
                    }
                    else
                    {
                        LOG("Archivo de foto url  " + url + " no creado");
                    }


                }
                catch (Exception ex)
                {
                    LOG("Problem: " + ex.Message);
                }
            }
            else
            {
                LOG("Error: Archivo de firma  " + acta + " ya existe");
                return true;
            }

            return false;
        }

        private void RegistrarAnomalias(String acta, List<Anomalia> anomalias)
        {
            foreach (Anomalia anomalia in anomalias)
            {
                try
                {
                    String sql = "INSERT INTO Anomalias (AcAnCodi,AcAnNuAc,AcAnDesc) " +
                        " VALUES (@codigo,@acta, @descripcion)";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 50).Value = anomalia.Id;
                        cmd.Parameters.Add("@acta", SqlDbType.VarChar, 50).Value = acta;
                        cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 200).Value = anomalia.Descripcion;

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            // Se guardó el registro
                        }
                        else
                        {
                            LOG("Error al guardar el registro de las anoamlias acta: " + acta);
                        }

                    }
                }
                catch (SqlException ex)
                {
                    LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
                }


            }
        }

        private void RegistrarCensoCarga(String acta, List<Censo> censo)
        {
            foreach (Censo oCenso in censo)
            {
                try
                {
                    String sql = "INSERT INTO CensoActas (AcCeNuAc,AcCeItem,AcCeNoIt,AcCeEsta) " +
                        " VALUES (@acta,@descripcion, @valor,1)";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 50).Value = oCenso.descripcion;
                        cmd.Parameters.Add("@acta", SqlDbType.VarChar, 50).Value = acta;
                        cmd.Parameters.Add("@valor", SqlDbType.Int, 11).Value = oCenso.cantidad;

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            // Se guardó el registro
                        }
                        else
                        {
                            LOG("Error al guardar el registro de las anoamlias acta: " + acta);
                        }

                    }

                }
                catch (SqlException ex)
                {
                    LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
                }
            }
        }

        private void RegistrarAcciones(String acta, List<Accion> acciones)
        {
            foreach (Accion accion in acciones)
            {
                try
                {
                    String sql = "INSERT INTO TrabajosEjecutados (Acta,CodigoPaso,CodigoAccion,DescripcionAccion,NuevoPaso,Cobro) VALUES " +
                                "(@Acta,@CodigoPaso,@CodigoAccion,@DescripcionAccion,@NuevoPaso,@Cobro)";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        cmd.Parameters.Add("@Acta", SqlDbType.VarChar, 50).Value = acta;
                        cmd.Parameters.Add("@CodigoPaso", SqlDbType.VarChar, 50).Value = accion.CodigoPaso;
                        cmd.Parameters.Add("@CodigoAccion", SqlDbType.VarChar, 50).Value = accion.CodigoAccion;
                        cmd.Parameters.Add("@DescripcionAccion", SqlDbType.VarChar, 50).Value = accion.DescripcionAccion;
                        cmd.Parameters.Add("@NuevoPaso", SqlDbType.VarChar, 50).Value = accion.NuevoPaso;
                        cmd.Parameters.Add("@Cobro", SqlDbType.VarChar, 50).Value = accion.Cobro;

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            // Se guardó el registro
                            // Registrar Materiales

                            if (accion.Materiales.Count > 0)
                            {

                                String Query = "INSERT INTO Materiales (Acta,CodigoAccion,Descripcion,Cantidad,Cobro,CodigoMaterial) VALUES " +
                                        "(@Acta,@CodigoAccion,@Descripcion,@Cantidad,@Cobro,@CodigoMaterial)";

                                foreach (Material material in accion.Materiales)
                                {

                                    using (SqlCommand cmd2 = new SqlCommand(Query))
                                    {
                                        cmd2.Connection = conexion.getConection();
                                        cmd2.Parameters.Add("@Acta", SqlDbType.VarChar, 50).Value = acta;
                                        cmd2.Parameters.Add("@CodigoAccion", SqlDbType.VarChar, 50).Value = accion.CodigoAccion;
                                        cmd2.Parameters.Add("@Descripcion", SqlDbType.VarChar, 50).Value = material.DescripcionMaterial;
                                        cmd2.Parameters.Add("@Cantidad", SqlDbType.VarChar, 50).Value = material.Cantidad;
                                        cmd2.Parameters.Add("@Cobro", SqlDbType.VarChar, 50).Value = material.Cobro;
                                        cmd2.Parameters.Add("@CodigoMaterial", SqlDbType.VarChar, 50).Value = material.CodigoMaterial;

                                        if (cmd2.ExecuteNonQuery() > 0)
                                        {
                                            // Se guardo el material.
                                        }
                                    }

                                }

                            }

                        }
                        else
                        {
                            LOG("Error al guardar el registro de las anoamlias acta: " + acta);
                        }

                    }

                }
                catch (SqlException ex)
                {
                    LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
                }
            }
        }


        private void RegistrarFotos(List<Foto> fotos, String _number)
        {
            try
            {
                String sql = "INSERT INTO Documentos (DocuActa,DocuTiDo,DocuUrRe,DocuUsCa,DocuFeCa,DocuUrLo,DocuSincro,DocuVeri,DocuUsVe,DocuFeVe) "
                    + "VALUES (@orden,@tipo,@url_remoto,'interfaz',SYSDATETIME(),@url_local,1,0,'',NULL)";

                foreach (Foto foto in fotos)
                {
                    if (foto.Firma == 0)
                    {
                        if (this.RecuperarArchivoFoto(foto.Id, foto.Url))
                        {
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();

                                cmd.Parameters.Add("@orden", SqlDbType.VarChar, 20).Value = _number;
                                cmd.Parameters.Add("@tipo", SqlDbType.Int, 11).Value = foto.Tipo;
                                cmd.Parameters.Add("@url_remoto", SqlDbType.VarChar, 200).Value = foto.Url;
                                cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = "File/Documentos/" + foto.Id.Trim() + ".jpg";
                                if (cmd.ExecuteNonQuery() > 0)
                                {
                                    // Se guardó el registro
                                }
                                else
                                {
                                    LOG("Error al guardar el registro de la Foto en el gestor documental");
                                }

                            }
                        }
                    }
                    else
                    {
                        if (this.RecuperarArchivoFirma(_number, foto.Id, foto.Url))
                        {
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();

                                cmd.Parameters.Add("@orden", SqlDbType.VarChar, 20).Value = _number;
                                cmd.Parameters.Add("@tipo", SqlDbType.Int, 11).Value = foto.Tipo;
                                cmd.Parameters.Add("@url_remoto", SqlDbType.VarChar, 200).Value = foto.Url;
                                cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = "File/Documentos/" + _number + "_" + foto.Id.Trim() + ".jpg";
                                if (cmd.ExecuteNonQuery() > 0)
                                {
                                    // Se guardó el registro
                                }
                                else
                                {
                                    LOG("Error al guardar el registro de la Firma en el gestor documental");
                                }

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

        private void RegistrarMedidorExistente(String acta, List<MedidorExistente> lista)
        {

            foreach (MedidorExistente medidor in lista)
            {
                try
                {
                    String sql = "INSERT INTO Acta_Medidor (_number,TipoMedidor,tipoRevision,numero,marca,tipo,"
                        + "tecnologia,lecturaUltimaFecha,lecturaUltima,lecturaActual,kdkh_tipo,kdkh_value,digitos,"
                        + "decimales,nFases,voltajeNominal,rangoCorrienteMin,rangoCorrienteMax,corrienteN_mec,"
                        + "corrienteFN_mec,voltageNT_mec,voltageRS_mec,voltageFNR_mec,voltageFTR_mec,corrienteR_mec,"
                        + "voltageFNS_mec,voltageFTS_mec,corrienteS_mec,pruebaAlta,voltageFNR_alta,corrienteR_alta,"
                        + "vueltasR_alta,tiempoR_alta,voltageFNS_alta,corrienteS_alta,vueltasS_alta,tiempoS_alta,"
                        + "errorPruebaR_alta,errorPruebaS_alta,pruebaBaja,voltageFNR_baja,corrienteR_baja,vueltasR_baja,"
                        + "tiempoR_baja,voltageFNS_baja,corrienteS_baja,vueltasS_baja,tiempoS_baja,errorPruebaR_baja,"
                        + "errorPruebaS_baja,pruebaDosificacion,voltageFNR_dosif,corrienteR_dosif,lecturaInicialR_dosif,"
                        + "lecturaFinalR_dosif,tiempoR_dosif,errorPruebaR_dosif,giroNormal,rozamiento,medidorFrena,"
                        + "estadoConexiones,continuidad,pruebaPuentes,display,estadoIntegrador,retirado,envioLab,"
                        + "envioLabNumCustodia,propietario,numeroCertificadoCalibracion,laboratorio,protocolo,"
                        + "resolucionAcreditacion,resolucionFecha,voltageFNT_mec,voltageFTT_mec,corrienteT_mec) "
                        + " VALUES (@_number,@TipoMedidor,@tipoRevision,@numero,@marca,@tipo,"
                        + "@tecnologia,@lecturaUltimaFecha,@lecturaUltima,@lecturaActual,@kdkh_tipo,@kdkh_value,@digitos,"
                        + "@decimales,@nFases,@voltajeNominal,@rangoCorrienteMin,@rangoCorrienteMax,@corrienteN_mec,"
                        + "@corrienteFN_mec,@voltageNT_mec,@voltageRS_mec,@voltageFNR_mec,@voltageFTR_mec,@corrienteR_mec,"
                        + "@voltageFNS_mec,@voltageFTS_mec,@corrienteS_mec,@pruebaAlta,@voltageFNR_alta,@corrienteR_alta,"
                        + "@vueltasR_alta,@tiempoR_alta,@voltageFNS_alta,@corrienteS_alta,@vueltasS_alta,@tiempoS_alta,"
                        + "@errorPruebaR_alta,@errorPruebaS_alta,@pruebaBaja,@voltageFNR_baja,@corrienteR_baja,@vueltasR_baja,"
                        + "@tiempoR_baja,@voltageFNS_baja,@corrienteS_baja,@vueltasS_baja,@tiempoS_baja,@errorPruebaR_baja,"
                        + "@errorPruebaS_baja,@pruebaDosificacion,@voltageFNR_dosif,@corrienteR_dosif,@lecturaInicialR_dosif,"
                        + "@lecturaFinalR_dosif,@tiempoR_dosif,@errorPruebaR_dosif,@giroNormal,@rozamiento,@medidorFrena,"
                        + "@estadoConexiones,@continuidad,@pruebaPuentes,@display,@estadoIntegrador,@retirado,@envioLab,"
                        + "@envioLabNumCustodia,@propietario,@numeroCertificadoCalibracion,@laboratorio,@protocolo,"
                        + "@resolucionAcreditacion,@resolucionFecha,@voltageFNT_mec,@voltageFTT_mec,@corrienteT_mec)";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();


                        cmd.Parameters.Add("@_number", SqlDbType.VarChar, 50).Value = acta;
                        cmd.Parameters.Add("@TipoMedidor", SqlDbType.VarChar, 50).Value = medidor.tipo;
                        cmd.Parameters.Add("@tipoRevision", SqlDbType.VarChar, 50).Value = medidor.tipoRevision;
                        cmd.Parameters.Add("@numero", SqlDbType.VarChar, 50).Value = medidor.numero;
                        cmd.Parameters.Add("@marca", SqlDbType.VarChar, 150).Value = medidor.marca;
                        cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 50).Value = medidor.tipo;
                        cmd.Parameters.Add("@tecnologia", SqlDbType.VarChar, 150).Value = medidor.tecnologia;
                        cmd.Parameters.Add("@lecturaUltimaFecha", SqlDbType.VarChar, 50).Value = medidor.lecturaUltimaFecha;
                        cmd.Parameters.Add("@lecturaUltima", SqlDbType.VarChar, 50).Value = medidor.lecturaUltima;
                        cmd.Parameters.Add("@lecturaActual", SqlDbType.VarChar, 50).Value = medidor.lecturaActual;
                        cmd.Parameters.Add("@kdkh_tipo", SqlDbType.VarChar, 50).Value = medidor.kdkh_tipo;
                        cmd.Parameters.Add("@kdkh_value", SqlDbType.VarChar, 50).Value = medidor.kdkh_value;
                        cmd.Parameters.Add("@digitos", SqlDbType.VarChar, 50).Value = medidor.digitos;
                        cmd.Parameters.Add("@decimales", SqlDbType.VarChar, 50).Value = medidor.decimales;
                        cmd.Parameters.Add("@nFases", SqlDbType.VarChar, 50).Value = medidor.nFases;
                        cmd.Parameters.Add("@voltajeNominal", SqlDbType.VarChar, 50).Value = medidor.voltajeNominal;
                        cmd.Parameters.Add("@rangoCorrienteMin", SqlDbType.VarChar, 50).Value = medidor.rangoCorrienteMin;
                        cmd.Parameters.Add("@rangoCorrienteMax", SqlDbType.VarChar, 50).Value = medidor.rangoCorrienteMax;
                        cmd.Parameters.Add("@corrienteN_mec", SqlDbType.VarChar, 50).Value = medidor.corrienteN_mec;
                        cmd.Parameters.Add("@corrienteFN_mec", SqlDbType.VarChar, 50).Value = medidor.corrienteFN_mec;
                        cmd.Parameters.Add("@voltageNT_mec", SqlDbType.VarChar, 50).Value = medidor.voltageNT_mec;
                        cmd.Parameters.Add("@voltageRS_mec", SqlDbType.VarChar, 50).Value = medidor.voltageRS_mec;
                        cmd.Parameters.Add("@voltageFNR_mec", SqlDbType.VarChar, 50).Value = medidor.voltageFNR_mec;
                        cmd.Parameters.Add("@voltageFTR_mec", SqlDbType.VarChar, 50).Value = medidor.voltageFTR_mec;
                        cmd.Parameters.Add("@corrienteR_mec", SqlDbType.VarChar, 50).Value = medidor.corrienteR_mec;
                        cmd.Parameters.Add("@voltageFNS_mec", SqlDbType.VarChar, 50).Value = medidor.voltageFNS_mec;
                        cmd.Parameters.Add("@voltageFTS_mec", SqlDbType.VarChar, 50).Value = medidor.voltageFTS_mec;
                        cmd.Parameters.Add("@corrienteS_mec", SqlDbType.VarChar, 50).Value = medidor.corrienteS_mec;
                        cmd.Parameters.Add("@pruebaAlta", SqlDbType.VarChar, 50).Value = medidor.pruebaAlta;
                        cmd.Parameters.Add("@voltageFNR_alta", SqlDbType.VarChar, 50).Value = medidor.voltageFNR_alta;
                        cmd.Parameters.Add("@corrienteR_alta", SqlDbType.VarChar, 50).Value = medidor.corrienteR_alta;
                        cmd.Parameters.Add("@vueltasR_alta", SqlDbType.VarChar, 50).Value = medidor.vueltasR_alta;
                        cmd.Parameters.Add("@tiempoR_alta", SqlDbType.VarChar, 50).Value = medidor.tiempoR_alta;
                        cmd.Parameters.Add("@voltageFNS_alta", SqlDbType.VarChar, 50).Value = medidor.voltageFNS_alta;
                        cmd.Parameters.Add("@corrienteS_alta", SqlDbType.VarChar, 50).Value = medidor.corrienteS_alta;
                        cmd.Parameters.Add("@vueltasS_alta", SqlDbType.VarChar, 50).Value = medidor.vueltasS_alta;
                        cmd.Parameters.Add("@tiempoS_alta", SqlDbType.VarChar, 50).Value = medidor.tiempoS_alta;
                        cmd.Parameters.Add("@errorPruebaR_alta", SqlDbType.VarChar, 50).Value = medidor.errorPruebaR_alta;
                        cmd.Parameters.Add("@errorPruebaS_alta", SqlDbType.VarChar, 50).Value = medidor.errorPruebaS_alta;
                        cmd.Parameters.Add("@pruebaBaja", SqlDbType.VarChar, 50).Value = medidor.pruebaBaja;
                        cmd.Parameters.Add("@voltageFNR_baja", SqlDbType.VarChar, 50).Value = medidor.voltageFNR_baja;
                        cmd.Parameters.Add("@corrienteR_baja", SqlDbType.VarChar, 50).Value = medidor.corrienteR_baja;
                        cmd.Parameters.Add("@vueltasR_baja", SqlDbType.VarChar, 50).Value = medidor.vueltasR_baja;
                        cmd.Parameters.Add("@tiempoR_baja", SqlDbType.VarChar, 50).Value = medidor.tiempoR_baja;
                        cmd.Parameters.Add("@voltageFNS_baja", SqlDbType.VarChar, 50).Value = medidor.voltageFNS_baja;
                        cmd.Parameters.Add("@corrienteS_baja", SqlDbType.VarChar, 50).Value = medidor.corrienteS_baja;
                        cmd.Parameters.Add("@vueltasS_baja", SqlDbType.VarChar, 50).Value = medidor.vueltasS_baja;
                        cmd.Parameters.Add("@tiempoS_baja", SqlDbType.VarChar, 50).Value = medidor.tiempoS_baja;
                        cmd.Parameters.Add("@errorPruebaR_baja", SqlDbType.VarChar, 50).Value = medidor.errorPruebaR_baja;
                        cmd.Parameters.Add("@errorPruebaS_baja", SqlDbType.VarChar, 50).Value = medidor.errorPruebaS_baja;
                        cmd.Parameters.Add("@pruebaDosificacion", SqlDbType.VarChar, 50).Value = medidor.pruebaDosificacion;
                        cmd.Parameters.Add("@voltageFNR_dosif", SqlDbType.VarChar, 50).Value = medidor.voltageFNR_dosif;
                        cmd.Parameters.Add("@corrienteR_dosif", SqlDbType.VarChar, 50).Value = medidor.corrienteR_dosif;
                        cmd.Parameters.Add("@lecturaInicialR_dosif", SqlDbType.VarChar, 50).Value = medidor.lecturaInicialR_dosif;
                        cmd.Parameters.Add("@lecturaFinalR_dosif", SqlDbType.VarChar, 50).Value = medidor.lecturaFinalR_dosif;
                        cmd.Parameters.Add("@tiempoR_dosif", SqlDbType.VarChar, 50).Value = medidor.tiempoR_dosif;
                        cmd.Parameters.Add("@errorPruebaR_dosif", SqlDbType.VarChar, 50).Value = medidor.errorPruebaR_dosif;
                        cmd.Parameters.Add("@giroNormal", SqlDbType.VarChar, 50).Value = medidor.giroNormal;
                        cmd.Parameters.Add("@rozamiento", SqlDbType.VarChar, 50).Value = medidor.rozamiento;
                        cmd.Parameters.Add("@medidorFrena", SqlDbType.VarChar, 50).Value = medidor.medidorFrena;
                        cmd.Parameters.Add("@estadoConexiones", SqlDbType.VarChar, 50).Value = medidor.estadoConexiones;
                        cmd.Parameters.Add("@continuidad", SqlDbType.VarChar, 50).Value = medidor.continuidad;
                        cmd.Parameters.Add("@pruebaPuentes", SqlDbType.VarChar, 50).Value = medidor.pruebaPuentes;
                        cmd.Parameters.Add("@display", SqlDbType.VarChar, 50).Value = medidor.display;
                        cmd.Parameters.Add("@estadoIntegrador", SqlDbType.VarChar, 50).Value = medidor.estadoIntegrador;
                        cmd.Parameters.Add("@retirado", SqlDbType.VarChar, 50).Value = medidor.retirado;
                        cmd.Parameters.Add("@envioLab", SqlDbType.VarChar, 50).Value = medidor.envioLab;
                        cmd.Parameters.Add("@envioLabNumCustodia", SqlDbType.VarChar, 50).Value = medidor.envioLabNumCustodia;
                        cmd.Parameters.Add("@propietario", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@numeroCertificadoCalibracion", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@laboratorio", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@protocolo", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@resolucionAcreditacion", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@resolucionFecha", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@voltageFNT_mec", SqlDbType.VarChar, 50).Value = medidor.voltageFNT_mec;
                        cmd.Parameters.Add("@voltageFTT_mec", SqlDbType.VarChar, 50).Value = medidor.voltageFTT_mec;
                        cmd.Parameters.Add("@corrienteT_mec", SqlDbType.VarChar, 50).Value = medidor.corrienteT_mec;


                        if (cmd.ExecuteNonQuery() > 0)
                        {

                            foreach (Sellos sello in medidor.sellos)
                            {
                                String cadena = "INSERT INTO Ac_Sellos (AcSeNuMe,AcSeNuAc,AcSeNuSe,AcSeEsta,AcSePosi,AcSeColo,AcSeTiSe,AcSeTipo) " +
                                            " VALUES (@AcSeNuMe,@AcSeNuAc,@AcSeNuSe,@AcSeEsta,@AcSePosi,@AcSeColo,@AcSeTiSe,@AcSeTipo)";
                                using (SqlCommand cmd2 = new SqlCommand(cadena))
                                {
                                    cmd2.Connection = conexion.getConection();
                                    cmd2.Parameters.Add("@AcSeNuMe", SqlDbType.VarChar, 50).Value = sello.Medidor;
                                    cmd2.Parameters.Add("@AcSeNuAc", SqlDbType.VarChar, 50).Value = sello.Acta;
                                    cmd2.Parameters.Add("@AcSeNuSe", SqlDbType.VarChar, 50).Value = sello.Serie;
                                    cmd2.Parameters.Add("@AcSeEsta", SqlDbType.VarChar, 50).Value = sello.Estado;
                                    cmd2.Parameters.Add("@AcSePosi", SqlDbType.VarChar, 50).Value = sello.Posicion;
                                    cmd2.Parameters.Add("@AcSeColo", SqlDbType.VarChar, 50).Value = sello.Color;
                                    cmd2.Parameters.Add("@AcSeTiSe", SqlDbType.VarChar, 50).Value = sello.Tipo;
                                    cmd2.Parameters.Add("@AcSeTipo", SqlDbType.VarChar, 50).Value = sello.Clasificacion;

                                    cmd2.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            LOG("Error al guardar el registro de medidor existente: " + acta);
                        }

                    }

                }
                catch (SqlException ex)
                {
                    LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
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
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_PENDIENTE_DE_PROCESO;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();
                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_PENDIENTE_DE_PROCESO, "1");
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

                                    int record = cmd.ExecuteNonQuery();
                                    if (record > 0)
                                    {
                                        AgregarBitacora((int)row["acta"], ESTADO_SIN_ANOMALIA, "1");
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

                                    int record = cmd.ExecuteNonQuery();
                                    if (record > 0)
                                    {
                                        //AgregarBitacora((int)row["acta"], ESTADO_REVISION_LIQUIDACION, "1");
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
                                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = ESTADO_REVISION_LIQUIDACION;
                                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = gb.CodigoBandeja;
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (Int32)row["acta"];
                                    cmd.Prepare();

                                    try
                                    {
                                        int record = cmd.ExecuteNonQuery();
                                        if (record > 0)
                                        {
                                            AgregarBitacora((int)row["acta"], ESTADO_PENDIENTE_DE_PROCESO, "1");
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

        public void ActualizarProtocoloOrdenServicio()
        {
            try
            {

                if (conexion != null)
                {
                    List<OrdenServicio> lista = new List<OrdenServicio>();
                    String sql = "SELECT Actas._number,Actas.nic, Acta_Medidor.numero "
                        + " FROM Actas with(nolock), Acta_Medidor "
                        + " WHERE Actas.protocolo=1 "
                        + " AND Actas._number = Acta_Medidor._number"
                        + " AND EstadoActa=1 "
                        + " AND conAnomalia = 1 "
                        + " ORDER BY fechaCarga";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            LOG("Consulta de OS pendiente de protocolo");
                            int contador = 0;
                            while (reader.Read())
                            {
                                WSMedidor ws = new WSMedidor();
                                ws.Nic = reader.GetString(1);
                                ws.Acta = reader.GetInt32(0).ToString();
                                ws.Medidor = reader.GetString(2);
                                LOG("Consultando WS SIGME NIC " + ws.Nic + " Acta " + ws.Acta + " Medidor " + ws.Medidor);
                                ws.CallWebService();
                                if (ws.protocolo != null)
                                {
                                    LOG("Protocolo encontrado acta " + reader.GetInt32(0));
                                    OrdenServicio os = new OrdenServicio();
                                    os.acta = reader.GetInt32(0);
                                    os.protocolo = ws.protocolo;
                                    lista.Add(os);
                                }
                                else
                                {
                                    LOG("Respuesta no valida de WS SIGME");
                                }
                                contador++;
                            }

                            if (contador == 0)
                            {
                                LOG("No hay OS pendientes de protoco");
                            }
                        }

                    }


                    if (lista.Count > 0)
                    {
                        foreach (OrdenServicio os in lista)
                        {
                            LOG("Cambiando estado <CON PROTOCOLO> acta " + os.acta);
                            if (this.RegistrarProtocolo(os.acta, os.protocolo))
                            {
                                sql = "UPDATE Actas SET "
                                    + " protocolo= '2' "
                                    + " WHERE _number=@acta "
                                    + " AND estadoActa = 1 "
                                    + " AND protocolo = 1";

                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = os.acta;
                                    cmd.Prepare();
                                    if (cmd.ExecuteNonQuery() > 0)
                                    {
                                        LOG("Protocolo acta " + os.acta + " REGISTRADO");
                                    }

                                }
                            }
                            else
                            {
                                LOG("Protocolo acta " + os.acta + " NO REGISTRADO");
                            }
                        }
                    }
                    else
                    {
                        LOG("No hay ordenes pendientes de protoco");
                    }

                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }

        }

        public bool RegistrarProtocolo(Int32 acta, Protocolo protoco)
        {
            bool result = false;
            String sql = "INSERT INTO Protocolo(NumeroActa,NumeroMedidor,Nic,Nis,ResultadoExactitud,"
                + "Fecha_Res_Exactitud,TipoEnergia,ResultadoPropieDialectrica,ResultadoArranque,"
                + "ResultadoEnsayoFuncioSinCarga,ResultadoInspeccionVisual,ResultadoVerificacionConstante,"
                + "ErrorPorcentual,ErrorporcenEnEnergiaReactiva,NumCertificado,CodLaboratorio,Ensayo1Activa,"
                + "Incertidumbre1Activa,Ensayo2Activa,Incertidumbre2Activa,Ensayo3Activa,"
                + "Incertidumbre3Activa,Ensayo4Activa,Incertidumbre4Activa,Ensayo5Activa,"
                + "Incertidumbre5Activa,Ensayo6Activa,Incertidumbre6Activa,Ensayo7Activa,"
                + "Incertidumbre7Activa,Ensayo8Activa,Incertidumbre8Activa,Ensayo1Reactiva,"
                + "Incertidumbre1Reactiva,Ensayo2Reactiva,Incertidumbre2Reactiva,Ensayo3Reactiva,"
                + "Incertidumbre3Reactiva,Ensayo4Reactiva,Incertidumbre4Reactiva,Ensayo5Reactiva,"
                + "Incertidumbre5Reactiva,Ensayo6Reactiva,Incertidumbre6Reactiva,Ensayo7Reactiva,"
                + "Incertidumbre7Reactiva) VALUES("
                + "@NumeroActa,@NumeroMedidor,@Nic,@Nis,@ResultadoExactitud,"
                + "@Fecha_Res_Exactitud,@TipoEnergia,@ResultadoPropieDialectrica,@ResultadoArranque,"
                + "@ResultadoEnsayoFuncioSinCarga,@ResultadoInspeccionVisual,@ResultadoVerificacionConstante,"
                + "@ErrorPorcentual,@ErrorporcenEnEnergiaReactiva,@NumCertificado,@CodLaboratorio,@Ensayo1Activa,"
                + "@Incertidumbre1Activa,@Ensayo2Activa,@Incertidumbre2Activa,@Ensayo3Activa,"
                + "@Incertidumbre3Activa,@Ensayo4Activa,@Incertidumbre4Activa,@Ensayo5Activa,"
                + "@Incertidumbre5Activa,@Ensayo6Activa,@Incertidumbre6Activa,@Ensayo7Activa,"
                + "@Incertidumbre7Activa,@Ensayo8Activa,@Incertidumbre8Activa,@Ensayo1Reactiva,"
                + "@Incertidumbre1Reactiva,@Ensayo2Reactiva,@Incertidumbre2Reactiva,@Ensayo3Reactiva,"
                + "@Incertidumbre3Reactiva,@Ensayo4Reactiva,@Incertidumbre4Reactiva,@Ensayo5Reactiva,"
                + "@Incertidumbre5Reactiva,@Ensayo6Reactiva,@Incertidumbre6Reactiva,@Ensayo7Reactiva,"
                + "@Incertidumbre7Reactiva)";
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Connection = conexion.getConection();

                cmd.Parameters.Add("@NumeroActa", SqlDbType.Int, 11).Value = acta;
                cmd.Parameters.Add("@NumeroMedidor", SqlDbType.VarChar, 20).Value = protoco.NumeroMedidor;
                cmd.Parameters.Add("@Nic", SqlDbType.VarChar, 20).Value = protoco.Nic;
                cmd.Parameters.Add("@Nis", SqlDbType.VarChar, 20).Value = protoco.Nis;
                cmd.Parameters.Add("@ResultadoExactitud", SqlDbType.VarChar, 10).Value = protoco.ResultadoExactitud;
                cmd.Parameters.Add("@Fecha_Res_Exactitud", SqlDbType.VarChar, 20).Value = protoco.Fecha_Res_Exactitud;
                cmd.Parameters.Add("@TipoEnergia", SqlDbType.VarChar, 20).Value = protoco.TipoEnergia;
                cmd.Parameters.Add("@ResultadoPropieDialectrica", SqlDbType.VarChar, 20).Value = protoco.ResultadoPropieDialectrica;
                cmd.Parameters.Add("@ResultadoArranque", SqlDbType.VarChar, 20).Value = protoco.ResultadoArranque;
                cmd.Parameters.Add("@ResultadoEnsayoFuncioSinCarga", SqlDbType.VarChar, 20).Value = protoco.ResultadoEnsayoFuncioSinCarga;
                cmd.Parameters.Add("@ResultadoInspeccionVisual", SqlDbType.VarChar, 20).Value = protoco.ResultadoInspeccionVisual;
                cmd.Parameters.Add("@ResultadoVerificacionConstante", SqlDbType.VarChar, 20).Value = protoco.ResultadoVerificacionConstante;


                cmd.Parameters.Add("@ErrorPorcentual", SqlDbType.Decimal, 12).Value = protoco.ErrorPorcentual;
                cmd.Parameters.Add("@ErrorporcenEnEnergiaReactiva", SqlDbType.Decimal, 12).Value = protoco.ErrorporcenEnEnergiaReactiva;

                cmd.Parameters.Add("@NumCertificado", SqlDbType.VarChar, 100).Value = protoco.NumCertificado;
                cmd.Parameters.Add("@CodLaboratorio", SqlDbType.VarChar, 100).Value = protoco.CodLaboratorio;

                cmd.Parameters.Add("@Ensayo1Activa", SqlDbType.Decimal, 12).Value = protoco.Ensayo1Activa;
                cmd.Parameters.Add("@Incertidumbre1Activa", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre1Activa;
                cmd.Parameters.Add("@Ensayo2Activa", SqlDbType.Decimal, 12).Value = protoco.Ensayo2Activa;
                cmd.Parameters.Add("@Incertidumbre2Activa", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre2Activa;
                cmd.Parameters.Add("@Ensayo3Activa", SqlDbType.Decimal, 12).Value = protoco.Ensayo3Activa;
                cmd.Parameters.Add("@Incertidumbre3Activa", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre3Activa;
                cmd.Parameters.Add("@Ensayo4Activa", SqlDbType.Decimal, 12).Value = protoco.Ensayo4Activa;
                cmd.Parameters.Add("@Incertidumbre4Activa", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre4Activa;
                cmd.Parameters.Add("@Ensayo5Activa", SqlDbType.Decimal, 12).Value = protoco.Ensayo5Activa;
                cmd.Parameters.Add("@Incertidumbre5Activa", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre5Activa;
                cmd.Parameters.Add("@Ensayo6Activa", SqlDbType.Decimal, 12).Value = protoco.Ensayo6Activa;
                cmd.Parameters.Add("@Incertidumbre6Activa", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre6Activa;
                cmd.Parameters.Add("@Ensayo7Activa", SqlDbType.Decimal, 12).Value = protoco.Ensayo7Activa;
                cmd.Parameters.Add("@Incertidumbre7Activa", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre7Activa;
                cmd.Parameters.Add("@Ensayo8Activa", SqlDbType.Decimal, 12).Value = protoco.Ensayo8Activa;
                cmd.Parameters.Add("@Incertidumbre8Activa", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre8Activa;
                cmd.Parameters.Add("@Ensayo1Reactiva", SqlDbType.Decimal, 12).Value = protoco.Ensayo1Reactiva;
                cmd.Parameters.Add("@Incertidumbre1Reactiva", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre1Reactiva;
                cmd.Parameters.Add("@Ensayo2Reactiva", SqlDbType.Decimal, 12).Value = protoco.Ensayo2Reactiva;
                cmd.Parameters.Add("@Incertidumbre2Reactiva", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre2Reactiva;
                cmd.Parameters.Add("@Ensayo3Reactiva", SqlDbType.Decimal, 12).Value = protoco.Ensayo3Reactiva;
                cmd.Parameters.Add("@Incertidumbre3Reactiva", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre3Reactiva;
                cmd.Parameters.Add("@Ensayo4Reactiva", SqlDbType.Decimal, 12).Value = protoco.Ensayo4Reactiva;
                cmd.Parameters.Add("@Incertidumbre4Reactiva", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre4Reactiva;
                cmd.Parameters.Add("@Ensayo5Reactiva", SqlDbType.Decimal, 12).Value = protoco.Ensayo5Reactiva;
                cmd.Parameters.Add("@Incertidumbre5Reactiva", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre5Reactiva;

                cmd.Parameters.Add("@Ensayo6Reactiva", SqlDbType.Decimal, 12).Value = protoco.Ensayo6Reactiva;
                cmd.Parameters.Add("@Incertidumbre6Reactiva", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre6Reactiva;
                cmd.Parameters.Add("@Ensayo7Reactiva", SqlDbType.Decimal, 12).Value = protoco.Ensayo7Reactiva;
                cmd.Parameters.Add("@Incertidumbre7Reactiva", SqlDbType.Decimal, 12).Value = protoco.Incertidumbre7Reactiva;

                if (cmd.ExecuteNonQuery() > 0)
                {
                    result = true;
                }
                else
                {
                    LOG("Error al guardar el registro de la Foto en el gestor documental");
                }

            }


            return result;
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

        public void RechazarActaSinAnomalia(int acta, string bandeja, string causal)
        {
            try
            {
                String sql = "insert into ActasRechazadas values(acta,causalrechazo,observacion,bandeja,usuario,fecha,observacion2,usuariodevolucion,fechaModificacion) "
                        + "VALUES (@acta,@causal,'Acta rechazada por interfaz por no registrar Anomlia en la HDA',@bandeja,'interfaz',SYSDATETIME(),'','interfaz',SYSDATETIME())";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();

                    cmd.Parameters.Add("@acta", SqlDbType.VarChar, 20).Value = acta;
                    cmd.Parameters.Add("@causal", SqlDbType.VarChar, 10).Value = causal;
                    cmd.Parameters.Add("@bandeja", SqlDbType.VarChar, 10).Value = bandeja;

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

        private class OrdenServicio
        {
            public Int32 acta { set; get; }
            public String nic { set; get; }
            public String tarifa { set; get; }
            public String medidor { set; get; }
            public Protocolo protocolo { set; get; }
            public DateTime fechaLevantamientoActa { set; get; }

        }

        public void ActualizarConsumosActa(String acta, String nic, String fecha, bool commit)
        {
            WSConsumo eca = new WSConsumo();
            eca.nic = nic;
            eca.fecha = fecha;
            eca.CallWebService();

            if (eca.ListaConsumos.Count > 0)
            {
                String sql = "DELETE FROM Consumo WHERE ConsActa=@acta";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@acta", SqlDbType.VarChar, 50).Value = acta;
                    cmd.ExecuteNonQuery();
                }


                for (int x = 0; x < eca.ListaConsumos.Count; x++)
                {
                    try
                    {

                        sql = "INSERT INTO Consumo (ConsActa,ConsFech,ConsValo) " +
                                        " VALUES (@acta,@fecha, @valor)";
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            cmd.Parameters.Add("@acta", SqlDbType.VarChar, 50).Value = acta;
                            cmd.Parameters.Add("@fecha", SqlDbType.VarChar, 50).Value = eca.ListaConsumos[x].fecha;
                            cmd.Parameters.Add("@valor", SqlDbType.VarChar, 50).Value = eca.ListaConsumos[x].consumo;

                            if (cmd.ExecuteNonQuery() > 0)
                            {

                            }
                            else
                            {
                                LOG("Error al actualizar el consumo: " + acta);
                            }
                        }
                    }

                    catch (SqlException ex)
                    {
                        LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
                    }
                }
            }
        }
    }



}
