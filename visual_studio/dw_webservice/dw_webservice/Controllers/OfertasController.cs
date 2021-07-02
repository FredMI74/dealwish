using dw_webservice.Models;
using dw_webservice.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class OfertasController : Controller
    {
        readonly OfertasRepository ofertasRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public OfertasController(OfertasRepository _ofertasRepository)
        {
            ofertasRepository = _ofertasRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_ofertasRepository.GetConfiguration());
        }

        [Route("consultar_oferta")]
        [HttpPost]
        [Authorize(Roles = "app,bka,bki,bko,fta,fto,tin")]
        public async Task<ActionResult> Consultar_oferta(long id = 0, int id_fatura = 0, long id_desejo = 0, int id_empresa = 0, int id_situacao = 0, string validade = "", 
                                             double valor = 0, string url = "", string descricao = "", string data_ini = "", string data_fim = "",
                                             string origem = "", string token = "", string paginacao = "N", int max_id = 0, int pagina = 1)
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_oferta");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await ofertasRepository.ConsultarOfertaAsync(id, id_fatura, id_desejo, id_empresa, id_situacao, validade, valor, url, descricao, 
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
        public async Task<ActionResult> Incluir_ofertaAsync(long id_desejo = 0, int id_empresa = 0, string validade = "", double valor = 0, string url = "", string descricao = "", string destaque = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_oferta");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result =  await ofertasRepository.IncluirOfertaAsync(id_desejo, id_empresa, validade, valor, url, descricao, destaque, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_oferta")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<ActionResult> Excluir_oferta(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_oferta");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await ofertasRepository.ExcluirOfertaAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_situacao_oferta")]
        [HttpPost]
        [Authorize(Roles = "bko,fto,tin")]
        public async Task<ActionResult> Atualizar_situacao_oferta(long id = 0, int id_situacao = 0, int id_empresa = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_situacao_oferta");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await ofertasRepository.AtualizarSituacaoOfertaAsync(id, id_situacao, id_empresa, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [Route("atualizar_oferta")]
        [HttpPost]
        [Authorize(Roles = "tin")]
        public async Task<ActionResult> Atualizar_oferta(long id = 0, long id_desejo = 0, int id_empresa = 0, string validade = "", double valor = 0, string url = "", string descricao = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_oferta");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await ofertasRepository.AtualizarOfertaAsync(id, id_desejo, id_empresa, validade, valor, url, descricao, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("incluir_oferta_lote")]
        [HttpPost]
        [Authorize(Roles = "fto,tin")]
        public async Task<ActionResult> Incluir_oferta_lote(IFormFile file = null, int id_empresa = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_oferta_lote");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await ofertasRepository.IncluirOfertaLoteAsync(file, id_empresa, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }


        [Route("retorno_oferta_lote")]
        [HttpPost]
        [Authorize(Roles = "fto,tin")]
        public async Task<IActionResult> Retorno_oferta_lote(string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_oferta_lote");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            string nome_arquivo = Path.Combine(@"temp", loginUsuario.Id.ToString() + Constantes.LOTEOFERTAS_CSV);
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
        public async Task<ActionResult> Atualizar_lida_oferta(long id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_lida_oferta");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await ofertasRepository.AtualizarLidaOfertaAsync(id, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_like_unlike_oferta")]
        [HttpPost]
        [Authorize(Roles = "app,tin")]
        public async Task<ActionResult> Atualizar_like_unlike_oferta(long id = 0, string like_unlike = "", string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_like_unlike_oferta");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await ofertasRepository.AtualizarLikeUnlikeOfertaAsync(id, like_unlike, loginUsuario.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}