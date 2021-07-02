using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class GrpProdutosController : Controller
    {
        readonly GrpProdutosRepository grpprodutosRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public GrpProdutosController(GrpProdutosRepository _grpprodutosRepository)
        {
            grpprodutosRepository = _grpprodutosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_grpprodutosRepository.GetConfiguration());
        }

        [Route("consultar_todos_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "app,bki,tin")]
        public async Task<ActionResult> Consultar_todos_grp_produto(string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_todos_grp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await grpprodutosRepository.ConsultarTodosGrpProdutoAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("consultar_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bki,bko,fta,fto,tin")]
        public async Task<ActionResult> Consultar_grp_produto(int id = 0, string descricao = "", int id_situacao = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_grp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await grpprodutosRepository.ConsultarGrpProdutoAsync(id, descricao, id_situacao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("consultar_ultima_atualizacao_produtos")]
        [HttpPost]
        [Authorize(Roles = "app,bki,tin")]
        public async Task<ActionResult> Consultar_ultima_atualizacao_produto(string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_ultima_atualizacao_produtos");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await grpprodutosRepository.ConsultarUltimaAtualizacaoProdutosAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<ActionResult> Incluir_grp_produto(string descricao = "", string icone = "", int ordem = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_grp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await grpprodutosRepository.IncluirGrpProdutoAsync(descricao, icone, ordem, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<ActionResult> Excluir_grp_produto(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_grp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await grpprodutosRepository .ExcluirGrpProdutoAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_grp_produto")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<ActionResult> Atualizar_grp_produto(int id = 0, string descricao = "", string icone = "", int id_situacao = 0, int ordem = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_grp_produto");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await grpprodutosRepository .AtualizarGrpProdutoAsync(id, descricao, id_situacao, icone, ordem, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}