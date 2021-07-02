using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class TpProdutosController : Controller
    {
        readonly TpProdutosRepository tpprodutosRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public TpProdutosController(TpProdutosRepository _tpprodutosRepository)
        {
            tpprodutosRepository = _tpprodutosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_tpprodutosRepository.GetConfiguration());
        }

        [Route("consultar_tp_produto")]
        [HttpPost]
        [Authorize(Roles = "app,bka,bkc,bki,bko,fta,fto,tin")]
        public async Task<ActionResult> Consultar_tp_produto(int id = 0, string descricao = "", int id_grp_prod = 0, int id_situacao = 0, string token = "", string paginacao = "N", int max_id = 0, int pagina = 1)
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_tp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await tpprodutosRepository.ConsultarTpProdutoAsync(id, descricao, id_grp_prod, id_situacao, paginacao, max_id, pagina);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_tp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<ActionResult> Incluir_tp_produto(string descricao = "", int id_grp_prod = 0, string icone = "", string preenchimento = "", int ordem = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_tp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await tpprodutosRepository.IncluirTpProdutoAsync(descricao, id_grp_prod, preenchimento, icone, ordem, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_tp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<ActionResult> Excluir_tp_produto(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_tp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await tpprodutosRepository.ExcluirTpProdutoAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_tp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<ActionResult> Atualizar_tp_produto(int id = 0, string descricao = "", int id_grp_prod = 0, int id_situacao = 0, string preenchimento = "", string icone = "", int ordem = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_tp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await tpprodutosRepository.AtualizarTpProdutoAsync(id, descricao, id_grp_prod, id_situacao, preenchimento, icone, ordem, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}