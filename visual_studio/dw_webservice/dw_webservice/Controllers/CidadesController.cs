using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class CidadesController : Controller
    {
        CidadesRepository cidadesRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public CidadesController(CidadesRepository _cidadesRepository)
        {
            cidadesRepository = _cidadesRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_cidadesRepository.GetConfiguration());
        }

        [Route("consultar_cidade")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Consultar_cidade(int id = 0, string nome = "", string nome_exato = "", string uf = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_cidade", true))
            {
                return Unauthorized();
            }

            var result = cidadesRepository.Consultar_cidade(id, nome, nome_exato, uf);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}