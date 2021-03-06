using System.Collections.Generic;

namespace dw_backoffice.Models
{
    public class Plano : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public string Descricao { get; set; }   
            public int Qtd_ofertas { get; set; }
            public double Valor_mensal { get; set; }
            public double Valor_oferta { get; set; }
            public string Visualizacao { get; set; }
            public string Desc_visualizacao { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}