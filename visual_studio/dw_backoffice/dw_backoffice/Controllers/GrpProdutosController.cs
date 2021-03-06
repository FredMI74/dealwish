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
    public class GrpProdutosController : Controller
    {
        private Situacao situacao;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult EditarGrpProdutos(int id, string responseStr, string responseStatusCode, string edt_grpprodutos = "")
        {

            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            GrpProduto _grpproduto = new GrpProduto();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                if (id == 0)
                {
                    _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
                }
                else
                {
                    _grpproduto = JsonConvert.DeserializeObject<GrpProduto>(responseStr);
                }
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.descricao = (_grpproduto == null || _grpproduto.Conteudo == null) ? "" : _grpproduto.Conteudo[0].Descricao;
            ViewBag.id_situacao = (_grpproduto == null || _grpproduto.Conteudo == null) ? 0 : _grpproduto.Conteudo[0].Id_situacao;
            ViewBag.icone = (_grpproduto == null || _grpproduto.Conteudo == null) ? "" : _grpproduto.Conteudo[0].Icone;
            ViewBag.situacoes = situacao.Conteudo;
            ViewBag.ordem = (_grpproduto == null || _grpproduto.Conteudo == null) ? 0 : _grpproduto.Conteudo[0].Ordem;
            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> edicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(edt_grpprodutos);
                ViewBag.id = int.Parse(edicao["id"]);
                ViewBag.descricao = edicao["descricao"];
                ViewBag.icone = edicao["icone"];
            }


            return View("EditarGrpProdutos");
        }


        public IActionResult IncluirGrpProdutos(string responseStr, string responseStatusCode, string inclusao_grpproduto)
        {
            NovoRegistro _novagrpproduto = new NovoRegistro();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _novagrpproduto = JsonConvert.DeserializeObject<NovoRegistro>(responseStr);
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = (_novagrpproduto == null || _novagrpproduto.Conteudo == null ? 0 : _novagrpproduto.Conteudo.Id);
            ViewBag.erro = (_novagrpproduto == null  || _novagrpproduto.Resultado == null ? false : _novagrpproduto.Resultado.Erro);
            ViewBag.mensagem = (_novagrpproduto == null || _novagrpproduto.Resultado == null ? "" : _novagrpproduto.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> inclusao = JsonConvert.DeserializeObject<Dictionary<string, string>>(inclusao_grpproduto);
                ViewBag.descricao = inclusao["descricao"];
                ViewBag.icone = inclusao["icone"];
            }

            return View("IncluirGrpProdutos");
        }

        public IActionResult ConsultarGrpProdutos(string responseStr, string responseStrExc, string responseStatusCode)
        {
            GrpProduto _grpproduto = new GrpProduto();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _grpproduto = JsonConvert.DeserializeObject<GrpProduto>(responseStr);
                ViewBag.erro = (_grpproduto == null || _grpproduto.Resultado == null ? false : _grpproduto.Resultado.Erro);
                ViewBag.mensagem = (_grpproduto == null || _grpproduto.Resultado == null ? "" : _grpproduto.Resultado.Mensagem);
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
            ViewBag.grpprodutos = (_grpproduto == null || _grpproduto.Conteudo == null) ? new List<GrpProduto.Dados>() : _grpproduto.Conteudo;
            ViewBag.situacoes = situacao.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarGrpProdutos");
        }


        [HttpPost]
        public async Task<IActionResult>IncluirGrpProdutosPost( string descricao, string icone, int ordem)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var novo_grpproduto = new Dictionary<string, string>
            {
                { "descricao", descricao },
                { "icone", icone },
                { "ordem", ordem.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };
 
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(novo_grpproduto);

            var response = httpClient.client.PostAsync("api/incluir_grp_produto", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return IncluirGrpProdutos(responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(novo_grpproduto));
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

        public Task<IActionResult> RefazerUltimaConsultaGrpProduto()
        {
           return ConsultarGrpProdutosIn(true);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarGrpProdutosPost( int id, string descricao, int id_situacao, bool refazerultimaconsulta = false, bool editando = false)
        {
            return await ConsultarGrpProdutosIn(false, false, id, descricao, id_situacao);
        }

        private async Task<IActionResult> ConsultarGrpProdutosIn(bool refazerultimaconsulta = false, bool editando = false, int id = 0, string descricao = "" , int id_situacao = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_grpproduto = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao },
                { "id_situacao", id_situacao.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            
            if (refazerultimaconsulta)
            {
                consulta_grpproduto = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_grpproduto);

            var response = httpClient.client.PostAsync("api/consultar_grp_produto", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (!editando)
                {
                    Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_grpproduto), HttpContext);
                    return ConsultarGrpProdutos(responseStr, "", response.StatusCode.ToString());
                }
                else
                {
                    return EditarGrpProdutos(id, responseStr, response.StatusCode.ToString());
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

        public async Task<IActionResult> ExcluirGrpProdutos(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_grpproduto = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_grpproduto);

            var response = httpClient.client.PostAsync("api/excluir_grp_produto", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarGrpProdutos("", responseStrExc, response.StatusCode.ToString());
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

        public Task<IActionResult> AcaoEditarGrpProdutos(int id)
        {
            return ConsultarGrpProdutosIn(false, true, id);
        }


        [HttpPost]
        public async Task<IActionResult> EditarGrpProdutosPost( int id, string descricao, int id_situacao, string icone, int ordem)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var edt_grpproduto = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao },
                { "id_situacao", id_situacao.ToString() },
                { "icone", icone},
                { "ordem", ordem.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(edt_grpproduto);

            var response = httpClient.client.PostAsync("api/atualizar_grp_produto", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return EditarGrpProdutos(0, responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(edt_grpproduto));
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
                { "produtos", Constantes.SIM },
                { "origem", Constantes.BACKOFFICE },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_situacao);

            var response = httpClient.client.PostAsync("api/consultar_situacao", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;

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