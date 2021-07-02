using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using dw_webservice.Models;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class EmpresasController : Controller
    {
        readonly EmpresasRepository empresasRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public EmpresasController(EmpresasRepository _empresasRepository)
        {
            empresasRepository = _empresasRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_empresasRepository.GetConfiguration());
        }

        [Route("consultar_empresa")]
        [HttpPost]
        [Authorize(Roles = "bkc,bkf,bki,bko,fta,fto,tin,app")]
        public async Task<ActionResult> Consultar_empresa(int id = 0, string fantasia = "", string razao_social = "", string cnpj = "", string insc_est = "", string url = "", string email_com = "", string email_sac = "", string fone_com = "", string fone_sac = "", string endereco = "", string numero = "", string complemento = "", string bairro = "", string cep = "", string endereco_cob = "", string numero_cob = "", string complemento_cob = "", string bairro_cob = "", string cep_cob = "", int id_cidade = 0, int id_cidade_cob = 0, string uf = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_empresa");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await empresasRepository.ConsultarEmpresaAsync(id, fantasia, razao_social, cnpj, insc_est, url, email_com, email_sac, fone_com, fone_sac, endereco, numero, complemento, bairro, cep, endereco_cob, numero_cob, complemento_cob, bairro_cob, cep_cob, id_cidade, id_cidade_cob, uf);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_empresa")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public async Task<ActionResult> Incluir_empresa(string fantasia = "", string razao_social = "", string cnpj = "", string insc_est = "", string url = "", string email_com = "", string email_sac = "", string fone_com = "", string fone_sac = "", string endereco = "", string numero = "", string complemento = "", string bairro = "", string cep = "", string endereco_cob = "", string numero_cob = "", string complemento_cob = "", string bairro_cob = "", string cep_cob = "", int id_cidade = 0, int id_cidade_cob = 0, string logo = "", int id_qualificacao = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_empresa");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await empresasRepository.IncluirEmpresaAsync(fantasia, razao_social, cnpj, insc_est, url, email_com, email_sac, fone_com, fone_sac, endereco, numero, complemento, bairro, cep, endereco_cob, numero_cob, complemento_cob, bairro_cob, cep_cob, id_cidade, id_cidade_cob, logo, id_qualificacao, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_empresa")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public async Task<ActionResult> Excluir_empresa(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_empresa");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await empresasRepository.ExcluirEmpresaAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_empresa")]
        [HttpPost]
        [Authorize(Roles = "bkc,tin")]
        public async Task<ActionResult> Atualizar_empresa(int id = 0, string fantasia = "", string razao_social = "", string cnpj = "", string insc_est = "", string url = "", string email_com = "", string email_sac = "", string fone_com = "", string fone_sac = "", string endereco = "", string numero = "", string complemento = "", string bairro = "", string cep = "", string endereco_cob = "", string numero_cob = "", string complemento_cob = "", string bairro_cob = "", string cep_cob = "", int id_cidade = 0, int id_cidade_cob = 0, string logo = "", int id_qualificacao = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_empresa");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await empresasRepository.AtualizarEmpresaAsync(id, fantasia, razao_social, cnpj, insc_est, url, email_com, email_sac, fone_com, fone_sac, endereco, numero, complemento, bairro, cep, endereco_cob, numero_cob, complemento_cob, bairro_cob, cep_cob, id_cidade, id_cidade_cob, logo, id_qualificacao, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("consultar_qualificacao")]
        [HttpPost]
        [Authorize(Roles = "bka,bkc,bkf,bki,tin")]
        public async Task<ActionResult> Consultar_qualificacao(string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_qualificacao");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await empresasRepository.ConsultarQualificacaoAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}