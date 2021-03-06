using dw_backoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;

namespace dw_backoffice.Controllers
{
    [Authorize]
    public class FaturasController : Controller
    {
        private Situacao situacao;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ProcessarRetornoBoleto(string responseStr, string responseStatusCode)
        {
            ProcessarRegistro _processado = new ProcessarRegistro();
            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _processado = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
            }
            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.linhasafetadas = (_processado == null || _processado == null || _processado.Conteudo == null ? 0 : _processado.Conteudo.Linhasafetadas);
            ViewBag.erro = (_processado == null || _processado.Resultado == null ? false : _processado.Resultado.Erro);
            ViewBag.mensagem = (_processado == null || _processado.Resultado == null ? "" : _processado.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            return View("ProcessarRetornoBoleto");
        }


        public IActionResult ProcessarRetornoNF(string responseStr, string responseStatusCode)
        {

            ProcessarRegistro _processado = new ProcessarRegistro();
            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _processado = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
            }
            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.linhasafetadas = (_processado == null || _processado == null || _processado.Conteudo == null ? 0 : _processado.Conteudo.Linhasafetadas);
            ViewBag.erro = (_processado == null || _processado.Resultado == null ? false : _processado.Resultado.Erro);
            ViewBag.mensagem = (_processado == null || _processado.Resultado == null ? "" : _processado.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            return View("ProcessarRetornoNF");
        }

        public IActionResult BaixarFaturas(int id, string responseStr, string responseStatusCode)
        {
            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            Fatura _fatura = new Fatura();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                if (id == 0)
                {
                    _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    _fatura = JsonConvert.DeserializeObject<Fatura>(responseStr, settings);
                }
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.razao_social = (_fatura == null || _fatura.Conteudo == null) ? "" : _fatura.Conteudo[0].Razao_social;
            ViewBag.valor = (_fatura == null || _fatura.Conteudo == null) ? 0 : _fatura.Conteudo[0].Valor;
            ViewBag.data_vct = (_fatura == null || _fatura.Conteudo == null) ? "" : _fatura.Conteudo[0].Data_vct.ToString("yyyy-MM-dd");
            ViewBag.data_pg = "";
            ViewBag.multa = 0;
            ViewBag.juros = 0;
            ViewBag.valor_pg = 0;

            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            return View("BaixarFaturas");
        }


        public IActionResult ConsultarFaturas(string responseStr, string responseStrExc, string responseStatusCode)
        {
            Fatura _fatura = new Fatura();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                _fatura = JsonConvert.DeserializeObject<Fatura>(responseStr, settings);
                ViewBag.erro = (_fatura == null || _fatura.Resultado == null ? false : _fatura.Resultado.Erro);
                ViewBag.mensagem = (_fatura == null || _fatura.Resultado == null ? "" : _fatura.Resultado.Mensagem);
                ViewBag.excluindo = false;
            }

            if (!string.IsNullOrWhiteSpace(responseStrExc))
            {
                ProcessarRegistro _proc = new ProcessarRegistro();
                _proc = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStrExc);
                ViewBag.erro = (_proc == null || _proc.Resultado == null ? false : _proc.Resultado.Erro);
                ViewBag.mensagem = (_proc == null || _proc.Resultado == null ? "" : _proc.Resultado.Mensagem);
                ViewBag.excluindo = true;
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.faturas = (_fatura == null || _fatura.Conteudo == null) ? new List<Fatura.Dados>() : _fatura.Conteudo;
            ViewBag.situacoes = situacao.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarFaturas");
        }

        public IActionResult ConsultarFaturasAbertas(string responseStr, string responseStrExc, string responseStatusCode)
        {
            Fatura _fatura = new Fatura();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                    
                };
                _fatura = JsonConvert.DeserializeObject<Fatura>(responseStr, settings);
                ViewBag.erro = (_fatura == null || _fatura.Resultado == null ? false : _fatura.Resultado.Erro);
                ViewBag.mensagem = (_fatura == null || _fatura.Resultado == null ? "" : _fatura.Resultado.Mensagem);
                ViewBag.efetivando = false;
            }

            if (!string.IsNullOrWhiteSpace(responseStrExc))
            {
                ProcessarRegistro _proc = new ProcessarRegistro();
                _proc = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStrExc);
                ViewBag.erro = (_proc == null || _proc.Resultado == null ? false : _proc.Resultado.Erro);
                ViewBag.mensagem = (_proc == null || _proc.Resultado == null ? "" : _proc.Resultado.Mensagem);
                ViewBag.efetivando = true;
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.faturas = (_fatura == null || _fatura.Conteudo == null) ? new List<Fatura.Dados>() : _fatura.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarFaturasAbertas");
        }

        public Task<IActionResult> RefazerUltimaConsultaFatura()
        {
           return ConsultarFaturasIn(true);
        }

        public Task<IActionResult> ExportarCSV()
        {
            return ConsultarFaturasIn(true, Constantes.CSV);
        }

        public Task<IActionResult> GerarRemessaBoleto()
        {
            return ConsultarFaturasIn(true, Constantes.BOLETO);
        }
        public Task<IActionResult> GerarPIX()
        {
            return ConsultarFaturasIn(true, Constantes.PIX);
        }

        public Task<IActionResult> GerarRemessaNF()
        {
            return ConsultarFaturasIn(true, Constantes.NOTA_FISCAL);
        }

        public Task<IActionResult> AcaoBaixarFaturas(int id)
        {
            return ConsultarFaturasIn(false, Constantes.NAO, id, baixando: true);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarFaturasPost( int id, int id_empresa, string razao_social, int mes, int ano, string nosso_numero, int id_situacao)
        {
            return await ConsultarFaturasIn(false, Constantes.NAO, id, id_empresa, razao_social, mes, ano, nosso_numero, id_situacao);
        }

        private async Task<IActionResult> ConsultarFaturasIn(bool refazerultimaconsulta = false, string exportar = Constantes.NAO, int id = 0, int id_empresa = 0, string razao_social = "", int mes = 0, int ano = 0,  string nosso_numero = "",  int id_situacao = 0, bool baixando = false)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_fatura = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "id_empresa", id_empresa.ToString() },
                { "razao_social", razao_social },
                { "mes", mes.ToString() },
                { "ano", ano.ToString() },
                { "nosso_numero", nosso_numero },
                { "id_situacao", id_situacao.ToString() },
                { "exportar", Constantes.NAO},
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            
            if (refazerultimaconsulta)
            {
                consulta_fatura = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
                consulta_fatura["exportar"] = exportar;
            }

            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_fatura);

            var response = httpClient.client.PostAsync("api/consultar_fatura", request.Content).Result;
            string responseStr = "";

            if (exportar == Constantes.NAO || exportar == Constantes.PIX)
            {
                responseStr = await response.Content.ReadAsStringAsync();
            }

            if (response.IsSuccessStatusCode)
            {
                 if (exportar == Constantes.NAO || exportar == Constantes.PIX)
                 {
                    if (!baixando)
                    {
                        Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_fatura), HttpContext);
                        if (exportar == Constantes.PIX)
                        {
                            return ConsultarFaturas("", responseStr, response.StatusCode.ToString());

                        }
                        else
                        {
                            return ConsultarFaturas(responseStr, "", response.StatusCode.ToString());
                        }
                    }
                    else
                    {
                        return BaixarFaturas(id, responseStr, response.StatusCode.ToString());
                    }
                 }
                 else
                 {
                     var responseStream = await response.Content.ReadAsStreamAsync();
                     return new FileStreamResult(responseStream, response.Content.Headers.ContentType.MediaType) { FileDownloadName = response.Content.Headers.ContentDisposition.FileName };
                 }
           }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }
        }

        public Task<IActionResult> AtualizarFaturasAbertas()
        {
            return ConsultarFaturasAbertasIn(false);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarFaturasAbertasPost()
        {
            return await ConsultarFaturasAbertasIn(false);
        }

        private async Task<IActionResult> ConsultarFaturasAbertasIn(bool refazerultimaconsulta = false)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_fatura = new Dictionary<string, string>
            {
                {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };


            if (refazerultimaconsulta)
            {
                consulta_fatura = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_fatura);

            var response = httpClient.client.PostAsync("api/consultar_faturas_abertas", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_fatura), HttpContext);
                return ConsultarFaturasAbertas(responseStr, "", response.StatusCode.ToString());
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }

            }

        }



        public async Task<IActionResult> ExcluirFaturas(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_fatura = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_fatura);

            var response = httpClient.client.PostAsync("api/excluir_fatura", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarFaturas("", responseStrExc, response.StatusCode.ToString());
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }
        }


        public async Task<IActionResult> CancelarFaturas(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_fatura = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              { "id_situacao", Constantes.CANCELADA.ToString() }, 
              {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_fatura);

            var response = httpClient.client.PostAsync("api/atualizar_fatura", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarFaturas("", responseStrExc, response.StatusCode.ToString());
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }
        }

        public async Task<IActionResult> EfetivarFaturasAbertas()
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var efetivar_fatura = new Dictionary<string, string>
          {
              {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(efetivar_fatura);

            var response = httpClient.client.PostAsync("api/efetivar_faturas_abertas", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarFaturasAbertas("", responseStrExc, response.StatusCode.ToString());
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> BaixarFaturasPost(int id, DateTime data_pg, Double multa, Double juros, Double valor_pg)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var edt_fatura = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "data_pg", data_pg.ToString() },
                { "multa", multa.ToString() },
                { "juros", juros.ToString() },
                { "valor_pg", valor_pg.ToString() },
                { "id_situacao", Constantes.LIQUIDADA.ToString() }, 
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(edt_fatura);

            var response = httpClient.client.PostAsync("api/atualizar_fatura", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
        
            if (response.IsSuccessStatusCode)
            {
                return BaixarFaturas(0, responseStr, response.StatusCode.ToString());
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }

        }

        [HttpPost]
        public async Task<IActionResult> ProcessarRetornoBoletoPost(IFormFile file)
        {
            if (file == null)
            {
                return RedirectToAction(nameof(ProcessarRetornoBoleto));
            }

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            byte[] data;
            using (var br = new BinaryReader(file.OpenReadStream()))
                data = br.ReadBytes((int)file.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(bytes, "file", file.FileName);
            multiContent.Add(new StringContent(Request.Cookies[Constantes.TOKEN_USUARIO]), "token");

            var response = httpClient.client.PostAsync("api/processar_retorno_boleto", multiContent).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ProcessarRetornoBoleto(responseStr, response.StatusCode.ToString());
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }

        }

        [HttpPost]
        public async Task<IActionResult> ProcessarRetornoNFPost(IFormFile file)
        {
            if (file == null)
            {
                return RedirectToAction(nameof(ProcessarRetornoNF));
            }

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            byte[] data;
            using (var br = new BinaryReader(file.OpenReadStream()))
                data = br.ReadBytes((int)file.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(bytes, "file", file.FileName);
            multiContent.Add(new StringContent(Request.Cookies[Constantes.TOKEN_USUARIO]), "token");

            var response = httpClient.client.PostAsync("api/processar_retorno_nf", multiContent).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ProcessarRetornoNF(responseStr, response.StatusCode.ToString());
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }

        }

        public async Task<IActionResult> ProcessarRetornoBoletoCSV()
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var retorno_csv = new Dictionary<string, string>
            {
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(retorno_csv);

            var response = httpClient.client.PostAsync("api/retorno_processamento_boleto", request.Content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                return new FileStreamResult(responseStream, response.Content.Headers.ContentType.MediaType) { FileDownloadName = response.Content.Headers.ContentDisposition.FileName };
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }
        }

        public async Task<IActionResult> ProcessarRetornoNFCSV()
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var retorno_csv = new Dictionary<string, string>
            {
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(retorno_csv);

            var response = httpClient.client.PostAsync("api/retorno_processamento_nf", request.Content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                return new FileStreamResult(responseStream, response.Content.Headers.ContentType.MediaType) { FileDownloadName = response.Content.Headers.ContentDisposition.FileName };
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }
        }


        private Situacao ConsultarSituacoes()
        {

            string responseStr;
            string responseStatusCode;

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_situacao = new Dictionary<string, string>
            {
                { "faturas", Constantes.SIM },
                { "origem", Constantes.BACKOFFICE },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_situacao);

            var response = httpClient.client.PostAsync("api/consultar_situacao", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;
            responseStatusCode = response.StatusCode.ToString();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                return JsonConvert.DeserializeObject<Situacao>(responseStr);
            }
            else
            {
                return null;
            }
        }

    }
}