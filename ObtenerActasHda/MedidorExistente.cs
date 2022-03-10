using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObtenerActasHda
{
    class MedidorExistente
    {
        public string tipoRevision { set; get; }
        public string numero { set; get; }
        public string marca { set; get; }
        public string tipo { set; get; }
        public string tecnologia { set; get; }
        public string lecturaUltimaFecha { set; get; }
        public string lecturaUltima { set; get; }
        public string lecturaActual { set; get; }
        public string kdkh_tipo { set; get; }
        public string kdkh_value { set; get; }
        public string digitos { set; get; }
        public string decimales { set; get; }
        public string nFases { set; get; }
        public string voltajeNominal { set; get; }
        public string rangoCorrienteMin { set; get; }
        public string rangoCorrienteMax { set; get; }
        public string corrienteN_mec { set; get; }
        public string corrienteFN_mec { set; get; }
        public string voltageNT_mec { set; get; }
        public string voltageRS_mec { set; get; }
        public string voltageFNR_mec { set; get; }
        public string voltageFTR_mec { set; get; }
        public string corrienteR_mec { set; get; }
        public string voltageFNS_mec { set; get; }
        public string voltageFTS_mec { set; get; }
        public string corrienteS_mec { set; get; }

        public string voltageFNT_mec { set; get; }
        public string voltageFTT_mec { set; get; }
        public string corrienteT_mec { set; get; }

        public string pruebaAlta { set; get; }
        public string voltageFNR_alta { set; get; }
        public string corrienteR_alta { set; get; }
        public string vueltasR_alta { set; get; }
        public string tiempoR_alta { set; get; }
        public string voltageFNS_alta { set; get; }
        public string corrienteS_alta { set; get; }
        public string vueltasS_alta { set; get; }
        public string tiempoS_alta { set; get; }
        public string errorPruebaR_alta { set; get; }
        public string errorPruebaS_alta { set; get; }
        public string pruebaBaja { set; get; }
        public string voltageFNR_baja { set; get; }
        public string corrienteR_baja { set; get; }
        public string vueltasR_baja { set; get; }
        public string tiempoR_baja { set; get; }
        public string voltageFNS_baja { set; get; }
        public string corrienteS_baja { set; get; }
        public string vueltasS_baja { set; get; }
        public string tiempoS_baja { set; get; }
        public string errorPruebaR_baja { set; get; }
        public string errorPruebaS_baja { set; get; }
        public string pruebaDosificacion { set; get; }
        public string fotosPruebaDosificacion { set; get; }
        public string voltageFNR_dosif { set; get; }
        public string corrienteR_dosif { set; get; }
        public string lecturaInicialR_dosif { set; get; }
        public string lecturaFinalR_dosif { set; get; }
        public string tiempoR_dosif { set; get; }
        public string errorPruebaR_dosif { set; get; }
        public string giroNormal { set; get; }
        public string rozamiento { set; get; }
        public string medidorFrena { set; get; }
        public string estadoConexiones { set; get; }
        public string continuidad { set; get; }
        public string pruebaPuentes { set; get; }
        public string display { set; get; }
        public string estadoIntegrador { set; get; }
        public string retirado { set; get; }
        public string envioLab { set; get; }
        public string envioLabNumCustodia { set; get; }

        public List<Sellos> sellos;

        public MedidorExistente()
        {
            tipoRevision = "";
            numero = "";
            marca = "";
            tipo = "";
            tecnologia = "";
            lecturaUltimaFecha = "";
            lecturaUltima = "";
            lecturaActual = "";
            kdkh_tipo = "";
            kdkh_value = "";
            digitos = "";
            decimales = "";
            nFases = "";
            voltajeNominal = "";
            rangoCorrienteMin = "";
            rangoCorrienteMax = "";
            corrienteN_mec = "";
            corrienteFN_mec = "";
            voltageNT_mec = "";
            voltageRS_mec = "";
            voltageFNR_mec = "";
            voltageFTR_mec = "";
            corrienteR_mec = "";
            voltageFNS_mec = "";
            voltageFTS_mec = "";
            corrienteS_mec = "";
            pruebaAlta = "";
            voltageFNR_alta = "";
            corrienteR_alta = "";
            vueltasR_alta = "";
            tiempoR_alta = "";
            voltageFNS_alta = "";
            corrienteS_alta = "";
            vueltasS_alta = "";
            tiempoS_alta = "";
            errorPruebaR_alta = "";
            errorPruebaS_alta = "";
            pruebaBaja = "";
            voltageFNR_baja = "";
            corrienteR_baja = "";
            vueltasR_baja = "";
            tiempoR_baja = "";
            voltageFNS_baja = "";
            corrienteS_baja = "";
            vueltasS_baja = "";
            tiempoS_baja = "";
            errorPruebaR_baja = "";
            errorPruebaS_baja = "";
            pruebaDosificacion = "";
            fotosPruebaDosificacion = "";
            voltageFNR_dosif = "";
            corrienteR_dosif = "";
            lecturaInicialR_dosif = "";
            lecturaFinalR_dosif = "";
            tiempoR_dosif = "";
            errorPruebaR_dosif = "";
            giroNormal = "";
            rozamiento = "";
            medidorFrena = "";
            estadoConexiones = "";
            continuidad = "";
            pruebaPuentes = "";
            display = "";
            estadoIntegrador = "";
            retirado = "";
            envioLab = "";
            envioLabNumCustodia = "";
            sellos = new List<Sellos>();
        }

    }
}
