namespace dw_backoffice.Models
{
    public class NovoRegistro : Base
    {
        public class Dados
        {
            public int Id { get; set; }
        }

        public Dados Conteudo { get; set; }

    }
}
