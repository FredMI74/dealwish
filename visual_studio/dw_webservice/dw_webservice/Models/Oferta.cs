namespace dw_webservice.Models
{
    public class Oferta
    {
        public int Id { get; set; }
        public long Id_desejo { get; set; }
        public long Id_usuario { get; set; }
        public int Id_empresa { get; set; }
        public string Fantasia { get; set; }
        public string Validade { get; set; }
        public double Valor { get; set; }
        public string Url { get; set; }
        public string Descricao { get; set; }
        public string Destaque { get; set; }

        public Oferta(string[] valores)
        {
            Id_desejo = long.Parse(valores[0]);
            Valor = double.Parse(valores[1]);
            Validade = valores[2];
            Url = valores[3];
            Descricao = valores[4];
            Destaque = valores[5];
        }
    }
}
