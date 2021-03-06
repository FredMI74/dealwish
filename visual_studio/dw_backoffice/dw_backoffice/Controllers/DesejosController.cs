using dw_backoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http.Headers;

namespace dw_backoffice.Controllers
{
    [Authorize]
    public class DesejosController : Controller
    {
        private TpProduto tpproduto;
        private Situacao situacao;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

         public IActionResult ConsultarDesejos(string responseStr, string responseStrExc, string responseStatusCode, int pagina, int num_pag, int max_id)
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

        public Task<IActionResult> ProxPaginaDesejo(int pagina, int num_pag, int max_id)
        {
            pagina += 1;
            if (pagina > num_pag)
            {
                pagina = num_pag;
            }
            
            return ConsultarDesejosIn(true, pagina: pagina, num_pag: num_pag, max_id: max_id);
        }

        public Task<IActionResult> AntePaginaDesejo(int pagina, int num_pag, int max_id)
        {
            pagina -= 1;
            if (pagina <= 0)
            {
                pagina = 1;
            }
            return ConsultarDesejosIn(true, pagina: pagina, num_pag: num_pag,max_id: max_id);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarDesejosPost( int id, string descricao, int id_usuario, int id_tipo_produto, int id_situacao, string oferta, string uf, int id_cidade)
        {
            return await ConsultarDesejosIn(false, id, descricao, id_usuario, id_tipo_produto, id_situacao, oferta, uf, id_cidade);
        }

        private async Task<IActionResult> ConsultarDesejosIn(bool refazerultimaconsulta = false, int id = 0, string descricao = "",
                                                             int id_usuario = 0, int id_tipo_produto = 0, int id_situacao = 0, string oferta = "", string uf = "", int id_cidade = 0,
                                                             int pagina = 1, int num_pag = 0, int max_id = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_desejo = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao},
                { "id_usuario", id_usuario.ToString() },
                { "id_tipo_produto", id_tipo_produto.ToString() },
                { "id_situacao", id_situacao.ToString() },
                { "oferta", oferta},
                { "uf", uf},
                { "id_empresa_oferta", Constantes.ID_DEALWISH.ToString()},
                { "id_cidade", id_cidade.ToString()},
                { "paginacao", Constantes.SIM},
                { "max_id", "0"},
                { "pagina", "1"},
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
            }
        
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_desejo);

            var response = httpClient.client.PostAsync("api/consultar_desejo", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
           
            if (response.IsSuccessStatusCode)
            {
                Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_desejo), HttpContext);
                return ConsultarDesejos(responseStr, "", response.StatusCode.ToString(), pagina, num_pag, max_id);
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

        public async Task<IActionResult> ExcluirDesejos(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_desejo = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_desejo);

            var response = httpClient.client.PostAsync("api/excluir_desejo", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarDesejos("", responseStrExc, response.StatusCode.ToString(), 0, 0, 0);
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
                { "desejos", Constantes.SIM },
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

        private TpProduto ConsultarTpprodutos()
        {

            string responseStr;
            string responseStatusCode;

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_tpproduto = new Dictionary<string, string>
            {
                { "descricao", "%" },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_tpproduto);

            var response = httpClient.client.PostAsync("api/consultar_tp_produto", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;
            responseStatusCode = response.StatusCode.ToString();

            if (!string.IsNullOrWhiteSpace(responseStr))
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