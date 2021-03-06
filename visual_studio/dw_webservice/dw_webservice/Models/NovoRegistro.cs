namespace dw_webservice.Models
{
    public class NovoRegistro
    {
        public class Dados
        {
            public long Id { get; set; }
        }

        public class Retorno
        {
            public bool Erro { get; set; }
            public string Mensagem { get; set; }
        }

        public Dados Conteudo { get; set; }
        public Retorno Resultado { get; set; }

        public NovoRegistro()
        {
            Conteudo = new Dados();
            Resultado = new Retorno();
        }
    }
}
