using dw_frontoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace dw_frontoffice.Controllers
{
    [Authorize]
    public class EmpresasController : Controller
    {
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ConsultarEmpresas(int id)
        {
            Empresa _Empresa = new Empresa();

            if (id != 0)
            {
                _Empresa = await ConsultarEmpresasId(id);
            }

            ProcessarRegistro _processarregistro = new ProcessarRegistro();
            
            Utils.formataCabecalho(ViewBag, Request);
            ViewBag.id = id;
            ViewBag.fantasia = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Fantasia;
            ViewBag.razao_social = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Razao_social;
            ViewBag.cnpj = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Cnpj;
            ViewBag.insc_est = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Insc_est;
            ViewBag.url = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Url;
            ViewBag.logo = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Logo;
            ViewBag.email_com = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Email_com;
            ViewBag.email_sac = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Email_sac;
            ViewBag.fone_com = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Fone_com;
            ViewBag.fone_sac = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Fone_sac;
            ViewBag.endereco = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Endereco;
            ViewBag.numero = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Numero;
            ViewBag.complemento = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Complemento;
            ViewBag.bairro = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Bairro;
            ViewBag.cep = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Cep;
            ViewBag.endereco_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Endereco_cob;
            ViewBag.numero_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Numero_cob;
            ViewBag.complemento_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Complemento_cob;
            ViewBag.bairro_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Bairro_cob;
            ViewBag.cep_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Cep_cob;
            ViewBag.nome_cidade = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Nome_cidade + "/" + _Empresa.Conteudo[0].Uf;
            ViewBag.nome_cidade_cob = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Nome_cidade_cob + "/" + _Empresa.Conteudo[0].Uf_cob;
            ViewBag.desc_qualificacao = (_Empresa == null || _Empresa.Conteudo == null) ? "" : _Empresa.Conteudo[0].Desc_qualificacao;

            ViewBag.erro = (_processarregistro == null || _processarregistro.Resultado == null ? false : _processarregistro.Resultado.Erro);
            ViewBag.linhasafetadas = (_processarregistro == null || _processarregistro.Conteudo == null ? -1 : _processarregistro.Conteudo.Linhasafetadas);
            ViewBag.mensagem = (_processarregistro == null || _processarregistro.Resultado == null ? "" : _processarregistro.Resultado.Mensagem);

            return View("ConsultarEmpresas");
        }

        public async Task<Empresa> ConsultarEmpresasId(int id = 0, string token = "", string tokenJwt = "")
        {
            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", string.IsNullOrWhiteSpace(tokenJwt) ? Request.Cookies[Constantes.TOKEN_JWT_USUARIO] : tokenJwt);

            if (string.IsNullOrWhiteSpace(token))
            {
                token = Request.Cookies[Constantes.TOKEN_USUARIO];
            }

            var consulta_Empresa = new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "token", token }
            };

            var request = new HttpRequestMessage();
            request.Content = new FormUrlEncodedContent(consulta_Empresa);

            var response = httpClient.client.PostAsync("api/consultar_empresa", request.Content).Result;

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Empresa>(responseStr);
            }
            else
            {
                return null;
            }
        }
    }
}