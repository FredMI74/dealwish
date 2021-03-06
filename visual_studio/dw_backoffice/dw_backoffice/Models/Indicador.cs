using System.Collections.Generic;

namespace dw_backoffice.Models
{
    public class Indicador : Base
    {
        public class Dados
        {
            public int Ordem { get; set; }
            public string Indicador { get; set; }
            public string Valor { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}