using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class ContratosController : Controller
    {
        readonly ContratosRepository contratosRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public ContratosController(ContratosRepository _contratosRepository)
        {
            contratosRepository = _contratosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_contratosRepository.GetConfiguration());
        }

        [Route("consultar_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkc,bkf,bki,tin")]
        public async Task<ActionResult> Consultar_contrato(int id = 0, int id_empresa = 0, int id_plano = 0, int id_situacao = 0, int dia_vct = 0, string data_inicio = "", string data_bloqueio = "", string data_termino = "", string inadimplentes = "" , string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_contrato");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await contratosRepository.ConsultarContratoAsync(id, id_empresa,id_plano, id_situacao, dia_vct,data_inicio, data_bloqueio, data_termino, inadimplentes);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public async Task<ActionResult> Incluir_contrato(int id_empresa = 0, int id_plano = 0, int id_situacao = 0, int dia_vct = 0, string data_inicio = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_contrato");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await contratosRepository.IncluirContratoAsync(id_empresa, id_plano, id_situacao, dia_vct, data_inicio, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public async Task<ActionResult> Excluir_contrato(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_contrato");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await contratosRepository.ExcluirContratoAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("bloquear_contratos_inadimplentes")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public async Task<ActionResult> Efetivar_faturas_abertas(string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/bloquear_contratos_inadimplentes");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await contratosRepository.BloquearContratosInadimplentesAsync(loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("desbloquear_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public async Task<ActionResult> Desbloquear_contrato(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/desbloquear_contrato");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await contratosRepository.DesbloquearContratoAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_contrato")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public async Task<ActionResult> Atualizar_contrato(int id = 0, int id_empresa = 0, int id_plano = 0, int id_situacao = 0, int dia_vct = 0, string data_inicio = "", string data_bloqueio = "", string data_termino = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_contrato");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await contratosRepository.AtualizarContratoAsync(id, id_empresa, id_plano, id_situacao, dia_vct, data_inicio, data_bloqueio, data_termino, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}