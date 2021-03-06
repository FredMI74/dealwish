using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class PlanosController : Controller
    {
        PlanosRepository planosRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public PlanosController(PlanosRepository _planosRepository)
        {
            planosRepository = _planosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_planosRepository.GetConfiguration());
        }

        [Route("consultar_plano")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,tin")]
        public ActionResult Consultar_plano(int id = 0, string descricao = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_plano"))
            {
                return Unauthorized();
            }

            var result = planosRepository.Consultar_plano(id, descricao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_plano")]
        [HttpPost]
        [Authorize(Roles = "bka,bkf,tin")]
        public ActionResult Incluir_plano(string descricao = "", int qtd_ofertas = 0, float valor_mensal = 0, float valor_oferta = 0, string visualizacao = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_plano"))
            {
                return Unauthorized();
            }

            var result = planosRepository.Incluir_plano(descricao , qtd_ofertas, valor_mensal, valor_oferta, visualizacao, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_plano")]
        [HttpPost]
        [Authorize(Roles = "bka,bkf,tin")]
        public ActionResult Excluir_plano(int id = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/excluir_plano"))
            {
                return Unauthorized();
            }

            var result = planosRepository.Excluir_plano(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_plano")]
        [HttpPost]
        [Authorize(Roles = "bka,bkf,tin")]
        public ActionResult Atualizar_plano(int id = 0, string descricao = "", int qtd_ofertas = 0, float valor_mensal = 0, float valor_oferta = 0, string visualizacao = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_plano"))
            {
                return Unauthorized();
            }

            var result = planosRepository.Atualizar_plano(id, descricao, qtd_ofertas, valor_mensal, valor_oferta, visualizacao, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}