using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObtenerActasHda
{
    class Accion
    {
        public string Acta { set; get; }
        public string CodigoPaso { set; get; }
        public string CodigoAccion { set; get; }
        public string DescripcionAccion { set; get; }
        public int NuevoPaso { set; get; }
        public int Cobro { set; get; }
        public List<Material> Materiales;

        public Accion()
        {
            this.Acta = "";
            this.CodigoPaso = "";
            this.CodigoAccion = "";
            this.DescripcionAccion = "";
            this.NuevoPaso = 0;
            this.Cobro = 0;
            this.Materiales = new List<Material>();
        }
    }
}
