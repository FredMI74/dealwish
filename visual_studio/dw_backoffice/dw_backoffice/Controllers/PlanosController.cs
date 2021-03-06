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
    public class PlanosController : Controller
    {
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult EditarPlanos(int id, string responseStr, string responseStatusCode, string edt_plano = "")
        {

            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            Plano _plano = new Plano();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                if (id == 0)
                {
                    _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
                }
                else
                {
                    _plano = JsonConvert.DeserializeObject<Plano>(responseStr);
                }
            }
            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.qtd_ofertas = (_plano == null || _plano.Conteudo == null) ? 0 : _plano.Conteudo[0].Qtd_ofertas;
            ViewBag.valor_mensal = (_plano == null || _plano.Conteudo == null) ? 0 : _plano.Conteudo[0].Valor_mensal;
            ViewBag.valor_oferta = (_plano == null || _plano.Conteudo == null) ? 0 : _plano.Conteudo[0].Valor_oferta;
            ViewBag.descricao = (_plano == null || _plano.Conteudo == null) ? "" : _plano.Conteudo[0].Descricao;
            ViewBag.visualizacao = (_plano == null || _plano.Conteudo == null) ? "" : _plano.Conteudo[0].Visualizacao;
            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> edicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(edt_plano);
                ViewBag.id = int.Parse(edicao["id"]);
                ViewBag.descricao = edicao["descricao"];
                ViewBag.qtd_ofertas = edicao["qtd_ofertas"];
                ViewBag.valor_mensal = edicao["valor_mensal"];
                ViewBag.valor_oferta = edicao["valor_oferta"];
                ViewBag.visualizacao = edicao["visualizacao"];
            }

            return View("EditarPlanos");
        }


        public IActionResult IncluirPlanos(string responseStr, string responseStatusCode, string inclusao_plano)
        {
            NovoRegistro _novaplano = new NovoRegistro();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _novaplano = JsonConvert.DeserializeObject<NovoRegistro>(responseStr);
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = (_novaplano == null || _novaplano.Conteudo == null ? 0 : _novaplano.Conteudo.Id);
            ViewBag.erro = (_novaplano == null  || _novaplano.Resultado == null ? false : _novaplano.Resultado.Erro);
            ViewBag.mensagem = (_novaplano == null || _novaplano.Resultado == null ? "" : _novaplano.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> inclusao = JsonConvert.DeserializeObject<Dictionary<string, string>>(inclusao_plano);
                ViewBag.descricao = inclusao["descricao"];
                ViewBag.qtd_ofertas = inclusao["qtd_ofertas"];
                ViewBag.valor_mensal = inclusao["valor_mensal"];
                ViewBag.valor_oferta = inclusao["valor_oferta"];
                ViewBag.visualizacao = inclusao["visualizacao"];
            }

            return View("IncluirPlanos");
        }

        public IActionResult ConsultarPlanos(string responseStr, string responseStrExc, string responseStatusCode)
        {
            Plano _plano = new Plano();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _plano = JsonConvert.DeserializeObject<Plano>(responseStr);
                ViewBag.erro = (_plano == null || _plano.Resultado == null ? false : _plano.Resultado.Erro);
                ViewBag.mensagem = (_plano == null || _plano.Resultado == null ? "" : _plano.Resultado.Mensagem);
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
            ViewBag.planos = (_plano == null || _plano.Conteudo == null) ? new List<Plano.Dados>() : _plano.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarPlanos");
        }

        [HttpPost]
        public async Task<IActionResult>IncluirPlanosPost( string descricao, int qtd_ofertas, float valor_mensal, float valor_oferta, string visualizacao)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var novo_plano = new Dictionary<string, string>
            {
                { "descricao", descricao },
                { "qtd_ofertas", qtd_ofertas.ToString() },
                { "valor_mensal", valor_mensal.ToString() },
                { "valor_oferta", valor_oferta.ToString() },
                { "visualizacao", visualizacao ?? Constantes.NAO},
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

           
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(novo_plano);

            var response = httpClient.client.PostAsync("api/incluir_plano", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
   
            if (response.IsSuccessStatusCode)
            {
                return IncluirPlanos(responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(novo_plano));
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

        public Task<IActionResult> RefazerUltimaConsultaPlano()
        {
           return ConsultarPlanosIn(true);
        }

        public Task<IActionResult> AcaoEditarPlanos(int id)
        {
            return ConsultarPlanosIn(false, true, id);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarPlanosPost( int id, string descricao)
        {
            return await ConsultarPlanosIn(false, false, id, descricao);
        }
   
        private async Task<IActionResult> ConsultarPlanosIn(bool refazerultimaconsulta = false, bool editando = false, int id = 0, string descricao = "" )
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_plano = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao },
                { "token",Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            
            if (refazerultimaconsulta)
            {
                consulta_plano = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_plano);

            var response = httpClient.client.PostAsync("api/consultar_plano", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
           
            if (response.IsSuccessStatusCode)
            {
                if (!editando)
                {
                    Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_plano), HttpContext);
                    return ConsultarPlanos(responseStr, "", response.StatusCode.ToString());
                }
                else
                {
                    return EditarPlanos(id, responseStr, response.StatusCode.ToString());
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

        public async Task<IActionResult> ExcluirPlanos(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_plano = new Dictionary<string, string>
            {
              { "id", id.ToString() },
              { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_plano);

            var response = httpClient.client.PostAsync("api/excluir_plano", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();
  
            if (response.IsSuccessStatusCode)
            {
                return ConsultarPlanos("", responseStrExc, response.StatusCode.ToString());
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
        public async Task<IActionResult> EditarPlanosPost( int id, string descricao, int qtd_ofertas, float valor_mensal, float valor_oferta, string visualizacao)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var edt_plano = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao },
                { "qtd_ofertas", qtd_ofertas.ToString() },
                { "valor_mensal", valor_mensal.ToString() },
                { "valor_oferta", valor_oferta.ToString() },
                { "visualizacao", visualizacao ?? Constantes.NAO },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(edt_plano);

            var response = httpClient.client.PostAsync("api/atualizar_plano", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
          
            if (response.IsSuccessStatusCode)
            {
                return EditarPlanos(0, responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(edt_plano));
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