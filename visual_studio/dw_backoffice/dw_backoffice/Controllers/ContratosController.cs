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
    public class ContratosController : Controller
    {
        private Plano plano;
        private Situacao situacao;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult EditarContratos(int id, string responseStr, string responseStatusCode, string edt_contratos = "")
        {
            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            Contrato _contrato = new Contrato();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                if (id == 0)
                {
                    _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    _contrato = JsonConvert.DeserializeObject<Contrato>(responseStr, settings);
                }
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            if (plano == null)
            {
                plano = new Plano();
                plano = ConsultarPlanos();
            }


            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.id_empresa = (_contrato == null || _contrato.Conteudo == null) ? 0 : _contrato.Conteudo[0].Id_empresa;
            ViewBag.razao_social = (_contrato == null || _contrato.Conteudo == null) ? "" : _contrato.Conteudo[0].Razao_social;
            ViewBag.id_situacao = (_contrato == null || _contrato.Conteudo == null) ? 0 : _contrato.Conteudo[0].Id_situacao;
            ViewBag.id_plano = (_contrato == null || _contrato.Conteudo == null) ? 0 : _contrato.Conteudo[0].Id_plano;
            ViewBag.dia_vct = (_contrato == null || _contrato.Conteudo == null) ? 0 : _contrato.Conteudo[0].Dia_vct;

            ViewBag.data_inicio = (_contrato == null || _contrato.Conteudo == null) ? "" : _contrato.Conteudo[0].Data_inicio.ToString("yyyy-MM-dd");
            ViewBag.data_termino = (_contrato == null || _contrato.Conteudo == null || _contrato.Conteudo[0].Data_termino.ToString("dd/MM/yyyy") == "01/01/0001") ? "" : _contrato.Conteudo[0].Data_termino.ToString("yyyy-MM-dd");

            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            ViewBag.situacoes = situacao.Conteudo;
            ViewBag.planos = plano.Conteudo;

            if (ViewBag.erro)
            {
                Dictionary<string, string> edicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(edt_contratos);
                ViewBag.id = int.Parse(edicao["id"]);
                ViewBag.id_empresa = int.Parse(edicao["id_empresa"]);
                ViewBag.razao_social = edicao["razao_social"];
                ViewBag.id_plano = int.Parse(edicao["id_plano"]);
                ViewBag.id_situacao = int.Parse(edicao["id_situacao"]);
                ViewBag.dia_vct = int.Parse(edicao["dia_vct"]);
                ViewBag.data_inicio = DateTime.Parse(edicao["data_inicio"]).ToString("yyyy-MM-dd");
            }

            return View("EditarContratos");
        }

        public IActionResult IncluirContratos(string responseStr, string responseStatusCode, string inclusao_contrato)
        {
            NovoRegistro _novacontrato = new NovoRegistro();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _novacontrato = JsonConvert.DeserializeObject<NovoRegistro>(responseStr);
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            if (plano == null)
            {
                plano = new Plano();
                plano = ConsultarPlanos();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = (_novacontrato == null || _novacontrato.Conteudo == null ? 0 : _novacontrato.Conteudo.Id);

            ViewBag.erro = (_novacontrato == null  || _novacontrato.Resultado == null ? false : _novacontrato.Resultado.Erro);
            ViewBag.mensagem = (_novacontrato == null || _novacontrato.Resultado == null ? "" : _novacontrato.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            ViewBag.situacoes = situacao.Conteudo;
            ViewBag.planos = plano.Conteudo;

            if (ViewBag.erro)
            {
                Dictionary<string, string> inclusao = JsonConvert.DeserializeObject<Dictionary<string, string>>(inclusao_contrato);
                ViewBag.id_empresa = int.Parse(inclusao["id_empresa"]);
                ViewBag.razao_social = inclusao["razao_social"];
                ViewBag.id_plano = int.Parse(inclusao["id_plano"]);
                ViewBag.id_situacao = int.Parse(inclusao["id_situacao"]);
                ViewBag.dia_vct = int.Parse(inclusao["dia_vct"]);
                ViewBag.data_inicio = DateTime.Parse(inclusao["data_inicio"]).ToString("yyyy-MM-dd");
            }

            return View("IncluirContratos");
        }

        public IActionResult ConsultarContratos(string acao = "", string responseStr = "", string responseStrExc ="" , string responseStatusCode = "")
        {
            ViewBag.acao = acao;

            Contrato _contrato = new Contrato();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                _contrato = JsonConvert.DeserializeObject<Contrato>(responseStr, settings);
                ViewBag.erro = (_contrato == null || _contrato.Resultado == null ? false : _contrato.Resultado.Erro);
                ViewBag.mensagem = (_contrato == null || _contrato.Resultado == null ? "" : _contrato.Resultado.Mensagem);
            }
          
            if (!string.IsNullOrWhiteSpace(responseStrExc))
            {
                ProcessarRegistro _proc = new ProcessarRegistro();
                _proc = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStrExc);
                ViewBag.erro = (_proc == null || _proc.Resultado == null ? false : _proc.Resultado.Erro);
                ViewBag.mensagem = (_proc == null || _proc.Resultado == null ? "" : _proc.Resultado.Mensagem);
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            if (plano == null)
            {
                plano = new Plano();
                plano = ConsultarPlanos();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.contratos = (_contrato == null || _contrato.Conteudo == null) ? new List<Contrato.Dados>() : _contrato.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            ViewBag.situacoes = situacao.Conteudo;
            ViewBag.planos = plano.Conteudo;

            return View("ConsultarContratos");
        }

        public Task<IActionResult> AcaoEditarContratos(int id)
        {
            return ConsultarContratosIn(false, true, id);
        }


        [HttpPost]
        public async Task<IActionResult>IncluirContratosPost( int id_empresa, string razao_social, int id_plano, int id_situacao, int dia_vct, DateTime data_inicio)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var novo_contrato = new Dictionary<string, string>
            {
                { "id_empresa", id_empresa.ToString() },
                { "razao_social", razao_social },
                { "id_plano", id_plano.ToString() },
                { "id_situacao", id_situacao.ToString() },
                { "dia_vct", dia_vct.ToString() },
                { "data_inicio", data_inicio.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(novo_contrato);

            var response = httpClient.client.PostAsync("api/incluir_contrato", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return IncluirContratos(responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(novo_contrato));
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

        public Task<IActionResult> AtualizarInadimplentes()
        {
            return ConsultarInadimplentesIn(false);
        }

        private async Task<IActionResult> ConsultarInadimplentesIn(bool refazerultimaconsulta = false)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_contrato = new Dictionary<string, string>
            {
                { "inadimplentes", Constantes.SIM },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };


            if (refazerultimaconsulta)
            {
                consulta_contrato = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_contrato);

            var response = httpClient.client.PostAsync("api/consultar_contrato", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                 Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_contrato), HttpContext);
                 return ConsultarInadimplentes(responseStr, "", response.StatusCode.ToString());
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

        public Task<IActionResult> RefazerUltimaConsultaContrato()
        {
           return ConsultarContratosIn(true);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarContratosPost( int id, int id_empresa, int id_plano, int id_situacao, int dia_vct, DateTime data_inicio, DateTime data_bloqueio, DateTime data_termino)
        {
            return await ConsultarContratosIn(false, false, id, id_empresa, id_plano, id_situacao, dia_vct, data_inicio.ToString(), data_bloqueio.ToString(), data_termino.ToString());
        }

        private async Task<IActionResult> ConsultarContratosIn(bool refazerultimaconsulta = false, bool editando = false, int id = 0, int id_empresa = 0, int id_plano = 0, 
                                                               int id_situacao = 0, int dia_vct = 0, string data_inicio = "", string data_bloqueio = "", string data_termino = "")
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_contrato = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "id_empresa", id_empresa.ToString() },
                { "id_plano", id_plano.ToString() },
                { "id_situacao", id_situacao.ToString() },
                { "dia_vct", dia_vct.ToString() },
                { "data_inicio", data_inicio },
                { "data_bloqueio", data_bloqueio },
                { "data_termino", data_termino },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            
            };

            if (refazerultimaconsulta)
            {
                consulta_contrato = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_contrato);

            var response = httpClient.client.PostAsync("api/consultar_contrato", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            
            if (response.IsSuccessStatusCode)
            {
                if (!editando)
                {
                    Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_contrato), HttpContext);
                    return ConsultarContratos("", responseStr, "", response.StatusCode.ToString());
                }
                else
                {
                    return EditarContratos(id, responseStr, response.StatusCode.ToString());
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

        public async Task<IActionResult> ExcluirContratos(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_contrato = new Dictionary<string, string>
            {
               { "id", id.ToString() },
               { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_contrato);

            var response = httpClient.client.PostAsync("api/excluir_contrato", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarContratos("E", "", responseStrExc, response.StatusCode.ToString());
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

        public async Task<IActionResult> DesbloquearContratos(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var desbloquear_contrato = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(desbloquear_contrato);

            var response = httpClient.client.PostAsync("api/desbloquear_contrato", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarContratos("D", "", responseStrExc, response.StatusCode.ToString());
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

        public IActionResult ConsultarInadimplentes(string responseStr, string responseStrExc, string responseStatusCode)
        {

            Contrato _contrato = new Contrato();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore

                };
                _contrato = JsonConvert.DeserializeObject<Contrato>(responseStr, settings);
                ViewBag.erro = (_contrato == null || _contrato.Resultado == null ? false : _contrato.Resultado.Erro);
                ViewBag.mensagem = (_contrato == null || _contrato.Resultado == null ? "" : _contrato.Resultado.Mensagem);
                ViewBag.bloqueando = false;
            }

            if (!string.IsNullOrWhiteSpace(responseStrExc))
            {
                ProcessarRegistro _proc = new ProcessarRegistro();
                _proc = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStrExc);
                ViewBag.erro = (_proc == null || _proc.Resultado == null ? false : _proc.Resultado.Erro);
                ViewBag.mensagem = (_proc == null || _proc.Resultado == null ? "" : _proc.Resultado.Mensagem);
                ViewBag.bloqueando = true;
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.contratos = (_contrato == null || _contrato.Conteudo == null) ? new List<Contrato.Dados>() : _contrato.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarInadimplentes");
        }

        [HttpPost]
        public async Task<IActionResult> EditarContratosPost( int id, int id_empresa, string razao_social, int id_plano, int id_situacao, int dia_vct, DateTime data_inicio, DateTime data_bloqueio, DateTime data_termino)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var edt_contrato = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "id_empresa", id_empresa.ToString() },
                { "razao_social", razao_social },
                { "id_plano", id_plano.ToString() },
                { "id_situacao", id_situacao.ToString() },
                { "dia_vct", dia_vct.ToString() },
                { "data_inicio", data_inicio.ToString() },
                { "data_bloqueio", data_bloqueio.ToString() },
                { "data_termino", data_termino.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(edt_contrato);

            var response = httpClient.client.PostAsync("api/atualizar_contrato", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return EditarContratos(0, responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(edt_contrato));
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

        public async Task<IActionResult> BloquearInadimplentes()
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var bloquear_inadimplentes = new Dictionary<string, string>
            {
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(bloquear_inadimplentes);

            var response = httpClient.client.PostAsync("api/bloquear_contratos_inadimplentes", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarInadimplentes("", responseStrExc, response.StatusCode.ToString());
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
                { "contratos", Constantes.SIM},
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

        private Plano ConsultarPlanos()
        {

            string responseStr;
            string responseStatusCode;

            DwClienteHttp httpClient = DwClienteHttp.Instance;

            var consulta_plano = new Dictionary<string, string>
            {
                { "descricao", "%" },
                {"token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_plano);

            var response = httpClient.client.PostAsync("api/consultar_plano", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;
            responseStatusCode = response.StatusCode.ToString();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                return JsonConvert.DeserializeObject<Plano>(responseStr);
            }
            else
            {
                return null;
            }
        }

    }

}