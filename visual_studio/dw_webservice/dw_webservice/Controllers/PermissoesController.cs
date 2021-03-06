using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class PermissoesController : Controller
    {
        PermissoesRepository permissoesRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public PermissoesController(PermissoesRepository _permissoesRepository)
        {
            permissoesRepository = _permissoesRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_permissoesRepository.GetConfiguration());
        }

        [Route("consultar_permissao")]
        [HttpPost]
        [Authorize(Roles = "bki,tin")]
        public ActionResult Consultar_permissao(int id = 0, string descricao = "", string codigo = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_permissao"))
            {
                return Unauthorized();
            }

            var result = permissoesRepository.Consultar_permissao(id, descricao, codigo);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
  
    }
}