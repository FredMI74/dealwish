namespace dw_webservice.Models
{
    public class HeaderRetornoBoleto
    {
        public string Id_registro { get; set; }
        public string Id_arquivo_retorno { get; set; }
        public string Literal_retorno { get; set; }
        public string Cod_serviço { get; set; }
        public string Literal_servico_cobranca { get; set; }
        public string Cod_empresa { get; set; }
        public string Nome_empresa { get; set; }
        public string Num_banco { get; set; }
        public string Nome_banco { get; set; }
        public string Data_gravacao { get; set; }
        public string Densidade_gravacao { get; set; }
        public string Num_aviso_bancario { get; set; }
        public string Branco1 { get; set; }
        public string Data_credito { get; set; }
        public string Branco2 { get; set; }
        public string Seq_registro { get; set; }
        public bool Valido { get; set; }

        public HeaderRetornoBoleto(string conteudo)
        {
            Valido = conteudo.Length == 400;
             
            if (Valido)
            {
                Id_registro = conteudo.Substring(0, 1);
                Id_arquivo_retorno = conteudo.Substring(1, 1);
                Literal_retorno = conteudo.Substring(2, 7);
                Cod_serviço = conteudo.Substring(9, 2);
                Literal_servico_cobranca = conteudo.Substring(11, 15);
                Cod_empresa = conteudo.Substring(26, 20);
                Nome_empresa = conteudo.Substring(46, 30);
                Num_banco = conteudo.Substring(76, 3);
                Nome_banco = conteudo.Substring(79, 15);
                Data_gravacao = conteudo.Substring(94, 6);
                Densidade_gravacao = conteudo.Substring(100, 8);
                Num_aviso_bancario = conteudo.Substring(108, 5);
                Branco1 = conteudo.Substring(113, 266);
                Data_credito = conteudo.Substring(379, 6);
                Branco2 = conteudo.Substring(385, 9);
                Seq_registro = conteudo.Substring(394, 6);
            }
        }
    }
}
