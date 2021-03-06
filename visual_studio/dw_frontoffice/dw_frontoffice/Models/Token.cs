namespace dw_frontoffice.Models
{
    public class Token : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public int Id_empresa { get; set; }
            public string Nome { get; set; }
            public string Token { get; set; }
            public string TokenJwt { get; set; }
            public string Grp_permissoes { get; set; }
        }

        public Dados Conteudo { get; set; }
        
    }
}
