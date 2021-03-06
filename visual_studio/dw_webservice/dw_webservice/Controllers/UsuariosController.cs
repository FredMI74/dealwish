using dw_webservice.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class UsuariosController : Controller
    {
        UsuariosRepository usuariosRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public UsuariosController(UsuariosRepository _usuariosRepository)
        {
            usuariosRepository = _usuariosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_usuariosRepository.GetConfiguration());
        }

        [Route("consultar_usuario")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,bko,fta,fto,tin")]
        public ActionResult Consultar_usuario(int id = 0, string email = "", string nome = "", string cpf = "", string aplicativo = "",
                                              string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, string uf = "", int id_empresa = 0, string origem = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_usuario"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Consultar_usuario(id, email, nome, cpf, aplicativo, retaguarda, empresa, id_situacao, id_cidade_ap, uf, id_empresa, origem);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_usuario")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Incluir_usuario(string email = "", string senha1 = "", string senha2 = "", string nome = "", string data_nasc = "", string cpf = "", string aplicativo = "",
                                            string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, int id_empresa = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/consultar_usuario", aplicativo == "S"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Incluir_usuario(email, senha1, senha2, nome, data_nasc, cpf, aplicativo, retaguarda, empresa, id_situacao, id_cidade_ap, id_empresa, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_usuario")]
        [HttpPost]
        [Authorize(Roles = "app,bka,fta,tin")]
        public ActionResult Excluir_usuario(int id = 0, string token = "", string origem = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/excluir_usuario"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Excluir_usuario(id, id_usuario_login, origem);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_usuario")]
        [HttpPost]
        [Authorize(Roles = "app,bka,fta,tin")]
        public ActionResult Atualizar_usuario(int id = 0, string email = "", string nome = "", string data_nasc = "", string cpf = "", string aplicativo = "",
                                              string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, int id_empresa = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_usuario"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Atualizar_usuario(id, email, nome, data_nasc, cpf, aplicativo, retaguarda, empresa, id_situacao, id_cidade_ap, id_empresa, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("reiniciar_senha")]
        [HttpPost]
        [Authorize(Roles = "bka,bko,fta,fto,tin")]
        public ActionResult Reiniciar_senha(string email = "", string cpf = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/reiniciar_senha", true))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Reiniciar_senha(email, cpf, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("trocar_senha")]
        [HttpPost]
        [Authorize(Roles = "app,bka,bkc,bkf,bki,bko,fta,fto,tin")]
        public ActionResult Trocar_senha(string email = "", string senha_atual = "", string senha_nova = "", string senha_nova_conf = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/trocar_senha"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Trocar_senha(email, senha_atual, senha_nova, senha_nova_conf, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("login_usuario")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login_usuario(string dados = "", string token = "", string origem = "", string token_app = "")
        {
            var values = dados.Split(';');

            string email = Encoding.UTF8.GetString(Convert.FromBase64String(values[0])); 
            string senha = Encoding.UTF8.GetString(Convert.FromBase64String(values[1]));

            if (!validarTokenPermissao.Validar(out _, token, "api/login_usuario", true))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Login_usuario( email, senha, origem, token_app);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("incluir_grp_permissao_usuario")]
        [HttpPost]
        [Authorize(Roles = "bka,fta,tin")]
        public ActionResult Incluir_grp_permissao_usuario(int id_usuario = 0, int id_grp_permissao = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_grp_permissao_usuario"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Incluir_grp_permissao_usuario(id_usuario, id_grp_permissao, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_grp_permissao_usuario")]
        [HttpPost]
        [Authorize(Roles = "bka,fta,tin")]
        public ActionResult Excluir_grp_permissao_usuario(int id = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/excluir_grp_permissao_usuario"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Excluir_grp_permissao_usuario(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("consultar_permissoes_usuario")]
        [HttpPost]
        [Authorize(Roles = "bki,tin")]
        public ActionResult Consultar_permissoes_usuario(int id_usuario = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_permissoes_usuario"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Consultar_permissoes_usuario(id_usuario);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("consultar_grp_permissoes_usuario")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,fta,tin")]
        public ActionResult Consultar_grp_permissoes_usuario(int id_usuario = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_grp_permissoes_usuario"))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Consultar_grp_permissoes_usuario(id_usuario);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("consultar_termo_servico")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Consultar_termo_servico(string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_termo_servico", true))
            {
                return Unauthorized();
            }

            var result = usuariosRepository.Consultar_termo_servico();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}