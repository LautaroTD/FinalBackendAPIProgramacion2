using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutentificacionController : ControllerBase //sin "ControllerBase" no podes usar cosas como "return BadRequest()"
    {
        private readonly Final_Programacion_2Context _context;
        private readonly IAutentificacionService _AutentificacionService;

        public AutentificacionController(Final_Programacion_2Context context, IAutentificacionService autentificacionService)
        {
            _context = context;
            _AutentificacionService = autentificacionService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            //Problema: O el frontend no puede recibir el token (pero si los codigos http), o el backend no puede enviar el token al frontend pero si a swagger.
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = _AutentificacionService.Login(request.Nombre, request.Contrasena);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        [Authorize]
        [HttpGet("authTest")]
        public IActionResult AuthTest()
        {
            var auth = Request.Headers.Authorization.ToString();
            Console.WriteLine($"pipupipu {auth}");

            return Ok("Autorizacion Correcta");
        }
    }
}
