using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazHda
{
    class Acta
    {
        public string Id { set; get; }
        public string _number { set; get; }
        public string numeroLote { set; get; }
        public string codigoEmpresa { set; get; }
        public string CodigoContrata { set; get; }
        public string tipoOrdenServicio { set; get; }
        public string tipoServicio { set; get; }
        public string comentario1 { set; get; }
        public string comentario2 { set; get; }
        public string direccion { set; get; }
        public string nic { set; get; }
        public string tipoCliente { set; get; }
        public string estrato { set; get; }
        public string cargaContratada { set; get; }
        public string departamento { set; get; }
        public string municipio { set; get; }
        public string localidad { set; get; }
        public string tipoVia { set; get; }
        public string calle { set; get; }
        public string numeroPuerta { set; get; }
        public string duplicador { set; get; }
        public string piso { set; get; }
        public string referenciaDireccion { set; get; }
        public string acceso { set; get; }
        public string numeroCircuito { set; get; }
        public string matriculaCT { set; get; }
        public string fotoFachada { set; get; }
        public string nombreTitularContrato { set; get; }
        public string apellido1TitularContrato { set; get; }
        public string apellido2TitularContrato { set; get; }
        public string cedulaTitularContrato { set; get; }
        public string telefonoFijoTitularContrato { set; get; }
        public string telefonoMovilTitularContrato { set; get; }
        public string emailTitularContrato { set; get; }
        public string relacionReceptorVisita { set; get; }
        public string solicitaTecnicoReceptorVisita { set; get; }
        public string aportaTestigo { set; get; }
        public string nombreReceptorVisita { set; get; }
        public string apellido1ReceptorVisita { set; get; }
        public string apellido2ReceptorVisita { set; get; }
        public string cedulaReceptorVisita { set; get; }
        public string telefonoFijoReceptorVisita { set; get; }
        public string telefonoMovilReceptorVisita { set; get; }
        public string emailReceptorVisita { set; get; }
        public string nombreTecnico { set; get; }
        public string apellido1Tecnico { set; get; }
        public string apellido2Tecnico { set; get; }
        public string cedulaTecnico { set; get; }
        public string comteTecnico { set; get; }
        public string nombreTestigo { set; get; }
        public string apellido1Testigo { set; get; }
        public string apellido2Testigo { set; get; }
        public string cedulaTestigo { set; get; }
        public string aparatosExistentes { set; get; }
        public string aparatosInstalados { set; get; }
        public string tarifa { set; get; }
        public string observacionAnomalia { set; get; }
        public string residuosRecolectados { set; get; }
        public string clasificacionResiduos { set; get; }
        public string ordenAseo { set; get; }
        public string recibidoQuejas { set; get; }
        public string atendidoQuejas { set; get; }
        public DateTime fechaInicioIrregularidad { set; get; }
        public string observaciones { set; get; }
        public string tipoCalculo { set; get; }
        public int protocolo { set; get; }
        public DateTime fechaModificacion { set; get; }

        public bool actaSinAnomalia { set; get; }

        public double censoCargaInstalada { set; get; }
        public string tipoCenso { set; get; }
        public List<MedidorExistente> medidorExistente = new List<MedidorExistente>();
        public List<Accion> acciones = new List<Accion>();
        public List<Anomalia> anomalias = new List<Anomalia>();
        public List<Censo> censo = new List<Censo>();
        public List<Foto> fotos = new List<Foto>();
        public int medidorRetirado;
        public int medidorEnviadoLaboratorio;
        public string medidaAnomaliaTipo { set; get; }
        public string medidaAnomaliaVR
        {
            set;
            get;
        }
        public string medidaAnomaliaVS
        {
            set;
            get;
        }
        public string medidaAnomaliaVT
        {
            set;
            get;
        }
        public string medidaAnomaliaIR
        {
            set;
            get;
        }
        public string medidaAnomaliaIS
        {
            set;
            get;
        }
        public string medidaAnomaliaIT
        {
            set;
            get;
        }
        public string fechaCierre { set; get; }
        public string nombreOperario { set; get; }
        public string apellido1Operario { set; get; }
        public string apellido2Operario { set; get; }
        public string cedulaOperario { set; get; }
        public string empresaOperario { set; get; }

        public string _clientCloseTs { set; get; }


        public Acta()
        {
            Id = "";
            _number = "";
            numeroLote = "";
            codigoEmpresa = "";
            tipoOrdenServicio = "";
            tipoServicio = "";
            comentario1 = "";
            comentario2 = "";
            direccion = "";
            nic = "";
            tipoCliente = "";
            estrato = "";
            cargaContratada = "";
            departamento = "";
            municipio = "";
            localidad = "";
            tipoVia = "";
            calle = "";
            numeroPuerta = "";
            duplicador = "";
            piso = "";
            referenciaDireccion = "";
            acceso = "";
            numeroCircuito = "";
            matriculaCT = "";
            fotoFachada = "";
            nombreTitularContrato = "";
            apellido1TitularContrato = "";
            apellido2TitularContrato = "";
            cedulaTitularContrato = "";
            telefonoFijoTitularContrato = "";
            telefonoMovilTitularContrato = "";
            emailTitularContrato = "";
            relacionReceptorVisita = "";
            solicitaTecnicoReceptorVisita = "";
            aportaTestigo = "";
            nombreReceptorVisita = "";
            apellido1ReceptorVisita = "";
            apellido2ReceptorVisita = "";
            cedulaReceptorVisita = "";
            telefonoFijoReceptorVisita = "";
            telefonoMovilReceptorVisita = "";
            emailReceptorVisita = "";
            nombreTecnico = "";
            apellido1Tecnico = "";
            apellido2Tecnico = "";
            cedulaTecnico = "";
            comteTecnico = "";
            nombreTestigo = "";
            apellido1Testigo = "";
            apellido2Testigo = "";
            cedulaTestigo = "";
            aparatosExistentes = "";
            aparatosInstalados = "";
            tarifa = "";
            observacionAnomalia = "";
            residuosRecolectados = "";
            clasificacionResiduos = "";
            ordenAseo = "";
            recibidoQuejas = "";
            atendidoQuejas = "";
            fechaInicioIrregularidad = DateTime.Now;
            observaciones = "";
            tipoCalculo = "";
            protocolo = 0;
            actaSinAnomalia = false;
            tipoCenso = "";
            censoCargaInstalada = 0.0;
            medidorRetirado = 0;
            medidorEnviadoLaboratorio = 0;
            CodigoContrata = "";
            medidaAnomaliaTipo = "";
            medidaAnomaliaVR = "0";
            medidaAnomaliaVS = "0";
            medidaAnomaliaVT = "0";
            medidaAnomaliaIR = "0";
            medidaAnomaliaIS = "0";
            medidaAnomaliaIT = "0";
            fechaCierre = "";
            nombreOperario="";
            apellido1Operario="";
            apellido2Operario="";
            cedulaOperario = "";
            empresaOperario="";
            _clientCloseTs = "";
            
        }

    }
}
