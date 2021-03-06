using System.Collections.Generic;

namespace dw_frontoffice.Models
{
    public class Situacao : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public string Descricao { get; set; }
            public string Contratos { get; set; }
            public string Usuarios { get; set; }
            public string Desejos { get; set; }
            public string Faturas { get; set; }
        }

        public List<Dados> Conteudo { get; set; }
 
    }
}