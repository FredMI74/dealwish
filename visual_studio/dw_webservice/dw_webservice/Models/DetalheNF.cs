using System;
using System.Collections.Generic;

namespace dw_webservice.Models
{
    public class DetalheNF
    {
        public string Conteudo { get; set; }

        public DetalheNF(IDictionary<string, object> fatura, string serie_rps, string cod_servico, string discriminacao)
        { 
            DateTime _hoje = DateTime.Today;
            Utils utils = new();

            string tipo_registro = "2";
            string tipo_rps = "RPS";
            string num_serie_rps = serie_rps.PadLeft(5, '0');
            string num_serie_NFe = " ".PadLeft(4, ' ');
            string num_rps = fatura["id"].ToString().PadLeft(10, '0');
            string data_rps = _hoje.ToString("yyyyMMdd");
            string hora_rps = _hoje.ToString("HHmmss");
            string situacao_rps = "E";
            string mtv_cancelamento = " ".PadLeft(2, ' ');
            string nf_cancelamento = " ".PadLeft(7, ' ');
            string serie_nf_cancelamento = " ".PadLeft(5, ' ');
            string data_emissao_nf_cancelada = " ".PadLeft(8, ' ');
            string desc_cancelamento = " ".PadLeft(180, ' ');
            string cod_servico_prestado = cod_servico.PadLeft(9, '0');
            string local_prestacao = "1";
            string prestado_via_publica = "2";
            string endereco_servico = " ".PadLeft(75, ' ');
            string num_logradouro_servico = " ".PadLeft(9, ' ');
            string complemento_endereco_servico = " ".PadLeft(30, ' ');
            string bairro_servico = " ".PadLeft(40, ' ');
            string cidade_servico = " ".PadLeft(40, ' ');
            string uf_servico = " ".PadLeft(2, ' ');
            string cep_servico = " ".PadLeft(8, ' ');
            string qtd_servico = "1";
            string valor_servico = double.Parse(fatura["valor"].ToString()).ToString("#.00").Replace(",", "").Replace(".", "").PadLeft(15, '0');
            string branco = " ".PadLeft(5, ' ');
            string valor_rentecao = "0".PadLeft(15, '0');
            string tomador_estrangeiro = "1";
            string pais_tomador = " ".PadLeft(3, ' ');
            string servico_exportado = "2";
            string tipo_pessoa_tomador = "2";
            string cnpj = utils.SomenteNumeros(fatura["cnpj"].ToString()).PadLeft(14, '0');
            string razao_social = fatura["razao_social"].ToString().PadRight(60, ' ').Substring(0, 60);
            string endereco_tomador = fatura["endereco"].ToString().PadRight(75, ' ').Substring(0, 75);
            string num_logradouro_tomador = fatura["numero"].ToString().PadRight(9, ' ').Substring(0, 9);
            string complemento_endereco_tomador = fatura["complemento"].ToString().PadRight(30, ' ').Substring(0, 30);
            string bairro_tomador = fatura["bairro"].ToString().PadRight(40, ' ').Substring(0, 40);
            string cidade_tomador = fatura["cidade"].ToString().PadRight(40, ' ').Substring(0, 40);
            string uf_tomador = fatura["uf"].ToString();
            string cep_tomador = fatura["cep"].ToString().PadLeft(8, '0').Substring(0, 8);
            string email_tomador = fatura["email_com"].ToString().PadRight(152,' ');
            string num_fatura = " ".PadLeft(6, ' ');
            string valor_fatura = " ".PadLeft(15, ' ');
            string forma_pg = " ".PadLeft(15, ' ');
            string discriminacao_servico = discriminacao.PadRight(1000, ' ');

            Conteudo = tipo_registro + tipo_rps + num_serie_rps + num_serie_NFe + num_rps + data_rps + hora_rps + situacao_rps + mtv_cancelamento + nf_cancelamento +
                       serie_nf_cancelamento + data_emissao_nf_cancelada + desc_cancelamento + cod_servico_prestado + local_prestacao + prestado_via_publica + endereco_servico +
                       num_logradouro_servico + complemento_endereco_servico + bairro_servico + cidade_servico + uf_servico + cep_servico + qtd_servico + valor_servico +
                       branco + valor_rentecao + tomador_estrangeiro + pais_tomador + servico_exportado + tipo_pessoa_tomador + cnpj + razao_social + endereco_tomador + num_logradouro_tomador +
                       complemento_endereco_tomador + bairro_tomador + cidade_tomador + uf_tomador + cep_tomador + email_tomador + num_fatura + valor_fatura + forma_pg + discriminacao_servico;
        }
    }
}