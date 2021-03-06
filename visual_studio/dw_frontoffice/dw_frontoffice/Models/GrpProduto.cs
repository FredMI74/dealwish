using System.Collections.Generic;

namespace dw_frontoffice.Models
{
    public class GrpProduto : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public string Descricao { get; set; }
            public string Icone { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}