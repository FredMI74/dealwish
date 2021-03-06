using dw_backoffice.Models;
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

namespace dw_backoffice.Controllers
{
    [Authorize]
    public class CidadesController : Controller
    {
        public IActionResult ConsultarCidadesAutoCompletar(string nome)
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                DwClienteHttp httpClient = DwClienteHttp.Instance;
                httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies[Constantes.TOKEN_JWT_USUARIO]);

                string responseStr;

                var consulta_cidade = new Dictionary<string, string>
                {
                   { "nome", nome},
                   { "token", Request.Cookies[Constantes.TOKEN_USUARIO] }
                };

                var request = new HttpRequestMessage();
                request.Content = new FormUrlEncodedContent(consulta_cidade);
                HttpResponseMessage response;

                try
                {
                    response = httpClient.client.PostAsync("api/consultar_cidade", request.Content).Result;
                }
                catch
                {
                    return new JsonResult("");
                }
                
                responseStr = response.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrWhiteSpace(responseStr))
                {
                    return new JsonResult(JsonConvert.DeserializeObject<Cidade>(responseStr).Conteudo);
                }
                else
                {
                    return new JsonResult("");
                }
            }
            else
            {
                return new JsonResult("");
            }
        }
    }
}