using System.Collections.Generic;

namespace dw_backoffice.Models
{
    public class Qualificacao : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public string Descricao { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}