namespace dw_webservice.Models
{
    public class TraillerNF
    {
        public string Conteudo { get; set; }

        public TraillerNF(int total_linhas, double total_servicos, double total_impostos)
        {
           
            string Tipo_registro = "9";
            string num_total_linhas = total_linhas.ToString().PadLeft(7,'0');
            string valor_total_servicos = total_servicos.ToString("#.00").Replace(",", "").Replace(".", "").PadLeft(15, '0');
            string valor_total_impostos = total_impostos.ToString("#.00").Replace(",", "").Replace(".", "").PadLeft(15, '0');

            Conteudo = Tipo_registro + num_total_linhas + valor_total_servicos + valor_total_impostos;
        }
    }
}
