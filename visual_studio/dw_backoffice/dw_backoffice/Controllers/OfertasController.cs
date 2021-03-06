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
    public class OfertasController : Controller
    {
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ConsultarOfertas(string responseStr, string responseStrExc, string responseStatusCode)
        {
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

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.ofertas = (_oferta == null || _oferta.Conteudo == null) ? new List<Oferta.Dados>() : _oferta.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarOfertas");
        }

        public Task<IActionResult> RefazerUltimaConsultaOferta()
        {
           return ConsultarOfertasIn(true);
        }

        public async Task<IActionResult> AcaoConsultarOfertas(int id_desejo)
        {
            return await ConsultarOfertasIn(false, id_desejo);
        }

        private async Task<IActionResult> ConsultarOfertasIn(bool refazerultimaconsulta = false, int id_desejo = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_oferta = new Dictionary<string, string>
            {
                { "id_desejo", id_desejo.ToString() },
                { "origem", Constantes.BACKOFFICE },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            
            };

            if (refazerultimaconsulta)
            {
                consulta_oferta = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA_AUX]);
            }
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_oferta);

            var response = httpClient.client.PostAsync("api/consultar_oferta", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            Utils.SetCookie(Constantes.ULTIMA_CONSULTA_AUX, JsonConvert.SerializeObject(consulta_oferta), HttpContext);

            if (response.IsSuccessStatusCode)
            {
                return ConsultarOfertas(responseStr, "", response.StatusCode.ToString());
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

        public async Task<IActionResult> ExcluirOfertas(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_oferta = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_oferta);

            var response = httpClient.client.PostAsync("api/excluir_oferta", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarOfertas("", responseStrExc, response.StatusCode.ToString());
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
            ViewBag.id = (_novaoferta == null || _novaoferta == null || _novaoferta.Conteudo == null ? 0 : _novaoferta.Conteudo.Id);
            ViewBag.erro = (_novaoferta == null || _novaoferta.Resultado == null ? false : _novaoferta.Resultado.Erro);
            ViewBag.mensagem = (_novaoferta == null || _novaoferta.Resultado == null ? "" : _novaoferta.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> inclusao = JsonConvert.DeserializeObject<Dictionary<string, string>>(nova_oferta);
                ViewBag.id_empresa = inclusao["id_empresa"];
                ViewBag.id_desejo = inclusao["id_desejo"];
                ViewBag.valor = inclusao["valor"];
                ViewBag.url = inclusao["url"];
                ViewBag.descricao = inclusao["descricao"];
                ViewBag.destaque = inclusao["destaque"];
            }

            return View("IncluirOfertas");
        }

        [HttpPost]
        public async Task<IActionResult> IncluirOfertasPost(int id_desejo, string descricao_desejo, string descricao, string url, float valor, string destaque)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var nova_oferta = new Dictionary<string, string>
            {
                { "id_desejo", id_desejo.ToString()},
                { "descricao", descricao},
                { "url", url},
                { "validade", DateTime.Today.AddMonths(1).ToString()},
                { "valor", valor.ToString()},
                { "destaque",  Constantes.NAO},
                { "id_empresa",Constantes.ID_DEALWISH.ToString()},
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



    }
}