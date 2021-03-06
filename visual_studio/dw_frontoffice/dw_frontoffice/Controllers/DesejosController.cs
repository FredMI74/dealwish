using dw_frontoffice.Models;
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
using System.Net.Http.Headers;

namespace dw_frontoffice.Controllers
{
    [Authorize]
    public class DesejosController : Controller
    { 
        private static TpProduto tpproduto;
        private static Situacao situacao;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

         public IActionResult ConsultarDesejos(string responseStr = "", string responseStatusCode = "", int pagina = 0, int num_pag = 0, int max_id = 0)
        {

            Desejo _desejo = new Desejo();
            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                _desejo = JsonConvert.DeserializeObject<Desejo>(responseStr, settings);
                ViewBag.erro = (_desejo == null || _desejo.Resultado == null ? false : _desejo.Resultado.Erro);
                ViewBag.mensagem = (_desejo == null || _desejo.Resultado == null ? "" : _desejo.Resultado.Mensagem);
            }
            else
            {
                ViewBag.erro = false;
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            if (tpproduto == null)
            {
                tpproduto = new TpProduto();
                tpproduto = ConsultarTpprodutos();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.desejos = (_desejo == null || _desejo.Conteudo == null) ? new List<Desejo.Dados>() : _desejo.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            if (_desejo.InfoPagina != null)
            {
                max_id = _desejo.InfoPagina.Max_id;
                num_pag = (_desejo.InfoPagina.Count_id / 20) + (_desejo.InfoPagina.Count_id % 20 > 0 ? 1 : 0);
            }

            ViewBag.NumPag = num_pag;
            ViewBag.Pagina = pagina;
            ViewBag.MaxId = max_id;
            ViewBag.AntePag = pagina > 1 && _desejo.Conteudo != null;
            ViewBag.ProxPag = pagina != num_pag && _desejo.Conteudo != null;

            ViewBag.situacoes = situacao.Conteudo;
            ViewBag.tpprodutos = tpproduto.Conteudo;

            return View("ConsultarDesejos");
        }

        public Task<IActionResult> RefazerUltimaConsultaDesejo()
        {
           return ConsultarDesejosIn(true);
        }

        public Task<IActionResult> ExportarCSV()
        {
            return ConsultarDesejosIn(true, false, true);
        }

        public Task<IActionResult> ProxPaginaDesejo(int pagina, int num_pag, int max_id)
        {
            pagina++;
            if (pagina > num_pag)
            {
                pagina = num_pag;
            }
            return ConsultarDesejosIn(true, pagina: pagina, num_pag: num_pag, max_id: max_id);
        }

        public Task<IActionResult> AntePaginaDesejo(int pagina, int num_pag, int max_id)
        {
            pagina--;
            if (pagina <= 0)
            {
                pagina = 1;
            }
            return ConsultarDesejosIn(true, pagina: pagina, num_pag: num_pag, max_id: max_id);
        }


        [HttpPost]
        public async Task<IActionResult> ConsultarDesejosPost(int id, string descricao, int id_tipo_produto, string oferta, string uf, int id_cidade)
        {
            return await ConsultarDesejosIn(false, false, false, id, descricao, id_tipo_produto, oferta, uf, id_cidade);
        }

        private async Task<IActionResult> ConsultarDesejosIn(bool refazerultimaconsulta = false, bool editando = false, bool exportar = false, int id = 0, string descricao = "", 
                                                              int id_tipo_produto = 0, string oferta = "", string uf = "", int id_cidade = 0, 
                                                              int pagina = 1, int num_pag = 0, int max_id = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_desejo = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao},
                { "id_tipo_produto", id_tipo_produto.ToString() },
                { "id_situacao", Constantes.ATIVO.ToString()}, 
                { "oferta", oferta},
                { "id_empresa_oferta",  Request.Cookies[Constantes.ID_EMPRESA] },
                { "uf", uf},
                { "id_cidade", id_cidade.ToString()},
                { "paginacao", Constantes.SIM},
                { "max_id", "0"},
                { "pagina", "1"},
                { "exportar", Constantes.NAO},
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] },
                { "num_pag", num_pag.ToString()}
            };

            if (refazerultimaconsulta)
            {
                consulta_desejo = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
                if (num_pag != 0)
                {
                    consulta_desejo["pagina"] = pagina.ToString();
                    consulta_desejo["max_id"] = max_id.ToString();
                    consulta_desejo["num_pag"] = num_pag.ToString();
                }
                else
                {
                    pagina = Int32.Parse(consulta_desejo["pagina"]);
                    max_id = Int32.Parse(consulta_desejo["max_id"]);
                    num_pag = Int32.Parse(consulta_desejo["num_pag"]);
                }
                consulta_desejo["exportar"] = exportar ? Constantes.SIM : Constantes.NAO;
                consulta_desejo["paginacao"] = exportar ? Constantes.NAO : Constantes.SIM;
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_desejo);

            var response = httpClient.client.PostAsync("api/consultar_desejo", request.Content).Result;
            string responseStr = "";

            if (!exportar)
            {
                responseStr = await response.Content.ReadAsStringAsync();
            }

            if (!editando)
            {
                Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_desejo), HttpContext);

                if (response.IsSuccessStatusCode)
                {
                    if (!exportar)
                    {
                        return ConsultarDesejos(responseStr, response.StatusCode.ToString(), pagina, num_pag, max_id);
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
            else
            {
                return null;
            }

        }

        private Situacao ConsultarSituacoes()
        {

            string responseStr;

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_situacao = new Dictionary<string, string>
            {
                { "desejos", Constantes.SIM },
                { "origem", Constantes.FRONTOFFICE },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
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

        private TpProduto ConsultarTpprodutos()
        {

            string responseStr;

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_tpproduto = new Dictionary<string, string>
            {
                { "descricao", "%" },
                { "id_situacao", Constantes.ATIVO.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_tpproduto);

            var response = httpClient.client.PostAsync("api/consultar_tp_produto", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;

            if (responseStr != null)
            {
                return JsonConvert.DeserializeObject<TpProduto>(responseStr);
            }
            else
            {
                return null;
            }
        }
    }
}