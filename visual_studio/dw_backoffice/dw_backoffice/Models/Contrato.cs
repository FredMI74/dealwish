using System;
using System.Collections.Generic;

namespace dw_backoffice.Models
{
    public class Contrato : Base
     {
        public class Dados
        {
            public int Id { get; set; }
            public int Id_empresa { get; set; }
            public string Razao_social { get; set; }
            public int Id_plano { get; set; }
            public string Desc_plano { get; set; }
            public int Id_situacao { get; set; }
            public string Desc_situacao { get; set; }
            public int Dia_vct { get; set; }
            public DateTime Data_inicio { get; set; }
            public DateTime Data_bloqueio { get; set; }
            public DateTime Data_termino { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}