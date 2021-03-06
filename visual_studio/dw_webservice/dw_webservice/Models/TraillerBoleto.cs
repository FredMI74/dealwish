using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dw_webservice.Models
{
    public class TraillerBoleto
    {
        public string Conteudo { get; set; }

        public TraillerBoleto(string banco, int seq_registro)
        {
            if (banco == "237") //Bradesco
            {

               string Id_registro = "9";
               string Branco = " ".PadLeft(393, ' ');
               string Seq_registro = seq_registro.ToString().PadLeft(6, '0');

               Conteudo = Id_registro + Branco + Seq_registro;

            }
    }


    }
}
