namespace dw_webservice.Models
{
    public class HeaderRetornoNF
    {
        public string Tipo_registro { get; set; }
        public string Insc_contribuinte { get; set; }
        public string Ini_periodo_transf { get; set; }
        public string Fim_periodo_transf { get; set; }
        public string Versao_layout { get; set; }
        public string Seq_remessa { get; set; }
        public bool Valido { get; set; }

        public HeaderRetornoNF(string conteudo)
        {
            Valido = conteudo.Length == 41;
             
            if (Valido)
            {
                Tipo_registro = conteudo.Substring(0, 1);
                Insc_contribuinte = conteudo.Substring(1, 7);
                Ini_periodo_transf = conteudo.Substring(8, 8);
                Fim_periodo_transf = conteudo.Substring(16, 8);
                Versao_layout = conteudo.Substring(24, 6);
                Seq_remessa = conteudo.Substring(30, 11);
               
            }
        }
    }
}
