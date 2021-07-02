using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class PermissoesController : Controller
    {
        readonly PermissoesRepository permissoesRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public PermissoesController(PermissoesRepository _permissoesRepository)
        {
            permissoesRepository = _permissoesRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_permissoesRepository.GetConfiguration());
        }

        [Route("consultar_permissao")]
        [HttpPost]
        [Authorize(Roles = "bki,tin")]
        public async Task<ActionResult> Consultar_permissao(int id = 0, string descricao = "", string codigo = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_permissao");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await permissoesRepository.ConsultarPermissaoAsync(id, descricao, codigo);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
  
    }
}