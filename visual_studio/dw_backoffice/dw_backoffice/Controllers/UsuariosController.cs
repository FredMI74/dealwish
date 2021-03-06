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
using System;

namespace dw_backoffice.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private Situacao situacao;
        private GrpPermissoes grppermissoes;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AtualizarSenhaUsuarios(int id_usuario, string responseStrExc, string responseStatusCode)
        {
            ProcessarRegistro _processarregistro = new ProcessarRegistro();

            if (!string.IsNullOrWhiteSpace(responseStrExc))
            {
                _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStrExc);
                ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
                ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            }
            else
            {
                ViewBag.erro = false;
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id_usuario;

            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            return View("AtualizarSenhaUsuarios");

        }

        public IActionResult EditarUsuarios(int id, string responseStr, string responseStatusCode, string edt_usuarios = "")
        {
            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            Usuario _usuario = new Usuario();

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
                    _usuario = JsonConvert.DeserializeObject<Usuario>(responseStr, settings);
                }
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.nome = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Nome;
            ViewBag.email = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Email;
            ViewBag.data_nasc = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Data_nasc.ToString("yyyy-MM-dd");
            ViewBag.cpf = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Cpf;
            ViewBag.aplicativo = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Aplicativo;
            ViewBag.retaguarda = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Retaguarda;
            ViewBag.empresa = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Empresa;

            ViewBag.id_situacao = (_usuario == null || _usuario.Conteudo == null) ? 0 : _usuario.Conteudo[0].Id_situacao;
            ViewBag.id_cidade_ap = (_usuario == null || _usuario.Conteudo == null) ? 0 : _usuario.Conteudo[0].Id_cidade_ap;
            ViewBag.nome_cidade_ap = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Nome_cidade_ap;
            ViewBag.id_empresa = (_usuario == null || _usuario.Conteudo == null) ? 0 : _usuario.Conteudo[0].Id_empresa;
            ViewBag.razao_social = (_usuario == null || _usuario.Conteudo == null) ? "" : _usuario.Conteudo[0].Razao_social;

            ViewBag.situacoes = situacao.Conteudo;

            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> edicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(edt_usuarios);
                ViewBag.id = long.Parse(edicao["id"]);
                ViewBag.email = edicao["email"];
                ViewBag.nome = edicao["nome"];
                ViewBag.data_nasc = DateTime.Parse(edicao["data_nasc"]).ToString("yyyy-MM-dd");
                ViewBag.cpf = edicao["cpf"];
                ViewBag.aplicativo = edicao["aplicativo"];
                ViewBag.retaguarda = edicao["retaguarda"];
                ViewBag.empresa = edicao["empresa"];
                ViewBag.id_situacao = int.Parse(edicao["id_situacao"]);
                ViewBag.id_cidade_ap = int.Parse(edicao["id_cidade_ap"]);
                ViewBag.nome_cidade_ap = edicao["nome_cidade_ap"];
                ViewBag.id_empresa = int.Parse(edicao["id_empresa"]);
                ViewBag.razao_social = edicao["razao_social"];
            }

            return View("EditarUsuarios");

        }

        public IActionResult IncluirUsuarios(string responseStr, string responseStatusCode, string inclusao_usuario)
        {
            NovoRegistro _novousuario = new NovoRegistro();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _novousuario = JsonConvert.DeserializeObject<NovoRegistro>(responseStr);
            }

            if (situacao == null)
            {
                situacao = new Situacao();
                situacao = ConsultarSituacoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = (_novousuario == null || _novousuario.Conteudo == null ? 0 : _novousuario.Conteudo.Id);

            ViewBag.situacoes = situacao.Conteudo;

            ViewBag.erro = (_novousuario == null || _novousuario.Resultado == null ? false : _novousuario.Resultado.Erro);
            ViewBag.mensagem = (_novousuario == null || _novousuario.Resultado == null ? "" : _novousuario.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            if (ViewBag.erro)
            {
                Dictionary<string, string> inclusao = JsonConvert.DeserializeObject<Dictionary<string, string>>(inclusao_usuario);
                ViewBag.email = inclusao["email"];
                ViewBag.nome = inclusao["nome"];
                ViewBag.data_nasc = DateTime.Parse(inclusao["data_nasc"]).ToString("yyyy-MM-dd");
                ViewBag.cpf = inclusao["cpf"];
                ViewBag.aplicativo = inclusao["aplicativo"];
                ViewBag.retaguarda = inclusao["retaguarda"];
                ViewBag.empresa = inclusao["empresa"];
                ViewBag.senha1 = inclusao["senha1"];
                ViewBag.senha2 = inclusao["senha2"];
                ViewBag.id_situacao = int.Parse(inclusao["id_situacao"]);
                ViewBag.id_cidade_ap = int.Parse(inclusao["id_cidade_ap"]);
                ViewBag.nome_cidade_ap = inclusao["nome_cidade_ap"];
                ViewBag.id_empresa = int.Parse(inclusao["id_empresa"]);
                ViewBag.razao_social = inclusao["razao_social"];

            }

            return View("IncluirUsuarios");
        }

        public IActionResult ConsultarUsuarios(string acao, string responseStr, string responseStrExc, string responseStatusCode)
        {
            ViewBag.acao = acao;

            Usuario _usuario = new Usuario();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                _usuario = JsonConvert.DeserializeObject<Usuario>(responseStr, settings);
                ViewBag.erro = (_usuario == null || _usuario.Resultado == null ? false : _usuario.Resultado.Erro);
                ViewBag.mensagem = (_usuario == null || _usuario.Resultado == null ? "" : _usuario.Resultado.Mensagem);
            }
            else
            {
                ViewBag.erro = false;
            }

            if (!string.IsNullOrWhiteSpace(responseStrExc))
            {
                ProcessarRegistro _proc = new ProcessarRegistro();
                _proc = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStrExc);
                ViewBag.erro = (_proc == null || _proc.Resultado == null ? false : _proc.Resultado.Erro);
                ViewBag.mensagem = (_proc == null || _proc.Resultado == null ? "" : _proc.Resultado.Mensagem);
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
            ViewBag.usuarios = (_usuario == null || _usuario.Conteudo == null) ? new List<Usuario.Dados>() : _usuario.Conteudo;
            ViewBag.id_usuario_logado = int.Parse(Request.Cookies[Constantes.ID_USUARIO]);
            ViewBag.situacoes = situacao.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarUsuarios");
        }

        public IActionResult GrupoPermissoesUsuarios(int id_usuario, string nome, string email, string responseStr, string responseStrGrpPermUsr, string responseStatusCode)
        {

            GrpPermissoesUsuario _grp_perm_usuario = new GrpPermissoesUsuario();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                _grp_perm_usuario = JsonConvert.DeserializeObject<GrpPermissoesUsuario>(responseStr, settings);
            }

            ProcessarRegistro _processarregistro = new ProcessarRegistro();

            if (!string.IsNullOrWhiteSpace(responseStrGrpPermUsr))
            {
                _processarregistro = JsonConvert.DeserializeObject<ProcessarRegistro>(responseStrGrpPermUsr);
            }


            if (grppermissoes == null)
            {
                grppermissoes = new GrpPermissoes();
                grppermissoes = ConsultarGrpPermissoes();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id_usuario = id_usuario;
            ViewBag.nome = nome;
            ViewBag.email = email;

            ViewBag.grppermissoes = grppermissoes.Conteudo;

            ViewBag.grps_perms_usr = (_grp_perm_usuario == null || _grp_perm_usuario.Conteudo == null) ? new List<GrpPermissoesUsuario.Dados>() : _grp_perm_usuario.Conteudo;

            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);
            ViewBag.responseStatusCode = responseStatusCode;

            return View("GrupoPermissoesUsuarios");
        }


        [HttpPost]
        public async Task<IActionResult> IncluirUsuariosPost( string email, string nome, DateTime data_nasc, string cpf, string aplicativo, string retaguarda, string empresa,
                                                                int id_situacao, int id_cidade_ap, string nome_cidade_ap, int id_empresa, string razao_social, string senha1, string senha2)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var novo_usuario = new Dictionary<string, string>
            {
                { "email", email },
                { "nome", nome },
                { "data_nasc", data_nasc.ToString()},
                { "cpf", cpf },
                { "aplicativo", aplicativo ?? Constantes.NAO},
                { "retaguarda", retaguarda ?? Constantes.NAO},
                { "empresa", empresa ?? Constantes.NAO},
                { "senha1", senha1 },
                { "senha2", senha2 },
                { "id_situacao", id_situacao.ToString() },
                { "id_cidade_ap", id_cidade_ap.ToString() },
                { "nome_cidade_ap", nome_cidade_ap },
                { "id_empresa", id_empresa.ToString() },
                { "razao_social", razao_social },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(novo_usuario);

            var response = httpClient.client.PostAsync("api/incluir_usuario", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return IncluirUsuarios(responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(novo_usuario));
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

        public Task<IActionResult> RefazerUltimaConsultaUsuario()
        {
            return ConsultarUsuariosIn(true);
        }

        public Task<IActionResult> AcaoEditarUsuarios(int id)
        {
            return ConsultarUsuariosIn(false, true, id);
        }

        public Task<IActionResult> AcaoGrupoPermissoesUsuarios(int id_usuario, string nome, string email, string responseStrGrpPermUsr)
        {
            return ConsultarGrpPermissoesUsuarioIn(false, true, id_usuario, nome, email, responseStrGrpPermUsr);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarUsuariosPost( int id, string email, string nome, string cpf, string aplicativo, string retaguarda, string empresa,
                                                                int id_situacao, string uf = "", int id_cidade_ap = 0, int id_empresa = 0)
        {
            return await ConsultarUsuariosIn(false, false, id, email, nome, cpf, aplicativo, retaguarda, empresa, id_situacao, uf, id_cidade_ap, id_empresa);
        }

        private async Task<IActionResult> ConsultarUsuariosIn(bool refazerultimaconsulta = false, bool editando = false, int id = 0, string email = "", string nome ="", string cpf= "", 
                                                            string aplicativo = "", string retaguarda = "", string empresa = "", int id_situacao =0, string uf = "", int id_cidade_ap = 0, int id_empresa = 0)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_usuario = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "email", email },
                { "nome", nome },
                { "cpf", cpf },
                { "aplicativo", aplicativo },
                { "retaguarda", retaguarda },
                { "empresa", empresa },
                { "id_situacao", id_situacao.ToString() },
                { "uf", uf},
                { "id_cidade_ap", id_cidade_ap.ToString() },
                { "id_empresa", id_empresa.ToString() },
                { "origem", Constantes.BACKOFFICE },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };
            if (refazerultimaconsulta)
            {
                consulta_usuario = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA]);
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_usuario);

            var response = httpClient.client.PostAsync("api/consultar_usuario", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (editando)
                {
                    return EditarUsuarios(id, responseStr, response.StatusCode.ToString());
                }
                else
                {
                    Utils.SetCookie(Constantes.ULTIMA_CONSULTA, JsonConvert.SerializeObject(consulta_usuario), HttpContext);
                    return ConsultarUsuarios("", responseStr, "", response.StatusCode.ToString());
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

        public async Task<IActionResult> ExcluirUsuarios(int id)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_usuario = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO]}
            };
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_usuario);

            var response = httpClient.client.PostAsync("api/excluir_usuario", request.Content).Result;
            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarUsuarios("E", "", responseStrExc, response.StatusCode.ToString());
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

        public async Task<IActionResult> ReiniciarSenhaUsuarios(string email)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var email_usuario = new Dictionary<string, string>
            {
                { "email", email },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO]}
            };
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(email_usuario);

            var response = httpClient.client.PostAsync("api/reiniciar_senha", request.Content).Result;
            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarUsuarios("R", "", responseStrExc, response.StatusCode.ToString());
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
        public async Task<IActionResult> EditarUsuariosPost( int id, string email, string nome, DateTime data_nasc, string cpf, string aplicativo, string retaguarda, string empresa,
                                                                int id_situacao, int id_cidade_ap, string nome_cidade_ap, int id_empresa, string razao_social)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var edt_usuario = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "email", email },
                { "nome", nome },
                { "data_nasc", data_nasc.ToString()},
                { "cpf", cpf },
                { "aplicativo", aplicativo ?? Constantes.NAO},
                { "retaguarda", retaguarda ?? Constantes.NAO},
                { "empresa", empresa ?? Constantes.NAO },
                { "id_situacao", id_situacao.ToString() },
                { "id_cidade_ap", id_cidade_ap.ToString() },
                { "nome_cidade_ap", nome_cidade_ap },
                { "id_empresa", id_empresa.ToString() },
                { "razao_social", razao_social },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(edt_usuario);

            var response = httpClient.client.PostAsync("api/atualizar_usuario", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return EditarUsuarios(0, responseStr, response.StatusCode.ToString(), JsonConvert.SerializeObject(edt_usuario));
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
        public async Task<IActionResult> ConsultarGrpPermissoesUsuarioPost( int id_usuario)
        {
            return await ConsultarGrpPermissoesUsuarioIn(false, false, id_usuario);
        }

        private async Task<IActionResult> ConsultarGrpPermissoesUsuarioIn(bool refazerultimaconsulta = false, bool editando = false, int id_usuario = 0, string nome = "", string email = "",
                                                                          string responseStrGrpPermUsr = "")
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_grp_permissao_usuario = new Dictionary<string, string>
            {
                { "id_usuario", id_usuario.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO]}
            };


            if (refazerultimaconsulta)
            {
                consulta_grp_permissao_usuario = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.Cookies[Constantes.ULTIMA_CONSULTA_AUX]);
            }

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_grp_permissao_usuario);

            var response = httpClient.client.PostAsync("api/consultar_grp_permissoes_usuario", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            Utils.SetCookie(Constantes.ULTIMA_CONSULTA_AUX, JsonConvert.SerializeObject(consulta_grp_permissao_usuario), HttpContext);

            if (response.IsSuccessStatusCode)
            {
                return GrupoPermissoesUsuarios(id_usuario, nome, email, responseStr, responseStrGrpPermUsr, response.StatusCode.ToString());
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
        public async Task<IActionResult> AtualizarSenhaUsuariosPost(int id, string senha_atual, string senha_nova, string senha_nova_conf)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var novo_senha_usuario = new Dictionary<string, string>
            {
                { "email",  Request.Cookies[Constantes.EMAIL] },
                { "senha_atual", senha_atual },
                { "senha_nova", senha_nova },
                { "senha_nova_conf", senha_nova_conf },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO]}
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(novo_senha_usuario);

            var response = httpClient.client.PostAsync("api/trocar_senha", request.Content).Result;
            string responseStrExc = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return AtualizarSenhaUsuarios(id, responseStrExc, response.StatusCode.ToString());
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
        public async Task<IActionResult> IncluirGrpPermissoesUsuarioPost(int id_usuario, int id_grp_permissao, string nome, string email)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var novo_grppermusr = new Dictionary<string, string>
            {
                { "id_usuario", id_usuario.ToString() },
                { "id_grp_permissao", id_grp_permissao.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO]}
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(novo_grppermusr);

            var response = httpClient.client.PostAsync("api/incluir_grp_permissao_usuario", request.Content).Result;

            string responseStrGrpPermUsr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return await ConsultarGrpPermissoesUsuarioIn(true, false, id_usuario, nome, email);
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


        public async Task<IActionResult> ExcluirGrpPermissao(int id, int id_usuario, string nome, string email)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var excluir_grppermusr = new Dictionary<string, string>
          {
              { "id", id.ToString() },
              {"token", Request.Cookies[Constantes.TOKEN_USUARIO]}
          };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(excluir_grppermusr);

            var response = httpClient.client.PostAsync("api/excluir_grp_permissao_usuario", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return await ConsultarGrpPermissoesUsuarioIn(true, false, id_usuario, nome, email, responseStr);
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
                { "usuarios", Constantes.SIM },
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

        private GrpPermissoes ConsultarGrpPermissoes()
        {

            string responseStr;
            string responseStatusCode;

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_grp_permissoes = new Dictionary<string, string>
            {
                { "descricao", "%" },
                { "origem", Constantes.BACKOFFICE },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_grp_permissoes);

            var response = httpClient.client.PostAsync("api/consultar_grp_permissao", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;
            responseStatusCode = response.StatusCode.ToString();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                return JsonConvert.DeserializeObject<GrpPermissoes>(responseStr);
            }
            else
            {
                return null;
            }
        }

    }
}