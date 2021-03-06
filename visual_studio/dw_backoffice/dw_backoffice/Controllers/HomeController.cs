using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dw_backoffice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace dw_backoffice.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Utils.formataCabecalho(ViewBag, Request);
            return View();
        }

         public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
