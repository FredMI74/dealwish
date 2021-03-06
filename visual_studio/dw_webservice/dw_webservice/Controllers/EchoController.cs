using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace dw_webservice.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class EchoController : Controller
    {
        IConfiguration configuration;
        public EchoController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }


        [Route("echo")]
        [HttpPost]
        public ActionResult Echo(string texto = "", string token = "")
        {
            if (token == configuration.GetSection("Chaves").GetSection("TokenAnonimo").Value)
            {
                return Ok(new { echo = texto });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}