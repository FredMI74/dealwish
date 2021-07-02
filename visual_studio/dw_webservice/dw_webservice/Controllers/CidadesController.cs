using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class CidadesController : Controller
    {
        readonly CidadesRepository cidadesRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public CidadesController(CidadesRepository _cidadesRepository)
        {
            cidadesRepository = _cidadesRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_cidadesRepository.GetConfiguration());
        }

        [Route("consultar_cidade")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Consultar_cidade(int id = 0, string nome = "", string nome_exato = "", string uf = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_cidade", true);
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await cidadesRepository.ConsultarCidadeAsync(id, nome, nome_exato, uf);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}