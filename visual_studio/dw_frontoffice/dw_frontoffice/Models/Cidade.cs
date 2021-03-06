using System.Collections.Generic;

namespace dw_frontoffice.Models
{
    public class Cidade : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string Uf { get; set; }

            public string Name
            {
                get { return Nome + "/" + Uf; }
            }
        }

        public List<Dados> Conteudo { get; set; }

    }
}