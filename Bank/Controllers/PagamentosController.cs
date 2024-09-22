using Microsoft.AspNetCore.Mvc;

namespace Bank.Controllers
{
    public class PagamentosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
