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
using System.Net.Http.Headers;

namespace dw_backoffice.Controllers
{
    [Authorize]
    public class TpProdutosController : Controller
    {
        private GrpProduto grpproduto;
        private Situacao situacao;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult EditarTpProdutos(int id, string responseStr, string responseStatusCode, string edt_tpprodutos = "")
        {
            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            TpProduto _tpproduto = new TpProduto();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                if (id == 0)
                {
                    _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
                }
                else
                {
                    _tpproduto = JsonConvert.DeserializeObject<TpProduto>(responseStr);
                }
            }

            if (grpproduto == null)
            {
                grpproduto = new GrpProduto();
                grpproduto = ConsultarGrpProdutos();
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.descricao = (_tpproduto == null || _tpproduto.Conteudo == null) ? "" : _tpproduto.Conteudo[0].Descricao;
            ViewBag.preenchimento = (_tpproduto == null || _tpproduto.Conteudo == null) ? "" : _tpproduto.Conteudo[0].Preenchimento;
            ViewBag.grpprodutos = grpproduto.Conteudo;
            ViewBag.id_grp_prod = (_tpproduto == null || _tpproduto.Conteudo == null) ? 0 : _tpproduto.Conteudo[0].Id_grp_prod;
            ViewBag.id_situacao = (_tpproduto == null || _tpproduto.Conteudo == null) ? 0 : _tpproduto.Conteudo[0].Id_situacao;
            ViewBag.icone = (_tpproduto == null || _tpproduto.Conteudo == null) ? "" : _tpproduto.Conteudo[0].Icone;
            ViewBag.ordem = (_tpproduto == null || _tpproduto.Conteudo == null) ? 0 : _tpproduto.Conteudo[0].Ordem;
            ViewBag.situacoes = situacao.Conteudo;
            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> edicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(edt_tpprodutos);
                ViewBag.id = int.Parse(edicao["id"]);
                ViewBag.descricao = edicao["descricao"];
                ViewBag.preenchimento = edicao["preenchimento"];
                ViewBag.id_grp_prod = int.Parse(edicao["id_grp_prod"]);
                ViewBag.icone = edicao["icone"];
            }



            return View("EditarTpProdutos");
        }


        public IActionResult IncluirTpProdutos(string responseStr, string responseStatusCode, string inclusao_tpproduto)
        {
            NovoRegistro _novatpproduto = new NovoRegistro();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _novatpproduto = JsonConvert.DeserializeObject<NovoRegistro>(responseStr);
            }

            if (grpproduto == null)
            {
                grpproduto = new GrpProduto();
                grpproduto = ConsultarGrpProdutos();
            }
            
            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = (_novatpproduto == null || _novatpproduto.Conteudo == null ? 0 : _novatpproduto.Conteudo.Id);
            ViewBag.grpprodutos = grpproduto.Conteudo;
            ViewBag.erro = (_novatpproduto == null  || _novatpproduto.Resultado == null ? false : _novatpproduto.Resultado.Erro);
            ViewBag.mensagem = (_novatpproduto == null || _novatpproduto.Resultado == null ? "" : _novatpproduto.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> inclusao = JsonConvert.DeserializeObject<Dictionary<string, string>>(inclusao_tpproduto);
                ViewBag.descricao = inclusao["descricao"];
                ViewBag.preenchimento = inclusao["preenchimento"];
                ViewBag.id_grp_prod = int.Parse(inclusao["id_grp_prod"]);
                ViewBag.icone = inclusao["icone"];
            }

            return View("IncluirTpProdutos");
        }

        public IActionResult ConsultarTpProdutos(string responseStr, string responseStrExc, string responseStatusCode, int pagina, int num_pag, int max_id)
        {
            TpProduto _tpproduto = new TpProduto();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _tpproduto = JsonConvert.DeserializeObject<TpProduto>(responseStr);
                ViewBag.erro = (_tpproduto == null || _tpproduto.Resultado == null ? false : _tpproduto.Resultado.Erro);
                ViewBag.mensagem = (_tpproduto == null || _tpproduto.Resultado == null ? "" : _tpproduto.Resultado.Mensagem);
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

            if (grpproduto == null)
            {
                grpproduto = new GrpProduto();
                grpproduto = ConsultarGrpProdutos();
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.tpprodutos = (_tpproduto == null || _tpproduto.Conteudo == null) ? new List<TpProduto.Dados>() : _tpproduto.Conteudo;
            ViewBag.situacoes = situacao.Conteudo;
            ViewBag.grpprodutos = grpproduto.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            if (_tpproduto.InfoPagina != null)
            {
               max_id = _tpproduto.InfoPagina.Max_id;
               num_pag = (_tpproduto.InfoPagina.Count_id / 20) + (_tpproduto.InfoPagina.Count_id % 20 > 0 ? 1 : 0);
            }

            ViewBag.NumPag = num_pag;
            ViewBag.Pagina = pagina;
            ViewBag.MaxId = max_id;
            ViewBag.AntePag = pagina > 1 && _tpproduto.Conteudo != null;
            ViewBag.ProxPag = pagina != num_pag && _tpproduto.Conteudo != null;

            return View("ConsultarTpProdutos");
        }


        [HttpPost]
        public async Task<IActionResult>IncluirTpProdutosPost( string descricao, int id_grp_prod, string preenchimento, string icone, int ordem)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var novo_tpproduto = new Dictionary<string, string>
            {
                { "descricao", descricao },
                { "id_grp_prod", id_grp_prod.ToString() },
                { "preenchimento", preenchimento },
                { "icone", icone },
                 { "ordem", ordem.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };
  
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(novo_tpproduto);

            var response = httpClient.client.PostAsync("api/incluir_tp_produto", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return IncluirTpProdutos(responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(novo_tpproduto));
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

        public Task<IActionResult> RefazerUltimaConsultaTpProduto()
        {
           return ConsultarTpProdutosIn(true);
        }

        public Task<IActionResult> ProxPaginaTpProduto(int pagina, int num_pag, int max_id)
        {
            pagina += 1;
            if (pagina > num_pag)
            {
                pagina = num_pag;
            }
            return ConsultarTpProdutosIn(true, pagina: pagina, num_pag: num_pag, max_id: max_id);
        }

        public Task<IActionResult> AntePaginaTpProduto(int pagina, int num_pag, int max_id)
        {
            pagina -= 1;
            if (pagina <= 0)
            {
                pagina = 1; 
            }
            return ConsultarTpProdutosIn(true, pagina: pagina, num_pag: num_pag, max_id: max_id);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarTpProdutosPost( int id, string descricao, int id_grp_prod, int id_situacao)
        {
            return await ConsultarTpProdutosIn(false, false, id, descricao, id_grp_prod, id_situacao);
        }

        private async Task<IActionResult> ConsultarTpProdutosIn(bool refazerultimaconsulta = false, bool editando = false, int id = 0, string descricao = "", int id_grp_prod = 0, int id_situacao = 0, int pagina = 1, int num_pag = 0, int max_id = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_tpproduto = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao },
                { "id_grp_prod", id_grp_prod.ToString() },
                { "id_situacao", id_situacao.ToString() },
                { "paginacao", Constantes.SIM},
                { "max_id", "0"},
                { "pagina", "1"},
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] },
                { "num_pag", num_pag.ToString()}
            };
            
            if (refazerultimaconsulta)
            {
                consulta_tpproduto = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
                if (num_pag != 0)
                {
                    consulta_tpproduto["pagina"] = pagina.ToString();
                    consulta_tpproduto["max_id"] = max_id.ToString();
                    consulta_tpproduto["num_pag"] = num_pag.ToString();
                }
                else
                {
                    pagina = Int32.Parse(consulta_tpproduto["pagina"]);
                    max_id = Int32.Parse(consulta_tpproduto["max_id"]);
                    num_pag = Int32.Parse(consulta_tpproduto["num_pag"]);
                }
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_tpproduto);

            var response = httpClient.client.PostAsync("api/consultar_tp_produto", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (!editando)
                {
                    Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_tpproduto), HttpContext);
                    return ConsultarTpProdutos(responseStr, "", response.StatusCode.ToString(), pagina ,num_pag, max_id);
                }
                else
                {
                    return EditarTpProdutos(id, responseStr, response.StatusCode.ToString());
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

        public async Task<IActionResult> ExcluirTpProdutos(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_tpproduto = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_tpproduto);

            var response = httpClient.client.PostAsync("api/excluir_tp_produto", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarTpProdutos("", responseStrExc, response.StatusCode.ToString(), 0, 0, 0);
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

        public Task<IActionResult> AcaoEditarTpProdutos(int id)
        {
            return ConsultarTpProdutosIn(false, true, id);
        }


        [HttpPost]
        public async Task<IActionResult> EditarTpProdutosPost( int id, string descricao, int id_grp_prod, int id_situacao, string preenchimento, string icone, int ordem)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var edt_tpproduto = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao },
                { "id_grp_prod", id_grp_prod.ToString() },
                { "preenchimento", preenchimento },
                { "id_situacao", id_situacao.ToString() },
                { "icone", icone},
                { "ordem", ordem.ToString() },
                { "token",Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(edt_tpproduto);

            var response = httpClient.client.PostAsync("api/atualizar_tp_produto", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return EditarTpProdutos(0, responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(edt_tpproduto));
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

        private GrpProduto ConsultarGrpProdutos()
        {
            string responseStr;

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_grpproduto = new Dictionary<string, string>
            {
                {"descricao", "%" },
                {"id_situacao", Constantes.ATIVO.ToString() }, 
                {"token",Request.Cookies[Constantes.TOKEN_USUARIO] }
            };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_grpproduto);

            var response = httpClient.client.PostAsync("api/consultar_grp_produto", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                return JsonConvert.DeserializeObject<GrpProduto>(responseStr);
            }
            else
            {
                return null;
            }
        }

    }
}