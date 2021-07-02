using System;
using System.Collections.Generic;

namespace dw_webservice.Models
{
    public class TransacaoBoleto
    {
        public static string Digito_verificador(string sequencia)
        {
            int soma = 0;
            int multiplicador = 2;

            for (int i = 0; i < sequencia.Length; i++)
            {
                soma += (int)char.GetNumericValue(sequencia[i]) * multiplicador;
                multiplicador--;
                if (multiplicador == 1)
                {
                    multiplicador = 7;
                }
            }
            string dv = (11 - (soma % 11)).ToString();
            if (dv == "10")
            {
                dv = "P";
            } else
            {
                if (dv == "11")
                {
                    dv = "0";
                }
            }
            return dv;
        }

        public string Conteudo { get; set; }

        public TransacaoBoleto(string banco, string carteira, string agencia, string conta_corrente, IDictionary<string, object> fatura, int seq)
        {
            if (banco == "237") //Bradesco
            {
                DateTime _hoje = DateTime.Today;
                Utils utils = new();

                string id_registro = "1";
                string ag_debito_pag = " ".PadLeft(5, ' ');
                string dig_ag_debito_pag = " ";
                string razao_conta_corrente_pag = " ".PadLeft(5, ' ');
                string conta_conrrente_pag = " ".PadLeft(7, ' ');
                string dig_conta_corrente_pag = " ";
                string id_beneficiario = "0" + carteira.PadLeft(3, '0') + agencia.PadLeft(5, '0') + conta_corrente.PadLeft(8, '0');
                string num_controle_participante = fatura["id"].ToString().PadLeft(25, '0');
                string cod_banco_debito_automatico = "000";
                string multa = "0";
                string percentual_multa = "0".PadLeft(4, '0');
                string id_titulo_banco = fatura["nosso_numero"].ToString().PadLeft(11, '0');
                string dig_id_titulo_banco = Digito_verificador(carteira + id_titulo_banco);
                string desc_bonif_dia = "0".PadLeft(10, '0');
                string condicao_papeleta = "1";
                string id_emite_boleto_debito_auto = " ";
                string id_oper_banco = " ".PadLeft(10, ' ');
                string indicador_rateio = " ";
                string end_aviso_deb_automatico = " ";
                string qtd_possi_pg = "  ";
                string id_ocorrencia = "01";
                string num_doc = "0".PadLeft(10, '0');
                string data_vct = DateTime.Parse(fatura["data_vct"].ToString()).ToString("ddMMyy");
                string valor_titulo = double.Parse(fatura["valor"].ToString()).ToString("#.00").Replace(",", "").Replace(".", "").PadLeft(13, '0');
                string banco_cobranca = "000";
                string ag_depositaria = "00000";
                string especie_titulo = "01";
                string identificacao = "N";
                string data_emissao_titulo = _hoje.ToString("ddMMyy");
                string instrucao_1 = "00";
                string instrucao_2 = "00";
                string mora_por_dia = "0".PadLeft(13, '0');
                string data_limite_conc_desconto = _hoje.ToString("ddMMyy");
                string valor_desconto = "0".PadLeft(13, '0');
                string valor_iof = "0".PadLeft(13, '0');
                string valor_abatimento = "0".PadLeft(13, '0');
                string tipo_pessoa = "02";
                string cpnj = utils.SomenteNumeros(fatura["cnpj"].ToString()).PadLeft(14, '0');
                string nome_pagador = fatura["razao_social"].ToString().PadRight(40, ' ');
                string endereco = (fatura["endereco_cob"].ToString() + ", " + fatura["numero_cob"].ToString() + " - " + fatura["cidade_cob"].ToString()).PadRight(40, ' ').Substring(0, 40);
                string mensagem_1 = " ".PadLeft(12, ' ');
                string cep = utils.SomenteNumeros(fatura["cep_cob"].ToString()).PadLeft(8, '0'); 
                string mensagem_2 = " ".PadLeft(60, ' ');
                string seq_registro = seq.ToString().PadLeft(6, '0');

                Conteudo = id_registro + ag_debito_pag + dig_ag_debito_pag + razao_conta_corrente_pag + conta_conrrente_pag + dig_conta_corrente_pag + id_beneficiario +
                            num_controle_participante + cod_banco_debito_automatico + multa + percentual_multa + id_titulo_banco + dig_id_titulo_banco + desc_bonif_dia + condicao_papeleta + id_emite_boleto_debito_auto +
                            id_oper_banco + indicador_rateio + end_aviso_deb_automatico + qtd_possi_pg + id_ocorrencia + num_doc + data_vct + valor_titulo + banco_cobranca + ag_depositaria +
                            especie_titulo + identificacao + data_emissao_titulo + instrucao_1 + instrucao_2 + mora_por_dia + data_limite_conc_desconto + valor_desconto + valor_iof +
                            valor_abatimento + tipo_pessoa + cpnj + nome_pagador + endereco + mensagem_1 + cep + mensagem_2 + seq_registro;
            }


          
        }
    }
}
