using System.Collections.Generic;

namespace dw_frontoffice.Models
{
    public class Desejo : Base
    {
        public class Dados
        {
            public long Id { get; set; }
            public string Descricao { get; set; }
            public string Icone_tp_produto { get; set; }
            public int Id_tipo_produto { get; set; }
            public string Desc_tp_produto { get; set; }
            public int Id_situacao { get; set; }
            public string Desc_situacao { get; set; }
            public double Qtd_ofertas { get; set; }
            public string Uf { get; set; }
            public string Cidade { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}