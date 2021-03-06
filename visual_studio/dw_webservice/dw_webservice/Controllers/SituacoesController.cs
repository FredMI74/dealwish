using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class SituacoesController : Controller
    {
        SituacoesRepository situacoesRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public SituacoesController(SituacoesRepository _situacoesRepository)
        {
            situacoesRepository = _situacoesRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_situacoesRepository.GetConfiguration());
        }

        [Route("consultar_situacao")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,bko,fta,fto,tin")]
        public ActionResult Consultar_situacao(int id = 0, string descricao = "", string contratos = "", string usuarios = "", string desejos = "", string faturas = "", string produtos = "", string ofertas = "", string origem = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_situacao"))
            {
                return Unauthorized();
            }

            var result = situacoesRepository.Consultar_situacao(id, descricao, contratos,  usuarios, desejos, faturas, produtos, ofertas, origem);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
  
    }
}