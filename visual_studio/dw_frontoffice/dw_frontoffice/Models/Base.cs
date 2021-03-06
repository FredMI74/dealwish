namespace dw_frontoffice.Models
{
    public class Base
    {
        public class Retorno
        {
            public bool Erro { get; set; }
            public string Mensagem { get; set; }
        }

        public Retorno Resultado { get; set; }

        public class Paginacao
        {
            public int Max_id { get; set; }
            public int Count_id { get; set; }
        }

        public Paginacao InfoPagina { get; set; }
    }
}