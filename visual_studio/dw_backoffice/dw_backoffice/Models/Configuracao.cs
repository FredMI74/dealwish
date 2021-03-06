using System.Collections.Generic;

namespace dw_backoffice.Models
{
    public class Configuracao : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Valor { get; set; }
        }

        public List<Dados> Conteudo { get; set; }
  
    }
}