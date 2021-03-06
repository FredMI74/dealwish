namespace dw_webservice.Models
{
    public class TransacaoRetornoBoleto
    {
        public string Id_registro { get; set; }
        public string Tipo_inscricao { get; set; }
        public string Num_inscricao { get; set; }
        public string Zeros1 { get; set; }
        public string Id_empresa_beneficiario { get; set; }
        public string Num_controle_participante { get; set; }
        public string Zeros2 { get; set; }
        public string id_titulo { get; set; }
        public string Zeros3 { get; set; }
        public string Zeros4 { get; set; }
        public string Indicador_rateio { get; set; }
        public string Pag_parcial { get; set; }
        public string Carteira { get; set; }
        public string Id_ocorrência { get; set; }
        public string Data_correncia { get; set; }
        public string Num_documento { get; set; }
        public string Id_titulo_banco { get; set; }
        public string Data_vct_titulo { get; set; }
        public string Valor_titulo { get; set; }
        public string Banco_cobrador { get; set; }
        public string Ag_cobradora { get; set; }
        public string Especie_titulo { get; set; }
        public string Despesascobrancao { get; set; }
        public string Outras_despesas { get; set; }
        public string Juros { get; set; }
        public string Iof { get; set; }
        public string Abatimento { get; set; }
        public string Desconto { get; set; }
        public string Valor_pago { get; set; }
        public string Juros_mora { get; set; }
        public string Outros_creditos { get; set; }
        public string Brancos1 { get; set; }
        public string Motivo_cod_ocorrencia { get; set; }
        public string Data_credito { get; set; }
        public string Origem_pagamento { get; set; }
        public string Brancos2 { get; set; }
        public string Cheque { get; set; }
        public string Motivos_rejeições { get; set; }
        public string Brancos3 { get; set; }
        public string Num_cartorio { get; set; }
        public string Num_protocolo { get; set; }
        public string Brancos4 { get; set; }
        public string Seq_registro { get; set; }
        public bool Valido { get; set; }

        public TransacaoRetornoBoleto(string num_banco, string conteudo)
        {
            Valido = conteudo.Length == 400;

            if (Valido)
            {
                if (num_banco == "237") //Bradesco
                {
                    Id_registro = conteudo.Substring(0, 1);
                    Tipo_inscricao = conteudo.Substring(1, 2);
                    Num_inscricao = conteudo.Substring(3, 14);
                    Zeros1 = conteudo.Substring(17, 3);
                    Id_empresa_beneficiario = conteudo.Substring(20, 17);
                    Num_controle_participante = conteudo.Substring(37, 25);
                    Zeros2 = conteudo.Substring(64, 8);
                    id_titulo = conteudo.Substring(70, 12).Trim();
                    Zeros3 = conteudo.Substring(82, 10);
                    Zeros4 = conteudo.Substring(92, 12);
                    Indicador_rateio = conteudo.Substring(104, 1);
                    Pag_parcial = conteudo.Substring(105, 2);
                    Carteira = conteudo.Substring(107, 1);
                    Id_ocorrência = conteudo.Substring(108, 2);
                    Data_correncia = conteudo.Substring(110, 6);
                    Num_documento = conteudo.Substring(116, 10);
                    Id_titulo_banco = conteudo.Substring(126, 20).Trim();
                    Data_vct_titulo = conteudo.Substring(146, 6);
                    Valor_titulo = conteudo.Substring(152, 13);
                    Banco_cobrador = conteudo.Substring(165, 3);
                    Ag_cobradora = conteudo.Substring(168, 5);
                    Especie_titulo = conteudo.Substring(173, 2);
                    Despesascobrancao = conteudo.Substring(175, 13);
                    Outras_despesas = conteudo.Substring(188, 13);
                    Juros = conteudo.Substring(201, 13);
                    Iof = conteudo.Substring(214, 13);
                    Abatimento = conteudo.Substring(227, 13);
                    Desconto = conteudo.Substring(240, 13);
                    Valor_pago = conteudo.Substring(253, 13);
                    Juros_mora = conteudo.Substring(266, 13);
                    Outros_creditos = conteudo.Substring(279, 13);
                    Brancos1 = conteudo.Substring(292, 2);
                    Motivo_cod_ocorrencia = conteudo.Substring(294, 1);
                    Data_credito = conteudo.Substring(295, 6);
                    Origem_pagamento = conteudo.Substring(301, 3);
                    Brancos2 = conteudo.Substring(304, 10);
                    Cheque = conteudo.Substring(314, 4);
                    Motivos_rejeições = conteudo.Substring(318, 10);
                    Brancos3 = conteudo.Substring(328, 40);
                    Num_cartorio = conteudo.Substring(368, 2);
                    Num_protocolo = conteudo.Substring(370, 10);
                    Brancos4 = conteudo.Substring(380, 14);
                    Seq_registro = conteudo.Substring(394, 6);
                }
            }

        }
    }
}

