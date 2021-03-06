using System.Collections.Generic;

namespace dw_frontoffice.Models
{
    public class GrpPermissoesUsuario : Base
    {
        public class Dados
        {
            public int Id { get; set; }
            public int Id_grp_permissao { get; set; }
            public string Descricao_grp_permissao { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}