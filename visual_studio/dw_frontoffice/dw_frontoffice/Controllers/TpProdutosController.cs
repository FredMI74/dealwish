using dw_frontoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace dw_frontoffice.Controllers
{
    [Authorize]
    public class TpProdutosController : Controller
    {
        private GrpProduto grpproduto;

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
 
        public IActionResult ConsultarTpProdutos(string responseStr = "", string responseStatusCode = "")
        {
            TpProduto _tpproduto = new TpProduto();

            if (!string.IsNullOrWhiteSpace(responseStr))
            {
                _tpproduto = JsonConvert.DeserializeObject<TpProduto>(responseStr);
                ViewBag.erro = (_tpproduto == null || _tpproduto.Resultado == null ? false : _tpproduto.Resultado.Erro);
                ViewBag.mensagem = (_tpproduto == null || _tpproduto.Resultado == null ? "" : _tpproduto.Resultado.Mensagem);
            }
            else
            {
                ViewBag.erro = false;
            }

            if (grpproduto == null)
            {
                grpproduto = new GrpProduto();
                grpproduto = ConsultarGrpProdutos();
            }

            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.tpprodutos = (_tpproduto == null || _tpproduto.Conteudo == null) ? new List<TpProduto.Dados>() : _tpproduto.Conteudo;
            ViewBag.grpprodutos = grpproduto.Conteudo;

            ViewBag.responseStatusCode = responseStatusCode;

            return View("ConsultarTpProdutos");
        }


        [HttpPost]
        public async Task<IActionResult> ConsultarTpProdutosPost(int id, string descricao, int id_grp_prod)
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_tpproduto = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "descricao", descricao },
                { "id_grp_prod", id_grp_prod.ToString() },
                { "id_situacao", Constantes.ATIVO.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_tpproduto);

            var response = httpClient.client.PostAsync("api/consultar_tp_produto", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ConsultarTpProdutos(responseStr, response.StatusCode.ToString());
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

        private GrpProduto ConsultarGrpProdutos()
        {

            string responseStr;

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

            var consulta_grpproduto = new Dictionary<string, string>
            {
                { "descricao", "%" },
                { "id_situacao", Constantes.ATIVO.ToString() },
                { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
            };
            
            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_grpproduto);

            var response = httpClient.client.PostAsync("api/consultar_grp_produto", request.Content).Result;

            responseStr = response.Content.ReadAsStringAsync().Result;

            if (responseStr != null)
            {
                return JsonConvert.DeserializeObject<GrpProduto>(responseStr);
            }
            else
            {
                return null;
            }
        }

    }
}