using dw_webservice.Models;
using dw_webservice.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class OfertasController : Controller
    {
        OfertasRepository ofertasRepository;
        ValidarTokenPermissao validarTokenPermissao;

        public OfertasController(OfertasRepository _ofertasRepository)
        {
            ofertasRepository = _ofertasRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_ofertasRepository.GetConfiguration());
        }

        [Route("consultar_oferta")]
        [HttpPost]
        [Authorize(Roles = "app,bka,bki,bko,fta,fto,tin")]
        public ActionResult Consultar_oferta(int id = 0, int id_fatura = 0, int id_desejo = 0, int id_empresa = 0, int id_situacao = 0, string validade = "", 
                                             double valor = 0, string url = "", string descricao = "", string data_ini = "", string data_fim = "",
                                             string origem = "", string token = "", string paginacao = "N", int max_id = 0, int pagina = 1)
        {
            if (!validarTokenPermissao.Validar(out _, token, "api/consultar_oferta"))
            {
                return Unauthorized();
            }

            var result = ofertasRepository.Consultar_oferta(id, id_fatura, id_desejo, id_empresa, id_situacao, validade, valor, url, descricao, 
                                                            origem, data_ini, data_fim, paginacao, max_id, pagina);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_oferta")]
        [HttpPost]
        [Authorize(Roles = "bkc,bko,fto,tin")]
        public async System.Threading.Tasks.Task<ActionResult> Incluir_ofertaAsync(int id_desejo = 0, int id_empresa = 0, string validade = "", double valor = 0, string url = "", string descricao = "", string destaque = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_oferta"))
            {
                return Unauthorized();
            }

            var result =  await ofertasRepository.Incluir_oferta(id_desejo, id_empresa, validade, valor, url, descricao, destaque, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_oferta")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public ActionResult Excluir_oferta(int id = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/excluir_oferta"))
            {
                return Unauthorized();
            }

            var result = ofertasRepository.Excluir_oferta(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_situacao_oferta")]
        [HttpPost]
        [Authorize(Roles = "bko,fto,tin")]
        public ActionResult Atualizar_situacao_oferta(int id = 0, int id_situacao = 0, int id_empresa = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_situacao_oferta"))
            {
                return Unauthorized();
            }

            var result = ofertasRepository.Atualizar_situacao_oferta(id, id_situacao, id_empresa, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("atualizar_oferta")]
        [HttpPost]
        [Authorize(Roles = "tin")]
        public ActionResult Atualizar_oferta(int id = 0, int id_desejo = 0, int id_empresa = 0, string validade = "", double valor = 0, string url = "", string descricao = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_oferta"))
            {
                return Unauthorized();
            }

            var result = ofertasRepository.Atualizar_oferta(id, id_desejo, id_empresa, validade, valor, url, descricao, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_oferta_lote")]
        [HttpPost]
        [Authorize(Roles = "fto,tin")]
        public async System.Threading.Tasks.Task<ActionResult> Incluir_oferta_loteAsync(IFormFile file = null, int id_empresa = 0, string token = "")
        {

            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_oferta_lote"))
            {
                return Unauthorized();
            }

            var result = await ofertasRepository.Incluir_oferta_loteAsync(file, id_empresa, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }


        [Route("retorno_oferta_lote")]
        [HttpPost]
        [Authorize(Roles = "fto,tin")]
        public async System.Threading.Tasks.Task<IActionResult> Retorno_oferta_lote(string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/incluir_oferta_lote"))
            {
                return Unauthorized();
            }

            string nome_arquivo = Path.Combine(@"temp", id_usuario_login.ToString() + Constantes.LOTEOFERTAS_CSV);
            var path = (nome_arquivo);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "text/csv", Path.GetFileName(path));
         }

        [Route("atualizar_lida_oferta")]
        [HttpPost]
        [Authorize(Roles = "app,tin")]
        public ActionResult Atualizar_lida_oferta(int id = 0, string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_lida_oferta"))
            {
                return Unauthorized();
            }

            var result = ofertasRepository.Atualizar_lida_oferta(id, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_like_unlike_oferta")]
        [HttpPost]
        [Authorize(Roles = "app,tin")]
        public ActionResult Atualizar_like_unlike_oferta(int id = 0, string like_unlike = "", string token = "")
        {
            if (!validarTokenPermissao.Validar(out int id_usuario_login, token, "api/atualizar_like_unlike_oferta"))
            {
                return Unauthorized();
            }

            var result = ofertasRepository.Atualizar_like_unlike_oferta(id, like_unlike, id_usuario_login);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}