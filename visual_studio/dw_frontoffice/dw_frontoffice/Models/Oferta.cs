using System;
using System.Collections.Generic;

namespace dw_frontoffice.Models
{
    public class Oferta : Base
    {
        public class Dados
        {
            public long Id { get; set; }
            public int Id_desejo { get; set; }
            public dynamic Id_empresa { get; set; }
            public string Fantasia { get; set; }
            public string Desc_qualificacao { get; set; }
            public DateTime Validade { get; set; }
            public double Valor { get; set; }
            public string Url { get; set; }
            public string Descricao { get; set; }
            public string Id_situacao { get; set; }
            public string Desc_situacao { get; set; }
            public string Lida { get; set; }
            public string Like_unlike { get; set; }
            public string Destaque { get; set; }
            public string Usuario_inclusao { get; set; }
            public DateTime Data { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}