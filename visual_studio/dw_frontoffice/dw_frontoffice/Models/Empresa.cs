using System.Collections.Generic;

namespace dw_frontoffice.Models
{
    public class Empresa : Base
    {
        public class Dados
        {
            private string _Cnpj;

            public int Id { get; set; }
            public string Fantasia { get; set; }
            public string Razao_social { get; set; }
            public string Cnpj
            {
                get { return _Cnpj; }
                set { _Cnpj = Utils.FormatCNPJ(value); }
            }
            public string Insc_est { get; set; }
            public string Url { get; set; }
            public string Email_com { get; set; }
            public string Email_sac { get; set; }
            public string Fone_com { get; set; }
            public string Fone_sac { get; set; }
            public string Endereco { get; set; }
            public string Numero { get; set; }
            public string Complemento { get; set; }
            public string Bairro { get; set; }
            public string Cep { get; set; }
            public string Endereco_cob { get; set; }
            public string Numero_cob { get; set; }
            public string Complemento_cob { get; set; }
            public string Bairro_cob { get; set; }
            public string Cep_cob { get; set; }
            public int Id_cidade { get; set; }
            public int Id_cidade_cob { get; set; }
            public string Nome_cidade { get; set; }
            public string Uf { get; set; }
            public string Nome_cidade_cob { get; set; }
            public string Uf_cob { get; set; }
            public string Logo { get; set; }
            public int Id_qualificacao { get; set; }
            public string Desc_qualificacao { get; set; }
        }

        public List<Dados> Conteudo { get; set; }

    }
}
