using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class TpProdutosController : Controller
    {
        TpProdutosRepository tpprodutosRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public TpProdutosController(TpProdutosRepository _tpprodutosRepository)
        {
            tpprodutosRepository = _tpprodutosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_tpprodutosRepository.GetConfiguration());
        }

        [Route("consultar_tp_produto")]
        [HttpPost]
        [Authorize(Roles = "app,bka,bkc,bki,bko,fta,fto,tin")]
        public ActionResult Consultar_tp_produto(int id = 0, string descricao = "", int id_grp_prod = 0, int id_situacao = 0, string token = "", string paginacao = "N", int max_id = 0, int pagina = 1)
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_tp_produto"))
            {
                return Unauthorized();
            }

            var result = tpprodutosRepository.Consultar_tp_produto(id, descricao, id_grp_prod, id_situacao, paginacao, max_id, pagina);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_tp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public ActionResult Incluir_tp_produto(string descricao = "", int id_grp_prod = 0, string icone = "", string preenchimento = "", int ordem = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_tp_produto"))
            {
                return Unauthorized();
            }

            var result = tpprodutosRepository.Incluir_tp_produto(descricao, id_grp_prod, preenchimento, icone, ordem, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_tp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public ActionResult Excluir_tp_produto(int id = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/excluir_tp_produto"))
            {
                return Unauthorized();
            }

            var result = tpprodutosRepository.Excluir_tp_produto(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_tp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public ActionResult Atualizar_tp_produto(int id = 0, string descricao = "", int id_grp_prod = 0, int id_situacao = 0, string preenchimento = "", string icone = "", int ordem = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_tp_produto"))
            {
                return Unauthorized();
            }

            var result = tpprodutosRepository.Atualizar_tp_produto(id, descricao, id_grp_prod, id_situacao, preenchimento, icone, ordem, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}