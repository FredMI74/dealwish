using System.Collections.Generic;

namespace dw_backoffice.Models
{
    public class TpProduto : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public string Descricao { get; set; }
            public int Id_grp_prod { get; set; }
            public string Desc_grp_produto { get; set; }
            public int Id_situacao { get; set; }
            public string Desc_situacao { get; set; }
            public string Preenchimento { get; set; }
            public string Icone { get; set; }
            public int Ordem { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}