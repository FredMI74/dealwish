using dw_frontoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;

namespace dw_frontoffice.Controllers
{
    [Authorize]
    public class OfertasController : Controller
    {
        private Situacao situacao;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult IncluirOfertasLote(string responseStr = "", string responseStatusCode = "")
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

            return View("IncluirOfertasLote");
        }

        public IActionResult IncluirOfertas(int id_desejo, string desejo, string responseStr = "", string responseStatusCode = "", string nova_oferta = "")
        {

            NovoRegistro _novaoferta = new NovoRegistro();
            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _novaoferta = JsonConvert.DeserializeObject<NovoRegistro>(responseStr);
            }
            ViewBag.Desejo = desejo;
            ViewBag.id_desejo = id_desejo;
            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.descricao = "";
            ViewBag.destaque = Constantes.NAO;
            ViewBag.id = (_novaoferta == null || _novaoferta== null || _novaoferta.Conteudo == null ? 0 : _novaoferta.Conteudo.Id);
            ViewBag.erro = (_novaoferta == null || _novaoferta.Resultado == null ? false : _novaoferta.Resultado.Erro);
            ViewBag.mensagem = (_novaoferta == null || _novaoferta.Resultado  == null ? "" : _novaoferta.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary <string, string> inclusao = JsonConvert.DeserializeObject<Dictionary<string, string>>(nova_oferta);
                ViewBag.id_empresa = inclusao["id_empresa"];
                ViewBag.id_desejo = inclusao["id_desejo"];
                ViewBag.valor = inclusao["valor"];
                ViewBag.url = inclusao["url"];
                ViewBag.descricao = inclusao["descricao"];
                ViewBag.destaque = inclusao["destaque"];
                ViewBag.validade = DateTime.Parse(inclusao["validade"]).ToString("yyyy-MM-dd");
            }

            return View("IncluirOfertas");
        }

        [HttpPost]
        public async Task<IActionResult> IncluirOfertasPost(int id_desejo, string descricao_desejo, string descricao, string url, DateTime validade, float valor, string destaque)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var nova_oferta = new Dictionary<string, string>
            {
                { "id_desejo", id_desejo.ToString()},
                { "descricao", descricao},
                { "url", url},
                { "validade", validade.ToString()},
                { "valor", valor.ToString()},
                { "destaque",  destaque ?? Constantes.NAO},
                { "id_empresa", Request.Cookies[Constantes.ID_EMPRESA]},
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(nova_oferta);

            var response = httpClient.client.PostAsync("api/incluir_oferta", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return IncluirOfertas(id_desejo, descricao_desejo, responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(nova_oferta));
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
        public async Task<IActionResult> IncluirOfertasLotePost(IFormFile file)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            if (file == null)
            {
                return RedirectToAction(nameof(IncluirOfertasLote));
            }

            byte[] data;
              using (var br = new BinaryReader(file.OpenReadStream()))
                data = br.ReadBytes((int)file.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(bytes, "file", file.FileName);
            multiContent.Add(new StringContent(Request.Cookies[Constantes.ID_EMPRESA]), "id_empresa");
            multiContent.Add(new StringContent(Request.Cookies[Constantes.TOKEN_USUARIO]), "token");

            var response = httpClient.client.PostAsync("api/incluir_oferta_lote", multiContent).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return IncluirOfertasLote(responseStr, response.StatusCode.ToString());
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

        public async Task<IActionResult> BaixarRetornoCSV()
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var retorno_csv = new Dictionary<string, string>
            {
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO]}
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(retorno_csv);

            var response = httpClient.client.PostAsync("api/retorno_oferta_lote", request.Content).Result;

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

        public IActionResult ConsultarOfertas(string tipo_consulta, string responseStr = "", string responseStatusCode = "", string responseStrExc = "", 
                                             int pagina = 0, int num_pag = 0, int max_id = 0)
        {

            if (!string.IsNullOrWhiteSpace(tipo_consulta))
            {
                ViewBag.tipo_consulta = tipo_consulta;
            }

            Oferta _oferta = new Oferta();
            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                _oferta = JsonConvert.DeserializeObject<Oferta>(responseStr, settings);
                ViewBag.erro = (_oferta == null || _oferta.Resultado == null ? false : _oferta.Resultado.Erro);
                ViewBag.mensagem = (_oferta == null || _oferta.Resultado == null ? "" : _oferta.Resultado.Mensagem);
                ViewBag.atualizando = false;
            }

            if (!string.IsNullOrWhiteSpace(responseStrExc))
            {
                ProcessarRegistro _proc = new ProcessarRegistro();
                _proc = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStrExc);
                ViewBag.erro = (_proc == null || _proc.Resultado == null ? false : _proc.Resultado.Erro);
                ViewBag.mensagem = (_proc == null || _proc.Resultado == null ? "" : _proc.Resultado.Mensagem);
                ViewBag.atualizando = true;
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.Id_empresa_logada = int.Parse(Request.Cookies[Constantes.ID_EMPRESA]);
            ViewBag.ofertas = (_oferta == null || _oferta.Conteudo == null) ? new List<Oferta.Dados>() : _oferta.Conteudo;
            ViewBag.situacoes = situacao.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            if (_oferta.InfoPagina != null)
            {
                max_id = _oferta.InfoPagina.Max_id;
                num_pag = (_oferta.InfoPagina.Count_id / 20) + (_oferta.InfoPagina.Count_id % 20 > 0 ? 1 : 0);

            }

            ViewBag.NumPag = num_pag;
            ViewBag.Pagina = pagina;
            ViewBag.MaxId = max_id;
            ViewBag.AntePag = pagina > 1 && _oferta.Conteudo != null;
            ViewBag.ProxPag = pagina != num_pag && _oferta.Conteudo != null;

            return View("ConsultarOfertas");
        }

        public Task<IActionResult> RefazerUltimaConsultaOferta()
        {
           return ConsultarOfertasIn(true);
        }

        public Task<IActionResult> ProxPaginaOferta(string tipo_consulta, int pagina, int num_pag, int max_id)
        {
            pagina++;
            if (pagina > num_pag)
            {
                pagina = num_pag;
            }
            return ConsultarOfertasIn(refazerultimaconsulta: true, tipo_consulta: tipo_consulta, pagina: pagina, num_pag: num_pag, max_id: max_id);
        }

        public Task<IActionResult> AntePaginaOferta(string tipo_consulta, int pagina, int num_pag, int max_id)
        {
            pagina--;
            if (pagina <= 0)
            {
                pagina = 1;
            }
            return ConsultarOfertasIn(refazerultimaconsulta: true, tipo_consulta: tipo_consulta, pagina: pagina, num_pag: num_pag, max_id: max_id);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarOfertasPost(int id_desejo, int id_fatura, int id_situacao, string descricao, string data_ini, string data_fim, string tipo_consulta)
        {
            return await ConsultarOfertasIn(false, id_desejo, id_fatura, id_situacao, descricao, tipo_consulta, data_ini, data_fim);
        }
        public Task<IActionResult> AcaoConsultarOfertas(int id_desejo, string tipo_consulta)
        {
            return ConsultarOfertasIn(false, id_desejo: id_desejo, tipo_consulta: tipo_consulta);
        }


        private async Task<IActionResult> ConsultarOfertasIn(bool refazerultimaconsulta = false, int id_desejo = 0, int id_fatura = 0, int id_situacao = 0, string descricao = "", 
                                                             string tipo_consulta = "", string data_ini = "", string data_fim = "", int pagina = 1, int num_pag = 0, int max_id = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_oferta = new Dictionary<string, string>
            {
                { "id_desejo", id_desejo.ToString() },
                { "id_fatura", id_fatura.ToString() },
                { "id_situacao", id_situacao.ToString() },
                { "descricao", descricao },
                { "origem", Constantes.FRONTOFFICE },
                { "id_empresa", Request.Cookies[Constantes.ID_EMPRESA] },
                { "data_ini", data_ini},
                { "data_fim", data_fim},
                { "paginacao", Constantes.SIM},
                { "max_id", "0"},
                { "pagina", "1"},
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO]}          
            };

            if (refazerultimaconsulta)
            {
                consulta_oferta = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA_AUX]);
                consulta_oferta["pagina"] = pagina.ToString();
                consulta_oferta["max_id"] = max_id.ToString();
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_oferta);

            var response = httpClient.client.PostAsync("api/consultar_oferta", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            Utils.SetCookie(Constantes.ULTIMA_CONSULTA_AUX, JsonConvert.SerializeObject(consulta_oferta), HttpContext);
           
            if (response.IsSuccessStatusCode)
            {
                    return ConsultarOfertas(tipo_consulta, responseStr, response.StatusCode.ToString(), "", pagina, num_pag, max_id);
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

        public async Task<IActionResult> AtualizarSituacaoOferta(int id, int id_situacao, string tipo_consulta)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_oferta = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "id_situacao", id_situacao.ToString() },
                { "id_empresa", Request.Cookies[Constantes.ID_EMPRESA] },
                {"token", Request.Cookies[Constantes.TOKEN_USUARIO]}
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_oferta);

            var response = httpClient.client.PostAsync("api/atualizar_situacao_oferta", request.Content).Result;
            string responseStrExc = await response.Content.ReadAsStringAsync();
            string responseStatusCode = response.StatusCode.ToString();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarOfertas(tipo_consulta, "", responseStatusCode, responseStrExc);
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

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_situacao = new Dictionary<string, string>
            {
                { "ofertas", Constantes.SIM },
                { "origem", Constantes.FRONTOFFICE },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO]}
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_situacao);

            var response = httpClient.client.PostAsync("api/consultar_situacao", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;

            if (responseStr != null)
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