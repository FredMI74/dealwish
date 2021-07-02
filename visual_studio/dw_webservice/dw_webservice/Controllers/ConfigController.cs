using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class ConfigController : Controller
    {
        readonly ConfigRepository configRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public ConfigController(ConfigRepository _configRepository)
        {
            configRepository = _configRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_configRepository.GetConfiguration());
        }

        [Route("consultar_config")]
        [HttpPost]
        [Authorize(Roles = "bka, bkc, bkf, bki, tin")]
        public async Task<ActionResult> Consultar_config(int id = 0, string codigo = "", string valor = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_config");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await configRepository.ConsultarConfigAsync(id, codigo, valor);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_config")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<ActionResult> Atualizar_config(int id = 0, string codigo = "", string valor = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_config");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await configRepository.AtualizarConfigAsync(id, codigo, valor, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}