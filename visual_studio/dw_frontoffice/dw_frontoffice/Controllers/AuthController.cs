using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dw_frontoffice.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dw_frontoffice.Controllers
{
   

    public class AuthController : Controller
    {
        private static string mensagem = "";

        public IActionResult Login()
        {
            ViewData["Message"] = mensagem;
            return View();
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            mensagem = "";
            
            Utils.ClearAllCookies(HttpContext);

            DwClienteHttp httpClient = DwClienteHttp.Instance;

            var usuario_senha = new Dictionary<string, string>
            {
                { "dados", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username)) +  ';' +
                           Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password ))}, 
                { "origem", Constantes.FRONTOFFICE},
                { "token", httpClient.token_anonimo }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(usuario_senha);

            var response = new HttpResponseMessage();
            try
            {
                response = httpClient.client.PostAsync("api/login_usuario", request.Content).Result;
            }
            catch (Exception e)
            {
                mensagem = "Erro crítico. " + e.Message;
                return RedirectToAction(nameof(Login));
            }

            string data = await response.Content.ReadAsStringAsync();

            Token token = new Token();
            if (response.IsSuccessStatusCode)
            {
                token = JsonConvert.DeserializeObject<Token>(data);
            }
            else
            {
                mensagem = "Erro crítico." + response.StatusCode.ToString();
                return RedirectToAction(nameof(Login));
            }

            if (!token.Resultado.Erro && token.Conteudo.Token != "")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username, ClaimValueTypes.String)
                };

                var userIdentity = new ClaimsIdentity(claims, Constantes.DW_FRONTOFFICE + "SecureLogin");
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                await HttpContext.SignInAsync(Constantes.DW_FRONTOFFICE,
                    userPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        AllowRefresh = false
                    });

                EmpresasController empresa_controller = new EmpresasController();

                Utils.ClearAllCookies(HttpContext);

                Utils.SetCookie(Constantes.GRUPO_PERMISSAO_FTO, token.Conteudo.Grp_permissoes.IndexOf("fto") >= 0 ? Constantes.TRUE : Constantes.FALSE, HttpContext);
                Utils.SetCookie(Constantes.GRUPO_PERMISSAO_FTA, token.Conteudo.Grp_permissoes.IndexOf("fta") >= 0 ? Constantes.TRUE : Constantes.FALSE, HttpContext);
                Utils.SetCookie(Constantes.GRUPO_PERMISSAO_TIN, token.Conteudo.Grp_permissoes.IndexOf("tin") >= 0 ? Constantes.TRUE : Constantes.FALSE, HttpContext);
                Utils.SetCookie(Constantes.TOKEN_USUARIO, token.Conteudo.Token, HttpContext);
                Utils.SetCookie(Constantes.TOKEN_JWT_USUARIO, token.Conteudo.TokenJwt, HttpContext);
                Utils.SetCookie(Constantes.NOME_USUARIO, token.Conteudo.Nome, HttpContext);
                Utils.SetCookie(Constantes.ID_USUARIO, token.Conteudo.Id, HttpContext);
                Empresa _empresa = await empresa_controller.ConsultarEmpresasId(token.Conteudo.Id_empresa, token.Conteudo.Token, token.Conteudo.TokenJwt);
                Utils.SetCookie(Constantes.ID_EMPRESA, token.Conteudo.Id_empresa, HttpContext);
                Utils.SetCookie(Constantes.EMAIL, username, HttpContext);

                if (_empresa.Conteudo != null)
                {
                    Utils.SetCookie(Constantes.LOGO, _empresa.Conteudo[0].Logo, HttpContext);
                    Utils.SetCookie(Constantes.FANTASIA, _empresa.Conteudo[0].Fantasia, HttpContext);
                    Utils.SetCookie(Constantes.ID_CIDADE_EMPRESA, _empresa.Conteudo[0].Id_cidade, HttpContext);
                }

                return RedirectToAction("Index", "Home");
            }

            mensagem = token.Resultado.Mensagem;

            return RedirectToAction(nameof(Login));

        }

        public async Task<IActionResult> Logout()
        {
            Utils.ClearAllCookies(HttpContext);
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}