using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dw_webservice.Models
{
    public class HeaderBoleto
    {
        public string Conteudo { get; set; }
        public string Nome_arquivo { get; set; }

        public HeaderBoleto(string banco, string cod_empresa, string nome_empresa, string seq_remessa, string seq_dia)
        {
            if (banco == "237") //Bradesco
            {
                DateTime _hoje = DateTime.Today;

               string Id_registro = "0";
               string Id_arquivo_remessa = "1";
               string Literal_remessa = "REMESSA";
               string Cod_servico = "01";
               string Literal_servico = "COBRANCA".PadRight(15, ' ');
               string Cod_empresa = cod_empresa.PadLeft(20, '0');
               string Nome_empresa = nome_empresa.PadRight(30, ' ');
               string Numero_camara_compensacao = banco;
               string Nome_banco = "BRADESCO".PadRight(15, ' ');
               string Data_geracao_arquivo = _hoje.ToString("ddMMyy");
               string Branco_1 = " ".PadLeft(8, ' ');
               string Id_sistema = "MX";
               string Seq_remessa = seq_remessa.PadLeft(7, '0');
               string Branco_2 = " ".PadLeft(277, ' ');
               string Seq_registro = "000001";

               Conteudo = Id_registro + Id_arquivo_remessa + Literal_remessa + Cod_servico + Literal_servico + Cod_empresa +
                          Nome_empresa + Numero_camara_compensacao + Nome_banco + Data_geracao_arquivo + Branco_1 +
                          Id_sistema + Seq_remessa + Branco_2 + Seq_registro;

               Nome_arquivo = "CB" + _hoje.ToString("ddMM") + seq_dia.PadLeft(2, '0') + ".REM";

            }
        }


    }
}
