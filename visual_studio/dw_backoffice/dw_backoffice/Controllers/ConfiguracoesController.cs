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
    public class ConfiguracoesController : Controller
    {

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult EditarConfiguracoes(int id, string responseStr, string responseStatusCode, string edt_configuracao = "")
        {

            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            Configuracao _configuracao = new Configuracao();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                if (id == 0)
                {
                    _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
                }
                else
                {
                    _configuracao = JsonConvert.DeserializeObject<Configuracao>(responseStr);
                }
            }
            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.codigo = (_configuracao == null || _configuracao.Conteudo == null) ? "" : _configuracao.Conteudo[0].Codigo;
            ViewBag.valor = (_configuracao == null || _configuracao.Conteudo == null) ? "" : _configuracao.Conteudo[0].Valor;
            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> edicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(edt_configuracao);
                ViewBag.id = int.Parse(edicao["id"]);
                ViewBag.codigo = edicao["codigo"];
                ViewBag.valor = edicao["valor"];
            }

            return View("EditarConfiguracoes");
        }

        public IActionResult ExcluirConfiguracoes(int id, string codigo, string responseStr , string responseStatusCode)
        {

            ProcessarRegistro _processarregistro = new ProcessarRegistro();

            if (responseStr != null && id == 0)
            {
                _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.codigo = codigo;
            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            return View("ExcluirConfiguracoes");
        }

        public IActionResult ConsultarConfiguracoes(string responseStr, string responseStatusCode)
        {
            Configuracao _configuracao = new Configuracao();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _configuracao = JsonConvert.DeserializeObject<Configuracao>(responseStr);
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.configuracoes = (_configuracao == null || _configuracao.Conteudo == null) ? new List<Configuracao.Dados>() : _configuracao.Conteudo;
            ViewBag.erro = (_configuracao == null || _configuracao.Resultado == null ? false : _configuracao.Resultado.Erro);
            ViewBag.mensagem = (_configuracao == null || _configuracao.Resultado == null ? "" : _configuracao.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarConfiguracoes");
        }

        public Task<IActionResult> RefazerUltimaConsultaConfiguracao()
        {
           return ConsultarConfiguracoesIn(true);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarConfiguracoesPost( int id, string codigo, string valor)
        {
            return await ConsultarConfiguracoesIn(false, false, id, codigo, valor);
        }

        private async Task<IActionResult> ConsultarConfiguracoesIn(bool refazerultimaconsulta = false, bool editando = false, int id = 0, string codigo = "", string valor = "")
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_configuracao = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "codigo", codigo },
                { "valor", valor },
                {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            
            if (refazerultimaconsulta)
            {
                consulta_configuracao = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_configuracao);

            var response = httpClient.client.PostAsync("api/consultar_config", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
          
            if (response.IsSuccessStatusCode)
            {
                if (!editando)
                {
                    Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_configuracao), HttpContext);
                    return ConsultarConfiguracoes(responseStr, response.StatusCode.ToString());
                }
                else
                {
                    return EditarConfiguracoes(id, responseStr, response.StatusCode.ToString());
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

        public Task<IActionResult> AcaoEditarConfiguracoes(int id)
        {
            return ConsultarConfiguracoesIn(false, true, id);
        }

        [HttpPost]
        public async Task<IActionResult> EditarConfiguracoesPost( int id, string codigo, string valor)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var edt_configuracao = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "codigo", codigo },
                { "valor", valor },
                {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(edt_configuracao);

            var response = httpClient.client.PostAsync("api/atualizar_config", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return EditarConfiguracoes(0, responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(edt_configuracao));
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