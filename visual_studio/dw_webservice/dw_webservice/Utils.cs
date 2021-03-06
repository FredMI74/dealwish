using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using dw_webservice.Models;
using dw_webservice.Repositories;

namespace dw_webservice
{
    public class Utils
    {
        public static string getHash(string text)
        {
            // SHA512 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

        public static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        public static void EnviarEmail(string para = "", string assunto = "", string mensagem = "", IConfiguration Configuration = null)
        {
            try
            {
                //From Address  
                string FromAddress = Configuration.GetSection("Email").GetSection("Email").Value;
                string FromAdressTitle = "Dealwish";
                //To Address  
                string ToAddress = para;
                string ToAdressTitle = para;
                string Subject = assunto;
                string BodyContent = mensagem;

                //Smtp Server  
                string SmtpServer = Configuration.GetSection("Email").GetSection("Host").Value;
                //Smtp Port Number  
                int SmtpPortNumber = Int32.Parse(Configuration.GetSection("Email").GetSection("Port").Value);

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(FromAdressTitle, FromAddress));
                mimeMessage.To.Add(new MailboxAddress(ToAdressTitle, ToAddress));
                mimeMessage.Subject = Subject;
                mimeMessage.Body = new TextPart("plain")
                {
                    Text = BodyContent

                };

                using (var client = new SmtpClient())
                {

                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    var email = Configuration.GetSection("Email").GetSection("Email").Value;
                    var password = Configuration.GetSection("Email").GetSection("Password").Value;
                    client.Authenticate(email, password);
                    client.Send(mimeMessage);
                    client.Disconnect(true);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GeneratePassword(int length)
        {
            string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz0123456789";
            string pass = "";

            Random random = new Random();
            for (int f = 0; f < length; f++)
            {
                pass = pass + chars.Substring(random.Next(0, chars.Length - 1), 1);
            }

            return pass;
        }


        public static bool ValidarEmail(string email)
        {
            bool emailValido = false;

            //Expressão regular retirada de
            //https://msdn.microsoft.com/pt-br/library/01escwtf(v=vs.110).aspx
            string emailRegex = string.Format("{0}{1}",
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))",
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");

            try
            {
                emailValido = Regex.IsMatch(
                    email,
                    emailRegex);
            }
            catch (RegexMatchTimeoutException)
            {
                emailValido = false;
            }

            return emailValido;
        }

        public static string SomenteNumeros(string texto)
        {
            if (!string.IsNullOrWhiteSpace(texto))
            {
                const string caracteresPermitidos = "1234567890";
                StringBuilder builder = new StringBuilder(texto.Length);

                for (int i = 0; i < texto.Length; i++)
                {
                    if (caracteresPermitidos.Contains(texto[i]))
                    {
                        builder.Append(texto[i]);
                    }
                }
                return builder.ToString();
            }
            else
            {
                return "";
            }
        }


        public static object DapperQueryToCsvFile(IEnumerable<dynamic> dapperQueryResults, int id_usuario_login, string descricao)
        {
            string nome_arquivo = Path.Combine(@"temp", id_usuario_login.ToString() + "_" + descricao + ".csv");
            StreamWriter file = new StreamWriter(nome_arquivo);
            string linha = "";
            var dapperRows = dapperQueryResults.Cast<IDictionary<string, object>>().ToList();
            bool headerWritten = false;

            try
            {
                foreach (IDictionary<string, object> row in dapperRows)
                {
                    int i = 0;
                    if (!headerWritten)
                    {
                        foreach (KeyValuePair<string, object> item in row)
                        {
                            linha = linha + item.Key.ToString().Trim() + (++i != row.Count ? ";" : "");
                        }
                        headerWritten = true;
                        file.WriteLine(linha);
                    }
                    linha = "";
                    i = 0;
                    foreach (KeyValuePair<string, object> item in row)
                    {
                        var _valor = item.Value == null ? "" : item.Value.ToString().Trim().Replace(";", " ");
                        linha = linha + _valor + (++i != row.Count ? ";" : "");
                    }
                    file.WriteLine(linha);
                }

                return new { Nome_arquivo = nome_arquivo, Tipo_arquivo = "text/csv" };
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                file.Close();
            }
        }

        public static object DapperQueryToRemessaBoleto(IEnumerable<dynamic> dapperQueryResults)
        {
            int i = 1;
            var faturas = dapperQueryResults.Cast<IDictionary<string, object>>().ToList();

            string cod_empresa = ConfigRepository.Consultar_valor_config("codigo_empresa");
            string nome_empresa = ConfigRepository.Consultar_valor_config("nome_empresa");
            string numero_banco = ConfigRepository.Consultar_valor_config("numero_banco");
            string carteira = ConfigRepository.Consultar_valor_config("carteira");
            string agencia = ConfigRepository.Consultar_valor_config("agencia");
            string conta_corrente = ConfigRepository.Consultar_valor_config("conta_corrente");

            string seq_remessa = ConfigRepository.Consultar_valor_config("sequencia_remessa_boleto");
            ConfigRepository.Atualizar_valor_config("sequencia_remessa_boleto", (Int32.Parse(seq_remessa) + 1).ToString());

            string seq_dia = ConfigRepository.Consultar_valor_config("sequencia_dia_remessa_boleto");
            string ultimo_dia_geracao = ConfigRepository.Consultar_valor_config("ultimo_dia_geracao_remessa");
            string hoje = DateTime.Today.ToString("dd/MM/yyyy");
            if (ultimo_dia_geracao != hoje)
            {
                seq_dia = "1";
                ConfigRepository.Atualizar_valor_config("ultimo_dia_geracao_remessa", hoje);
            }
            ConfigRepository.Atualizar_valor_config("sequencia_dia_remessa_boleto", (Int32.Parse(seq_dia) + 1).ToString());

            HeaderBoleto headerboleto = new HeaderBoleto(numero_banco, cod_empresa, nome_empresa, seq_remessa, seq_dia);

            string nome_arquivo = Path.Combine(@"temp", headerboleto.Nome_arquivo);
            StreamWriter file = new StreamWriter(nome_arquivo);

            try
            {
                file.WriteLine(headerboleto.Conteudo);

                foreach (IDictionary<string, object> fatura in faturas)
                {
                    i++;
                    TransacaoBoleto transacaoboleto = new TransacaoBoleto(numero_banco, carteira, agencia, conta_corrente, fatura, i);
                    file.WriteLine(transacaoboleto.Conteudo);
                }

                i++;
                TraillerBoleto traillerboleto = new TraillerBoleto(numero_banco, i);
                file.WriteLine(traillerboleto.Conteudo);

                return new { Nome_arquivo = nome_arquivo, Tipo_arquivo = "text/plain" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                file.Close();
            }
        }

        public static object ArquivoErro(int id_usuario_login, string nome, string mensagem)
        {
            string nome_arquivo = Path.Combine(@"temp", id_usuario_login.ToString() + "_" + nome + ".txt");
            StreamWriter file = new StreamWriter(nome_arquivo);
            try
            {
                file.WriteLine(mensagem);
                return new { Nome_arquivo = nome_arquivo, Tipo_arquivo = "text/plain" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                file.Close();
            }
       
        }

        public static object DapperQueryToRemessaNF(IEnumerable<dynamic> dapperQueryResults)
        {
            int i = 1;
            var faturas = dapperQueryResults.Cast<IDictionary<string, object>>().ToList();

            double total_servidos = 0;
            double total_impostos = 0;

            string inscricao_contribuinte = ConfigRepository.Consultar_valor_config("inscricao_contribuinte");
            string serie_rps = ConfigRepository.Consultar_valor_config("serie_rps");
            string cod_servico_prestado = ConfigRepository.Consultar_valor_config("cod_servico_prestado");
            string discriminacao_servico = ConfigRepository.Consultar_valor_config("discriminacao_servico");

            string seq_remessa = ConfigRepository.Consultar_valor_config("sequencia_remessa_nf");
            ConfigRepository.Atualizar_valor_config("sequencia_remessa_nf", (Int32.Parse(seq_remessa) + 1).ToString());

            HeaderNF headernf = new HeaderNF(inscricao_contribuinte, seq_remessa);

            string nome_arquivo = Path.Combine(@"temp", headernf.Nome_arquivo);
            StreamWriter file = new StreamWriter(nome_arquivo);

            try
            {
                file.WriteLine(headernf.Conteudo);

                foreach (IDictionary<string, object> fatura in faturas)
                {
                    i++;
                    DetalheNF detalhenf = new DetalheNF(fatura, serie_rps, cod_servico_prestado, discriminacao_servico);
                    file.WriteLine(detalhenf.Conteudo);
                    total_servidos = total_servidos + double.Parse(fatura["valor"].ToString());
                }

                i++;
                TraillerNF traillernf = new TraillerNF(i, total_servidos, total_impostos);
                file.WriteLine(traillernf.Conteudo);

                return new { Nome_arquivo = nome_arquivo, Tipo_arquivo = "text/plain" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                file.Close();
            }
        }

        public static void Listtofile(List<string> lista, string nome_arquivo)
        {
            nome_arquivo = Path.Combine(@"temp", nome_arquivo);
            StreamWriter file = new StreamWriter(nome_arquivo);

            foreach (string linha in lista)
            {
                file.WriteLine(linha);
            }
            file.Close();
        }

        public static bool isEmpty(IEnumerable<dynamic> queryResults)
        {
            if (queryResults != null && queryResults.GetEnumerator().MoveNext())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string formatarDataZero(string data)
        {
            return data == "1/1/0001 12:00:00 AM" || data == "01 /01/0001 00:00:00" || data == "01/01/01 00:00:00" || data == "01/01/0001" || data == "01/01/01" ? null : data;
        }


        public static void Atualizar_valor_config(string chave, string valor)
        {
            ConfigRepository.Atualizar_valor_config(chave, valor);
        }

        public static string Consultar_valor_config(string chave)
        {
            return ConfigRepository.Consultar_valor_config(chave);
        }

        public static object FormataRetorno(object conteudo, object retorno)
        {
            BaseRetorno _retorno = new BaseRetorno
            {
                Conteudo = conteudo,
                Resultado = retorno
            };
           return _retorno;
        }

        public static object FormataRetorno(object conteudo, object retorno, object infopagina)
        {
            BaseRetornoPagina _retorno = new BaseRetornoPagina
            {
                Conteudo = conteudo,
                Resultado = retorno,
                InfoPagina = infopagina
            };
            return _retorno;
        }

        public static class CRC16
        {

           private static readonly byte[] data = { 0x54, 0xA1, 0x4A, 0x03, 0xE7, 0x03, 0xD4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
           static private ushort wCRC = 0;

            public static String ComputeChecksum(byte[] data)
            {
                for (int i = 1; i < data.Length; i++)
                {
                    wCRC = (ushort)(wCRC ^ (data[i] << 8));

                    for (int j = 0; j < 8; j++)
                    {
                        if ((wCRC & 0x8000) != 0)
                            wCRC = (ushort)((wCRC << 1) ^ 0x1021);
                        else
                            wCRC <<= 1;
                    }
                }

                return wCRC.ToString("X");
            }
        }

    }  
}  

