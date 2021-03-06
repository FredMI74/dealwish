using System;

namespace dw_webservice.Models
{
    public class HeaderNF
    {
        public string Conteudo { get; set; }
        public string Nome_arquivo { get; set; }

        public HeaderNF(string inscricao_contribuinte,  string seq_remessa)
        {
               DateTime _hoje = DateTime.Today;

               string Tipo_registro = "1";
               string Inscricao_contribuinte = inscricao_contribuinte;
               string Versao_layout = "PMB002";
               string Id_remessa_contribuinte = seq_remessa.PadLeft(11, '0');

               Conteudo = Tipo_registro + Inscricao_contribuinte + Versao_layout + Id_remessa_contribuinte;

               Nome_arquivo = "NF" + _hoje.ToString("ddMMyyyy") + seq_remessa.PadLeft(11, '0') + ".REM";
        }
    }
}
