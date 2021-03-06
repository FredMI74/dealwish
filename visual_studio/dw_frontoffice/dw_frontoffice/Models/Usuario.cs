using System;
using System.Collections.Generic;


namespace dw_frontoffice.Models
{

    public class Usuario : Base
    {
        public class Dados
        {
            private string _Cpf;

            public long Id { get; set; }
            public string Email { get; set; }
            public string Senha { get; set; }
            public string Nome { get; set; }
            public DateTime Data_nasc { get; set; }
            public string Cpf
            {
                get { return _Cpf; }
                set { _Cpf = Utils.FormatCPF(value); }
            }
            public string Aplicativo { get; set; }
            public string Retaguarda { get; set; }
            public string Empresa { get; set; }
            public int Id_cidade_ap { get; set; }
            public string Nome_cidade_ap { get; set; }
            public int Id_situacao { get; set; }
            public string Desc_situacao { get; set; }
            public int Id_empresa { get; set; }
            public string Razao_social { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}