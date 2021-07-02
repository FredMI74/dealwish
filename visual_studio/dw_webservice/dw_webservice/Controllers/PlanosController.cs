using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class PlanosController : Controller
    {
        readonly PlanosRepository planosRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public PlanosController(PlanosRepository _planosRepository)
        {
            planosRepository = _planosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_planosRepository.GetConfiguration());
        }

        [Route("consultar_plano")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,tin")]
        public async Task<ActionResult> Consultar_plano(int id = 0, string descricao = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_plano");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await planosRepository.ConsultarPlanoAsync(id, descricao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_plano")]
        [HttpPost]
        [Authorize(Roles = "bka,bkf,tin")]
        public async Task<ActionResult> Incluir_plano(string descricao = "", int qtd_ofertas = 0, float valor_mensal = 0, float valor_oferta = 0, string visualizacao = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_plano");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await planosRepository.IncluirPlanoAsync(descricao , qtd_ofertas, valor_mensal, valor_oferta, visualizacao, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_plano")]
        [HttpPost]
        [Authorize(Roles = "bka,bkf,tin")]
        public async Task<ActionResult> Excluir_plano(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_plano");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await planosRepository.ExcluirPlanoAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_plano")]
        [HttpPost]
        [Authorize(Roles = "bka,bkf,tin")]
        public async Task<ActionResult> Atualizar_plano(int id = 0, string descricao = "", int qtd_ofertas = 0, float valor_mensal = 0, float valor_oferta = 0, string visualizacao = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_plano");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await planosRepository.AtualizarPlanoAsync(id, descricao, qtd_ofertas, valor_mensal, valor_oferta, visualizacao, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}