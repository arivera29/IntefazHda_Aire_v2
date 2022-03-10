using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazHda
{
    class Material
    {
        public string CodigoMaterial { set; get; }
        public string DescripcionMaterial { set; get; }
        public long Cantidad { set; get; }
        public int Cobro { set; get; }

        public Material()
        {
            this.CodigoMaterial = "";
            this.DescripcionMaterial = "";
            this.Cantidad = 0;
            this.Cobro = 0;
        }

    }
}
