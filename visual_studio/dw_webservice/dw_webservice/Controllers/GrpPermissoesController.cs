using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class GrpPermissoesController : Controller
    {
        GrpPermissoesRepository grppermissoesRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public GrpPermissoesController(GrpPermissoesRepository _grppermissoesRepository)
        {
            grppermissoesRepository = _grppermissoesRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_grppermissoesRepository.GetConfiguration());
        }

        [Route("consultar_grp_permissao")]
        [HttpPost]
        [Authorize(Roles = "bka,bkf,bki,fta,tin")]
        public ActionResult Consultar_grp_permissao(int id = 0, string descricao = "", string origem = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/consultar_grp_permissao"))
            {
                return Unauthorized();
            }

            var result = grppermissoesRepository.Consultar_grp_permissao(id, id_usuario_login, descricao, origem);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("consultar_permissao_grupo")]
        [HttpPost]
        [Authorize(Roles = "bki,tin")]
        public ActionResult Consultar_permissao_grupo(int id_grp_permissao = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_permissao_grupo"))
            {
                return Unauthorized();
            }

            var result = grppermissoesRepository.Consultar_permissao_grupo(id_grp_permissao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}