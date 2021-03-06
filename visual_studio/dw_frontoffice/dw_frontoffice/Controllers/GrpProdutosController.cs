using dw_frontoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;

namespace dw_frontoffice.Controllers
{
    [Authorize]
    public class GrpProdutosController : Controller
    {

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ConsultarGrpProdutos(string responseStr = "", string responseStatusCode = "")
        {
            GrpProduto _grpproduto = new GrpProduto();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _grpproduto = JsonConvert.DeserializeObject<GrpProduto>(responseStr);
                ViewBag.erro = (_grpproduto == null || _grpproduto.Resultado == null ? false : _grpproduto.Resultado.Erro);
                ViewBag.mensagem = (_grpproduto == null || _grpproduto.Resultado == null ? "" : _grpproduto.Resultado.Mensagem);
            }
            else
            {
                ViewBag.erro = false;
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.grpprodutos = (_grpproduto == null || _grpproduto.Conteudo == null) ? new List<GrpProduto.Dados>() : _grpproduto.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarGrpProdutos");
        }

        [HttpPost]
        public async Task<IActionResult> ConsultarGrpProdutosPost(int id, string descricao)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_grpproduto = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao },
                { "id_situacao", Constantes.ATIVO.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_grpproduto);

            var response = httpClient.client.PostAsync("api/consultar_grp_produto", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();
 
            if (response.IsSuccessStatusCode)
            {
                return ConsultarGrpProdutos(responseStr, response.StatusCode.ToString());
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    return RedirectToAction(nameof(Error));
                }
            }

        }

    }
}