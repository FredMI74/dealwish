using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class ConfigController : Controller
    {
        ConfigRepository configRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public ConfigController(ConfigRepository _configRepository)
        {
            configRepository = _configRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_configRepository.GetConfiguration());
        }

        [Route("consultar_config")]
        [HttpPost]
        [Authorize(Roles = "bka, bkc, bkf, bki, tin")]
        public ActionResult Consultar_config(int id = 0, string codigo = "", string valor = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_config"))
            {
                return Unauthorized();
            }

            var result = configRepository.Consultar_config(id, codigo, valor);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_config")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public ActionResult Atualizar_config(int id = 0, string codigo = "", string valor = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_config"))
            {
                return Unauthorized();
            }

            var result = configRepository.Atualizar_config(id, codigo, valor, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}