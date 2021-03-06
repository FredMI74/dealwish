namespace dw_webservice.Models
{
    public class DetalheRetornoNF
    {
        public string Tipo_registro { get; set; }
        public string Serie_nf { get; set; }
        public string Num_nf { get; set; }
        public string Data_nf { get; set; }
        public string Hora_nf { get; set; }
        public string Cod_autenticidade { get; set; }
        public string Serie_rps { get; set; }
        public string Num_rps { get; set; }
        public string Tributacao { get; set; }
        public string ISS_retido { get; set; }
        public string Situacao_nf { get; set; }
        public string Data_cancelamento { get; set; }
        public string Num_guia { get; set; }
        public string Data_pg_guia { get; set; }
        public string CPF_CPNJ_tomador { get; set; }
        public string Razao_social_tomador { get; set; }
        public string Endereco_tomador { get; set; }
        public string Num_endereco_tomador { get; set; }
        public string Complemento_tomador  { get; set; }
        public string Bairro_tomador { get; set; }
        public string Cidade_tomador { get; set; }
        public string Cep_tomador { get; set; }
        public string Pais_tomador { get; set; }
        public string Email_tomador { get; set; }
        public string Discriminacao_servico { get; set; }
        public bool Valido { get; set; }

        public DetalheRetornoNF(string conteudo)
        {
            Valido = conteudo.Length == 1628;

            if (Valido)
            {
                Tipo_registro = conteudo.Substring(0, 1);
                Serie_nf  = conteudo.Substring(1, 5);
                Num_nf = conteudo.Substring(6, 6);
                Data_nf = conteudo.Substring(12, 8);
                Hora_nf = conteudo.Substring(20, 6);
                Cod_autenticidade = conteudo.Substring(26, 24);
                Serie_rps = conteudo.Substring(50, 4);
                Num_rps = conteudo.Substring(54, 10);
                Tributacao = conteudo.Substring(64, 1);
                ISS_retido = conteudo.Substring(65, 1);
                Situacao_nf = conteudo.Substring(66, 1);
                Data_cancelamento = conteudo.Substring(67, 8);
                Num_guia = conteudo.Substring(75, 10);
                Data_pg_guia = conteudo.Substring(84, 8);
                CPF_CPNJ_tomador = conteudo.Substring(93, 14);
                Razao_social_tomador = conteudo.Substring(107, 100);
                Endereco_tomador = conteudo.Substring(207, 100);
                Num_endereco_tomador = conteudo.Substring(307, 100);
                Complemento_tomador  = conteudo.Substring(316, 20);
                Bairro_tomador = conteudo.Substring(336, 40);
                Cidade_tomador = conteudo.Substring(376, 40);
                Cep_tomador = conteudo.Substring(418, 8);
                Pais_tomador = conteudo.Substring(426, 50);
                Email_tomador = conteudo.Substring(476, 152);
                Discriminacao_servico = conteudo.Substring(628, 1000);

            }

        }
    }
}

