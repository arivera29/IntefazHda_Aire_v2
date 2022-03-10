using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazHda
{
    class Sellos
    {
        public string Acta { set; get; }
        public string Medidor { set; get; }
        public string Serie { set; get; }
        public string Estado { set; get; }
        public string Color { set; get; }
        public string Posicion { set; get; }
        public string Tipo { set; get; }
        public int Clasificacion { set; get; }

        public Sellos()
        {
            this.Acta = "";
            this.Medidor = "";
            this.Serie = "";
            this.Estado = "";
            this.Color = "";
            this.Posicion = "";
            this.Tipo = "";
            this.Clasificacion = 0;
        }
    }
}
