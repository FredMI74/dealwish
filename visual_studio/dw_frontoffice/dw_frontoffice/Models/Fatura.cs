using System;
using System.Collections.Generic;

namespace dw_frontoffice.Models
{
    public class Fatura : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public int Mes { get; set; }
            public int Ano { get; set; }
            public int Id_empresa { get; set; }
            public string Nosso_numero { get; set; }
            public double Valor { get; set; }
            public DateTime Data_vct { get; set; }
            public DateTime Data_pg { get; set; }
            public double Multa { get; set; }
            public double Juros { get; set; }
            public double Valor_pg { get; set; }
            public int Qtd_ofertas { get; set; }
            public double Id_situacao { get; set; }
            public string Razao_social { get; set; }
            public string Desc_situacao { get; set; }
            public string Pix { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}