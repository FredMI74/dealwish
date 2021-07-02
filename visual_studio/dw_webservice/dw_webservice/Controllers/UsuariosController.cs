using dw_webservice.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class UsuariosController : Controller
    {
        readonly UsuariosRepository usuariosRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public UsuariosController(UsuariosRepository _usuariosRepository)
        {
            usuariosRepository = _usuariosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_usuariosRepository.GetConfiguration());
        }

        [Route("consultar_usuario")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,bko,fta,fto,tin")]
        public async Task<ActionResult> Consultar_usuario(long id = 0, string email = "", string nome = "", string cpf = "", string aplicativo = "",
                                              string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, string uf = "", int id_empresa = 0, string origem = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_usuario");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.ConsultarUsuarioAsync(id, email, nome, cpf, aplicativo, retaguarda, empresa, id_situacao, id_cidade_ap, uf, id_empresa, origem);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_usuario")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Incluir_usuario(string email = "", string senha1 = "", string senha2 = "", string nome = "", string data_nasc = "", string cpf = "", string aplicativo = "",
                                            string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, int id_empresa = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_usuario", aplicativo == "S");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.IncluirUsuarioAsync(email, senha1, senha2, nome, data_nasc, cpf, aplicativo, retaguarda, empresa, id_situacao, id_cidade_ap, id_empresa, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_usuario")]
        [HttpPost]
        [Authorize(Roles = "app,bka,fta,tin")]
        public async Task<ActionResult> Excluir_usuario(long id = 0, string token = "", string origem = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_usuario");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.ExcluirUsuarioAsync(id, loginUsuario.Id, origem);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_usuario")]
        [HttpPost]
        [Authorize(Roles = "app,bka,fta,tin")]
        public async Task<ActionResult> Atualizar_usuario(long id = 0, string email = "", string nome = "", string data_nasc = "", string cpf = "", string aplicativo = "",
                                              string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, int id_empresa = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_usuario");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.AtualizarUsuarioAsync(id, email, nome, data_nasc, cpf, aplicativo, retaguarda, empresa, id_situacao, id_cidade_ap, id_empresa, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("reiniciar_senha")]
        [HttpPost]
        [Authorize(Roles = "bka,bko,fta,fto,tin")]
        public async Task<ActionResult> Reiniciar_senha(string email = "", string cpf = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/reiniciar_senha", true);
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.ReiniciarSenhaAsync(email, cpf, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("trocar_senha")]
        [HttpPost]
        [Authorize(Roles = "app,bka,bkc,bkf,bki,bko,fta,fto,tin")]
        public async Task<ActionResult> Trocar_senha(string email = "", string senha_atual = "", string senha_nova = "", string senha_nova_conf = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/trocar_senha");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.TrocarSenhaAsync(email, senha_atual, senha_nova, senha_nova_conf, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("login_usuario")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login_usuario(string dados = "", string token = "", string origem = "", string token_app = "")
        {
            var values = dados.Split(';');

            string email = Encoding.UTF8.GetString(Convert.FromBase64String(values[0])); 
            string senha = Encoding.UTF8.GetString(Convert.FromBase64String(values[1]));
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/login_usuario", true);
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.LoginUsuarioAsync( email, senha, origem, token_app);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("incluir_grp_permissao_usuario")]
        [HttpPost]
        [Authorize(Roles = "bka,fta,tin")]
        public async Task<ActionResult> Incluir_grp_permissao_usuario(long id_usuario = 0, int id_grp_permissao = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_grp_permissao_usuario");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.IncluirGrpPermissaoUsuarioAsync(id_usuario, id_grp_permissao, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_grp_permissao_usuario")]
        [HttpPost]
        [Authorize(Roles = "bka,fta,tin")]
        public async Task<ActionResult> Excluir_grp_permissao_usuario(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_grp_permissao_usuario");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.ExcluirGrpPermissaoUsuarioAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("consultar_permissoes_usuario")]
        [HttpPost]
        [Authorize(Roles = "bki,tin")]
        public async Task<ActionResult> Consultar_permissoes_usuario(long id_usuario = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_permissoes_usuario");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.ConsultarPermissoesUsuarioAsync(id_usuario);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("consultar_grp_permissoes_usuario")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,fta,tin")]
        public async Task<ActionResult> Consultar_grp_permissoes_usuario(long id_usuario = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_grp_permissoes_usuario");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.ConsultarGrpPermissoesUsuarioAsync(id_usuario);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("consultar_termo_servico")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Consultar_termo_servico(string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_termo_servico", true);
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await usuariosRepository.ConsultarTermoServicoAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}