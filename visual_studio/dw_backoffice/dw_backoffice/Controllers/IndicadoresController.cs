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
using System.Net.Http.Headers;

namespace dw_backoffice.Controllers
{
    [Authorize]
    public class IndicadoresController : Controller
    {

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ConsultarIndicadores(string responseStr, string responseStatusCode)
        {
            Indicador _Indicador = new Indicador();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _Indicador = JsonConvert.DeserializeObject<Indicador>(responseStr);
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.Indicadores = (_Indicador == null || _Indicador.Conteudo == null) ? new List<Indicador.Dados>() : _Indicador.Conteudo;
            ViewBag.erro = (_Indicador == null || _Indicador.Resultado == null ? false : _Indicador.Resultado.Erro);
            ViewBag.mensagem = (_Indicador == null || _Indicador.Resultado == null ? "" : _Indicador.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarIndicadores");
        }

        public Task<IActionResult> RefazerUltimaConsultaIndicador()
        {
           return ConsultarIndicadoresIn(true);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarIndicadoresPost()
        {
            return await ConsultarIndicadoresIn(false);
        }

        private async Task<IActionResult> ConsultarIndicadoresIn(bool refazerultimaconsulta = false)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_indicador = new Dictionary<string, string>
            {
                {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            
            if (refazerultimaconsulta)
            {
                consulta_indicador = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_indicador);

            var response = httpClient.client.PostAsync("api/consultar_indicadores", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
          
            if (response.IsSuccessStatusCode)
            {
               Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_indicador), HttpContext);
               return ConsultarIndicadores(responseStr, response.StatusCode.ToString());
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

    }
}