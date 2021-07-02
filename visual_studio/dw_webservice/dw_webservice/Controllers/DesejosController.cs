using dw_webservice.Models;
using dw_webservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class DesejosController : Controller
    {
        readonly DesejosRepository desejosRepository;
        readonly ValidarTokenPermissao validarTokenPermissao;

        public DesejosController(DesejosRepository _desejosRepository)
        {
            desejosRepository = _desejosRepository;
            validarTokenPermissao = new ValidarTokenPermissao(_desejosRepository.GetConfiguration());
        }

        [Route("consultar_desejo")]
        [HttpPost]
        [Authorize(Roles = "app,bka,bki,bko,fto,tin")]
        public async Task<IActionResult> Consultar_desejo(int id = 0, string descricao = "", long id_usuario = 0, int id_tipo_produto = 0, int id_situacao = 0, string oferta = "", int id_empresa_oferta = 0, string uf = "", int id_cidade = 0, string token = "", string paginacao = "N", int max_id = 0, int pagina = 1, string exportar = "N")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/consultar_desejo");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await desejosRepository.ConsultarDesejoAsync(loginUsuario.Id, id, descricao, id_usuario, id_tipo_produto, id_situacao, oferta, id_empresa_oferta, uf, id_cidade, paginacao, max_id, pagina, exportar);
                        
            if (result == null)
            {
                return NotFound();
            }

            if (exportar == "S")
            {
                string nome_arquivo = Path.Combine(@"temp", loginUsuario.Id.ToString() + Constantes.DESEJOS_CSV);
                var path = (nome_arquivo);
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "text/csv", Path.GetFileName(path));
            }
            else
            {
                return Ok(result);
            }
        }

        [Route("incluir_desejo")]
        [HttpPost]
        [Authorize(Roles = "app,tin")]
        public async Task<IActionResult> Incluir_desejo(string descricao = "", long id_usuario = 0, int id_tipo_produto = 0, int id_situacao = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/incluir_desejo");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await desejosRepository.IncluirDesejoAsync(descricao, id_usuario, id_tipo_produto, id_situacao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("excluir_desejo")]
        [HttpPost]
        [Authorize(Roles = "bka,tin")]
        public async Task<IActionResult> Excluir_desejo(int id = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/excluir_desejo");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await desejosRepository.ExcluirDesejoAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_desejo")]
        [HttpPost]
        [Authorize(Roles = "tin")]
        public async Task<IActionResult> Atualizar_desejo(int id = 0, string descricao = "", long id_usuario = 0, int id_tipo_produto = 0, int id_situacao = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_desejo");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await desejosRepository.AtualizarDesejoAsync(id, descricao, id_usuario, id_tipo_produto, id_situacao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("atualizar_situacao_desejo")]
        [HttpPost]
        [Authorize(Roles = "app,tin")]
        public async Task<IActionResult> Atualizar_situacao_desejo(int id = 0, int id_situacao = 0, string token = "")
        {
            LoginUsuario loginUsuario = await validarTokenPermissao.ValidarAsync(token, "api/atualizar_situacao_desejo");
            if (!loginUsuario.Valido)
            {
                return Unauthorized();
            }

            var result = await desejosRepository.AtualizarSituacaoDesejoAsync(id, id_situacao);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}