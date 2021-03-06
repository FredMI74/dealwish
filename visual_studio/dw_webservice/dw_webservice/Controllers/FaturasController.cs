using dw_webservice.Models;
using dw_webservice.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class FaturasController : Controller
    {
        FaturasRepository faturasRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public FaturasController(FaturasRepository _faturasRepository)
        {
            faturasRepository = _faturasRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_faturasRepository.GetConfiguration());
        }

        [Route("consultar_fatura")]
        [HttpPost]
        [Authorize(Roles = "bkc,bkf,bki,fta,tin")]
        public async System.Threading.Tasks.Task<ActionResult> Consultar_faturaAsync(int id = 0, int id_empresa = 0, string razao_social = "", int mes = 0, int ano = 0, string nosso_numero = "", int id_situacao = 0, string abertas = "", string exportar = "N", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/consultar_fatura"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Consultar_fatura(out bool erro_exportar, id_usuario_login, id, id_empresa, razao_social, mes, ano, nosso_numero, id_situacao, abertas, exportar);
            if (result == null)
            {
                return NotFound();
            }

            if (erro_exportar)
            {
                return Ok(result);
            }

            if (exportar != Constantes.NAO && exportar != Constantes.PIX)
            {
                string nome_arquivo = result.GetType().GetProperty("Nome_arquivo").GetValue(result).ToString();
                string tipo_arquivo = result.GetType().GetProperty("Tipo_arquivo").GetValue(result).ToString();

                var memory = new MemoryStream();
                using (var stream = new FileStream(nome_arquivo, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, tipo_arquivo, Path.GetFileName(nome_arquivo));
            }
            else
            {
                return Ok(result);
            }
        }

        [Route("consultar_faturas_abertas")]
        [HttpPost]
        [Authorize(Roles = "bkf,bki,tin")]
        public ActionResult Consultar_faturas_abertas(string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/consultar_faturas_abertas"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Consultar_faturas_abertas(id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("efetivar_faturas_abertas")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public ActionResult Efetivar_faturas_abertas(string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/efetivar_faturas_abertas"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Efetivar_faturas_abertas(id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_fatura")]
        [HttpPost]
        [Authorize(Roles = "tin")]
        public ActionResult Incluir_fatura(int id_empresa = 0 , int mes = 0, int ano = 0, string nosso_numero = "", double valor = 0, string data_vct = "", int qtd_ofertas = -1, int id_situacao = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_fatura"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Incluir_fatura(id_empresa, mes, ano, nosso_numero, valor , data_vct, qtd_ofertas, id_situacao, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_fatura")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public ActionResult Excluir_fatura(int id = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/excluir_fatura"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Excluir_fatura(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_fatura")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public ActionResult Atualizar_fatura(int id = 0, int mes = 0, int ano = 0, string nosso_numero = "", double valor = 0, string data_vct = "", string data_pg = "", double multa = 0, double juros = 0, double valor_pg = 0, int qtd_ofertas = -1, int id_situacao = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_fatura"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Atualizar_fatura(id, mes, ano, nosso_numero, valor, data_vct, data_pg, multa, juros, valor_pg, qtd_ofertas, id_situacao, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("processar_retorno_boleto")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public ActionResult Processar_retorno_boleto(IFormFile file = null, string token = "")
        {

            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/processar_retorno_boleto"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Processar_retorno_boleto(file, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [Route("retorno_processamento_boleto")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public async System.Threading.Tasks.Task<IActionResult> Retorno_processar_retorno_boleto(string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/processar_retorno_boleto"))
            {
                return Unauthorized();
            }

            string nome_arquivo = Path.Combine(@"temp", id_usuario_login.ToString() + Constantes.RETORNO_PROCESSAMENTO_BOLETO_CSV);
            var path = (nome_arquivo);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "text/csv", Path.GetFileName(path));
        }


        [Route("processar_retorno_nf")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public ActionResult Processar_retorno_nota_fiscal(IFormFile file = null, string token = "")
        {

            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/processar_retorno_nf"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Processar_retorno_nf(file, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [Route("retorno_processamento_nf")]
        [HttpPost]
        [Authorize(Roles = "bkf,tin")]
        public async System.Threading.Tasks.Task<IActionResult> Retorno_processar_retorno_nf(string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/processar_retorno_nf"))
            {
                return Unauthorized();
            }

            string nome_arquivo = Path.Combine(@"temp", id_usuario_login.ToString() + Constantes.RETORNO_PROCESSAMENTO_NF_CSV);
            var path = (nome_arquivo);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "text/csv", Path.GetFileName(path));
        }


        [Route("consultar_indicadores")]
        [HttpPost]
        [Authorize(Roles = "bka,bki,tin")]
        public ActionResult Consultar_indicadores(string token = "")
        {

            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/consultar_indicadores"))
            {
                return Unauthorized();
            }

            var result = faturasRepository.Consultar_indicadores(id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}