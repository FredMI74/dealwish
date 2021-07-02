using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class GrpPermissoesController : Controller
    {
        readonly GrpPermissoesRepository grppermissoesRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public GrpPermissoesController(GrpPermissoesRepository _grppermissoesRepository)
        {
            grppermissoesRepository = _grppermissoesRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_grppermissoesRepository.GetConfiguration());
        }

        [Route("consultar_grp_permissao")]
        [HttpPost]
        [Authorize(Roles = "bka,bkf,bki,fta,tin")]
        public async Task<ActionResult> Consultar_grp_permissao(int id = 0, string descricao = "", string origem = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_grp_permissao");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await grppermissoesRepository.ConsultarGrpPermissaoAsync(id, loginUsuario.Id, descricao, origem);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("consultar_permissao_grupo")]
        [HttpPost]
        [Authorize(Roles = "bki,tin")]
        public async Task<ActionResult> Consultar_permissao_grupo(int id_grp_permissao = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_permissao_grupo");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await grppermissoesRepository.ConsultarPermissaoGrupoAsync(id_grp_permissao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}