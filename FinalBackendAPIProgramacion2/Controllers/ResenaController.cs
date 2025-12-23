using FinalBackendAPIProgramacion2.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResenaController : Controller
    {
        private readonly Final_Programacion_2Context _context;

        public IActionResult Index()
        {
            return View();
        }
    }
}
