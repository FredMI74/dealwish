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
    public class EmpresasController : Controller
    {
        private Qualificacao qualificacao;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult EditarEmpresas(int id, string responseStr, string responseStatusCode, string edt_empresas = "")
        {
            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            Empresa _Empresa = new Empresa();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                if (id == 0)
                {
                    _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStr);
                }
                else
                {
                    _Empresa = JsonConvert.DeserializeObject<Empresa>(responseStr);
                }
            }

            if (qualificacao == null)
            {
                qualificacao = new Qualificacao();
                qualificacao = ConsultarQualificacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.fantasia = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Fantasia;
            ViewBag.razao_social = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Razao_social;
            ViewBag.cnpj = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Cnpj;
            ViewBag.insc_est = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Insc_est;
            ViewBag.url = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Url;
            ViewBag.logo = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Logo;
            ViewBag.email_com = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Email_com;
            ViewBag.email_sac = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Email_sac;
            ViewBag.fone_com = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Fone_com;
            ViewBag.fone_sac = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Fone_sac;
            ViewBag.endereco = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Endereco;
            ViewBag.numero = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Numero;
            ViewBag.complemento = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Complemento;
            ViewBag.bairro = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Bairro;
            ViewBag.cep = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Cep;
            ViewBag.endereco_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Endereco_cob;
            ViewBag.numero_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Numero_cob;
            ViewBag.complemento_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Complemento_cob;
            ViewBag.bairro_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Bairro_cob;
            ViewBag.cep_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Cep_cob;
            ViewBag.id_cidade = (_Empresa == null || _Empresa.Conteudo == null) ? 0 : _Empresa.Conteudo[0].Id_cidade;
            ViewBag.nome_cidade = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Nome_cidade + "/" + _Empresa.Conteudo[0].Uf;
            ViewBag.id_cidade_cob = (_Empresa == null || _Empresa.Conteudo == null) ? 0 : _Empresa.Conteudo[0].Id_cidade_cob;
            ViewBag.nome_cidade_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Nome_cidade_cob + "/" + _Empresa.Conteudo[0].Uf_cob;
            ViewBag.id_qualificacao = (_Empresa == null || _Empresa.Conteudo == null) ? 0 : _Empresa.Conteudo[0].Id_qualificacao;

            ViewBag.qualificacoes = qualificacao.Conteudo;

            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> edicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(edt_empresas);
                ViewBag.id = int.Parse(edicao["id"]);
                ViewBag.fantasia = edicao["fantasia"];
                ViewBag.razao_social = edicao["razao_social"];
                ViewBag.cnpj = edicao["cnpj"];
                ViewBag.insc_est = edicao["insc_est"];
                ViewBag.url = edicao["url"];
                ViewBag.logo = edicao["logo"];
                ViewBag.email_com = edicao["email_com"];
                ViewBag.email_sac = edicao["email_sac"];
                ViewBag.fone_com = edicao["fone_com"];
                ViewBag.fone_sac = edicao["fone_sac"];
                ViewBag.endereco = edicao["endereco"];
                ViewBag.numero = edicao["numero"];
                ViewBag.complemento = edicao["complemento"];
                ViewBag.bairro = edicao["bairro"];
                ViewBag.cep = edicao["cep"];
                ViewBag.endereco_cob = edicao["endereco_cob"];
                ViewBag.numero_cob = edicao["numero_cob"];
                ViewBag.complemento_cob = edicao["complemento_cob"];
                ViewBag.bairro_cob = edicao["bairro_cob"];
                ViewBag.cep_cob = edicao["cep_cob"];
                ViewBag.id_cidade = int.Parse(edicao["id_cidade"]);
                ViewBag.nome_cidade = edicao["nome_cidade"];
                ViewBag.id_cidade_cob = int.Parse(edicao["id_cidade_cob"]);
                ViewBag.nome_cidade_cob = edicao["nome_cidade_cob"];
                ViewBag.id_qualificacao = int.Parse(edicao["id_qualificacao"]);
            }

            return View("EditarEmpresas");
        }

        public IActionResult IncluirEmpresas(string responseStr, string responseStatusCode, string inclusao_empresa)
        {
            NovoRegistro _novaEmpresa = new NovoRegistro();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _novaEmpresa = JsonConvert.DeserializeObject<NovoRegistro>(responseStr);
            }

            if (qualificacao == null)
            {
                qualificacao = new Qualificacao();
                qualificacao = ConsultarQualificacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);

            ViewBag.qualificacoes = qualificacao.Conteudo;

            ViewBag.id = (_novaEmpresa == null || _novaEmpresa.Conteudo == null ? 0 : _novaEmpresa.Conteudo.Id);
            
            ViewBag.erro = (_novaEmpresa == null  || _novaEmpresa.Resultado == null ? false : _novaEmpresa.Resultado.Erro);
            ViewBag.mensagem = (_novaEmpresa == null || _novaEmpresa.Resultado == null ? "" : _novaEmpresa.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> inclusao = JsonConvert.DeserializeObject<Dictionary<string, string>>(inclusao_empresa);
                ViewBag.fantasia = inclusao["fantasia"];
                ViewBag.razao_social = inclusao["razao_social"];
                ViewBag.cnpj = inclusao["cnpj"];
                ViewBag.insc_est = inclusao["insc_est"];
                ViewBag.url = inclusao["url"];
                ViewBag.logo = inclusao["logo"];
                ViewBag.email_com = inclusao["email_com"];
                ViewBag.email_sac = inclusao["email_sac"];
                ViewBag.fone_com = inclusao["fone_com"];
                ViewBag.fone_sac = inclusao["fone_sac"];
                ViewBag.endereco = inclusao["endereco"];
                ViewBag.numero = inclusao["numero"];
                ViewBag.complemento = inclusao["complemento"];
                ViewBag.bairro = inclusao["bairro"];
                ViewBag.cep = inclusao["cep"];
                ViewBag.endereco_cob = inclusao["endereco_cob"];
                ViewBag.numero_cob = inclusao["numero_cob"];
                ViewBag.complemento_cob = inclusao["complemento_cob"];
                ViewBag.bairro_cob = inclusao["bairro_cob"];
                ViewBag.cep_cob = inclusao["cep_cob"];
                ViewBag.id_cidade = int.Parse(inclusao["id_cidade"]);
                ViewBag.nome_cidade = inclusao["nome_cidade"];
                ViewBag.id_cidade_cob = int.Parse(inclusao["id_cidade_cob"]);
                ViewBag.nome_cidade_cob = inclusao["nome_cidade_cob"];
                ViewBag.id_qualificacao = int.Parse(inclusao["id_qualificacao"]);
            }

            return View("IncluirEmpresas");
        }

        public IActionResult ConsultarEmpresas(string responseStr, string responseStrExc, string responseStatusCode)
        {
            Empresa _Empresa = new Empresa();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _Empresa = JsonConvert.DeserializeObject<Empresa>(responseStr);
                ViewBag.erro = (_Empresa == null || _Empresa.Resultado == null ? false : _Empresa.Resultado.Erro);
                ViewBag.mensagem = (_Empresa == null || _Empresa.Resultado == null ? "" : _Empresa.Resultado.Mensagem);
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
            ViewBag.Empresas = (_Empresa == null || _Empresa.Conteudo == null) ? new List<Empresa.Dados>() : _Empresa.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarEmpresas");
        }

        public Task<IActionResult> AcaoEditarEmpresas(int id)
        {
            return ConsultarEmpresasIn(false, true, id);
        }

        [HttpPost]
        public async Task<IActionResult>IncluirEmpresasPost( string fantasia, string razao_social, string cnpj, string insc_est, string url, string email_com, 
                                                            string email_sac, string fone_com, string fone_sac, string endereco, string numero, 
                                                            string complemento, string bairro, string cep, string endereco_cob, string numero_cob, string complemento_cob, 
                                                            string bairro_cob, string cep_cob, int id_cidade, string nome_cidade, int id_cidade_cob, string nome_cidade_cob, 
                                                            int id_qualificacao, string logo)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var novo_empresa = new Dictionary<string, string>
            {
                { "fantasia", fantasia },
                { "razao_social", razao_social },
                { "cnpj", cnpj },
                { "insc_est", insc_est },
                { "url", url },
                { "email_com", email_com },
                { "email_sac", email_sac },
                { "fone_com", fone_com },
                { "fone_sac", fone_sac },
                { "endereco", endereco },
                { "numero", numero },
                { "complemento", complemento },
                { "bairro", bairro },
                { "cep", cep },
                { "endereco_cob", endereco_cob },
                { "numero_cob", numero_cob },
                { "complemento_cob", complemento_cob },
                { "bairro_cob", bairro_cob },
                { "cep_cob", cep_cob },
                { "id_cidade", id_cidade.ToString() },
                { "id_cidade_cob", id_cidade_cob.ToString() },
                { "nome_cidade", nome_cidade },
                { "nome_cidade_cob", nome_cidade_cob },
                { "logo", logo},
                { "id_qualificacao", id_qualificacao.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(novo_empresa);

            var response = httpClient.client.PostAsync("api/incluir_empresa", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return IncluirEmpresas(responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(novo_empresa));
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

        public Task<IActionResult> RefazerUltimaConsultaEmpresa()
        {
           return ConsultarEmpresasIn(true);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarEmpresasPost( int id, string fantasia, string razao_social, string cnpj, string endereco, string uf, int id_cidade)
        {
            return await ConsultarEmpresasIn(false, false, id, fantasia, razao_social, cnpj, endereco, uf, id_cidade);
        }

        private async Task<IActionResult> ConsultarEmpresasIn(bool refazerultimaconsulta = false, bool editando = false, int id = 0, string fantasia = "", string razao_social = "", 
                                                             string cnpj = "", string endereco = "", string uf = "", int id_cidade = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_empresa = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "fantasia", fantasia },
                { "razao_social", razao_social },
                { "cnpj", cnpj },
                { "endereco", endereco },
                { "uf", uf},
                { "id_cidade", id_cidade.ToString()},
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            
            if (refazerultimaconsulta)
            {
                consulta_empresa = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_empresa);

            var response = httpClient.client.PostAsync("api/consultar_empresa", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
         
            if (response.IsSuccessStatusCode)
            {
                if (!editando)
                {
                    Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_empresa), HttpContext);
                    return ConsultarEmpresas(responseStr, "", response.StatusCode.ToString());
                }
                else
                {
                    return EditarEmpresas(id, responseStr, response.StatusCode.ToString());
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

      
        public async Task<IActionResult> ExcluirEmpresas(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_empresa = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
          };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_empresa);

            var response = httpClient.client.PostAsync("api/excluir_empresa", request.Content).Result;

            string responseStrExc = await response.Content.ReadAsStringAsync();
         
            if (response.IsSuccessStatusCode)
            {
                return ConsultarEmpresas("", responseStrExc, response.StatusCode.ToString());
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
        public async Task<IActionResult> EditarEmpresasPost( int id, string fantasia, string razao_social, string cnpj, string insc_est, string url, string email_com,
                                                             string email_sac, string fone_com, string fone_sac, string endereco, string numero,
                                                             string complemento, string bairro, string cep, string endereco_cob, string numero_cob, string complemento_cob,
                                                             string bairro_cob, string cep_cob, int id_cidade, string nome_cidade, int id_cidade_cob, string nome_cidade_cob,
                                                             string logo, int id_qualificacao)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var edt_empresa = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "fantasia", fantasia },
                { "razao_social", razao_social },
                { "cnpj", cnpj },
                { "insc_est", insc_est },
                { "url", url }, 
                { "email_com", email_com },
                { "email_sac", email_sac },
                { "fone_com", fone_com },
                { "fone_sac", fone_sac },
                { "endereco", endereco },
                { "numero", numero },
                { "complemento", complemento },
                { "bairro", bairro },
                { "cep", cep },
                { "endereco_cob", endereco_cob },
                { "numero_cob", numero_cob },
                { "complemento_cob", complemento_cob },
                { "bairro_cob", bairro_cob },
                { "cep_cob", cep_cob },
                { "id_cidade", id_cidade.ToString() },
                { "nome_cidade", nome_cidade },
                { "id_cidade_cob", id_cidade_cob.ToString() },
                { "nome_cidade_cob", nome_cidade_cob },
                { "logo",logo },
                { "id_qualificacao", id_qualificacao.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(edt_empresa);

            var response = httpClient.client.PostAsync("api/atualizar_empresa", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
           
            if (response.IsSuccessStatusCode)
            {
                return EditarEmpresas(0, responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(edt_empresa));
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

        public IActionResult ConsultarEmpresasAutoCompletar(string razao_social)
        {
            if (!string.IsNullOrWhiteSpace(razao_social))
            {
                DwClienteHttp httpClient = DwClienteHttp.Instance;
                httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

                string responseStr;
                string responseStatusCode;

                var consulta_empresa = new Dictionary<string, string>
                {
                   { "razao_social", razao_social},
                   { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
                };

                var request = new HttpRequestMessage();
                request.Content = new FormUrlEncodedContent(consulta_empresa);
                HttpResponseMessage response;

                try
                {
                    response = httpClient.client.PostAsync("api/consultar_empresa", request.Content).Result;
                }
                catch
                {
                    return new JsonResult("");
                }


                responseStr = response.Content.ReadAsStringAsync().Result;

                responseStatusCode = response.StatusCode.ToString();

                if (!string.IsNullOrWhiteSpace(responseStr))
                {
                    return new JsonResult(JsonConvert.DeserializeObject<Empresa>(responseStr).Conteudo);
                }
                else
                {
                    return new JsonResult("");
                }
            }
            else
            {
                return new JsonResult("");
            }
        }

        private Qualificacao ConsultarQualificacoes()
        {

            string responseStr;
            string responseStatusCode;

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_qualificacao = new Dictionary<string, string>
            {
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_qualificacao);

            var response = httpClient.client.PostAsync("api/consultar_qualificacao", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;
            responseStatusCode = response.StatusCode.ToString();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                return JsonConvert.DeserializeObject<Qualificacao>(responseStr);
            }
            else
            {
                return null;
            }
        }


    }
}