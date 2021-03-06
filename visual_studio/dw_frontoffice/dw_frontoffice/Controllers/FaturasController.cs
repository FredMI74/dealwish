using dw_frontoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class FaturasController : Controller
    {
        private Situacao situacao;
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    
        public IActionResult ConsultarFaturas(string responseStr = "", string responseStatusCode = "")
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
            else
            {
                ViewBag.erro = false;
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

        [HttpPost]
        public async Task<IActionResult> ConsultarFaturasPost(int id_situacao)
        {
            return await ConsultarFaturasIn(id_situacao);
        }

        public async Task<IActionResult> RefazerUltimaConsultaFatura(int id_situacao = 0)
        {
            return await ConsultarFaturasIn(id_situacao);
        }

        private async Task<IActionResult> ConsultarFaturasIn(int id_situacao = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_fatura = new Dictionary<string, string>
            {
                { "id_empresa", Request.Cookies[Constantes.ID_EMPRESA] },
                { "abertas", id_situacao == -1 ? Constantes.SIM : Constantes.NAO},
                { "id_situacao", id_situacao == -1 ? "999" : id_situacao == 0 ? "9" : id_situacao.ToString()},
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_fatura);

            var response = httpClient.client.PostAsync("api/consultar_fatura", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarFaturas(responseStr, response.StatusCode.ToString());
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
                { "origem", Constantes.FRONTOFFICE },
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