using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class GrpProdutosController : Controller
    {
        GrpProdutosRepository grpprodutosRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public GrpProdutosController(GrpProdutosRepository _grpprodutosRepository)
        {
            grpprodutosRepository = _grpprodutosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_grpprodutosRepository.GetConfiguration());
        }

        [Route("consultar_todos_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "app,bki,tin")]
        public ActionResult Consultar_todos_grp_produto(string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_todos_grp_produto"))
            {
                return Unauthorized();
            }

            var result = grpprodutosRepository.Consultar_todos_grp_produto();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("consultar_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bki,bko,fta,fto,tin")]
        public ActionResult Consultar_grp_produto(int id = 0, string descricao = "", int id_situacao = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_grp_produto"))
            {
                return Unauthorized();
            }

            var result = grpprodutosRepository.Consultar_grp_produto(id, descricao, id_situacao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("consultar_ultima_atualizacao_produtos")]
        [HttpPost]
        [Authorize(Roles = "app,bki,tin")]
        public ActionResult Consultar_ultima_atualizacao_produto(string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_ultima_atualizacao_produtos"))
            {
                return Unauthorized();
            }

            var result = grpprodutosRepository.Consultar_ultima_atualizacao_produtos();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public ActionResult Incluir_grp_produto(string descricao = "", string icone = "", int ordem = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_grp_produto"))
            {
                return Unauthorized();
            }

            var result = grpprodutosRepository.Incluir_grp_produto(descricao, icone, ordem, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public ActionResult Excluir_grp_produto(int id = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/excluir_grp_produto"))
            {
                return Unauthorized();
            }

            var result = grpprodutosRepository.Excluir_grp_produto(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public ActionResult Atualizar_grp_produto(int id = 0, string descricao = "", string icone = "", int id_situacao = 0, int ordem = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_grp_produto"))
            {
                return Unauthorized();
            }

            var result = grpprodutosRepository.Atualizar_grp_produto(id, descricao, id_situacao, icone, ordem, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}