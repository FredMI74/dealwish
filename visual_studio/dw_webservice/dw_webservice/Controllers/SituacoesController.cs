using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class SituacoesController : Controller
    {
        readonly SituacoesRepository situacoesRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public SituacoesController(SituacoesRepository _situacoesRepository)
        {
            situacoesRepository = _situacoesRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_situacoesRepository.GetConfiguration());
        }

        [Route("consultar_situacao")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,bko,fta,fto,tin")]
        public async Task<ActionResult> Consultar_situacao(int id = 0, string descricao = "", string contratos = "", string usuarios = "", string desejos = "", string faturas = "", string produtos = "", string ofertas = "", string origem = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_situacao");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await situacoesRepository.ConsultarSituacaoAsync(id, descricao, contratos,  usuarios, desejos, faturas, produtos, ofertas, origem);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
  
    }
}