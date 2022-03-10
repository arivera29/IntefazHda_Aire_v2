using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazHda
{
    class Foto
    {
        public String Id { set; get; }
        public String Url { set; get; }
        public int Tipo { set; get; }

        public int Firma { set; get; }

        public Foto()
        {
            Id = "";
            Url = "";
            Tipo = 0;
            Firma = 0;
        }

    }
}
