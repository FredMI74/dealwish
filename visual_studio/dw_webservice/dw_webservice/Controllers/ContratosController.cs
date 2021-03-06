using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class ContratosController : Controller
    {
        ContratosRepository contratosRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public ContratosController(ContratosRepository _contratosRepository)
        {
            contratosRepository = _contratosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_contratosRepository.GetConfiguration());
        }

        [Route("consultar_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkc,bkf,bki,tin")]
        public ActionResult Consultar_contrato(int id = 0, int id_empresa = 0, int id_plano = 0, int id_situacao = 0, int dia_vct = 0, string data_inicio = "", string data_bloqueio = "", string data_termino = "", string inadimplentes = "" , string token = "")
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_contrato"))
            {
                return Unauthorized();
            }

            var result = contratosRepository.Consultar_contrato(id, id_empresa,id_plano, id_situacao, dia_vct,data_inicio, data_bloqueio, data_termino, inadimplentes);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public ActionResult Incluir_contrato(int id_empresa = 0, int id_plano = 0, int id_situacao = 0, int dia_vct = 0, string data_inicio = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_contrato"))
            {
                return Unauthorized();
            }

            var result = contratosRepository.Incluir_contrato(id_empresa, id_plano, id_situacao, dia_vct, data_inicio, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public ActionResult Excluir_contrato(int id = 0, string token = "")
        {

            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/excluir_contrato"))
            {
                return Unauthorized();
            }

            var result = contratosRepository.Excluir_contrato(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("bloquear_contratos_inadimplentes")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public ActionResult Efetivar_faturas_abertas(string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/bloquear_contratos_inadimplentes"))
            {
                return Unauthorized();
            }

            var result = contratosRepository.Bloquear_contratos_inadimplentes(id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("desbloquear_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public ActionResult Desbloquear_contrato(int id = 0, string token = "")
        {

            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/desbloquear_contrato"))
            {
                return Unauthorized();
            }

            var result = contratosRepository.Desbloquear_contrato(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public ActionResult Atualizar_contrato(int id = 0, int id_empresa = 0, int id_plano = 0, int id_situacao = 0, int dia_vct = 0, string data_inicio = "", string data_bloqueio = "", string data_termino = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_contrato"))
            {
                return Unauthorized();
            }

            var result = contratosRepository.Atualizar_contrato(id, id_empresa, id_plano, id_situacao, dia_vct, data_inicio, data_bloqueio, data_termino, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}