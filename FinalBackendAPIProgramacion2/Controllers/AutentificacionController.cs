using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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
                return BadRequest("Nombre de usuario o contraseña incorrectos");
            }

            return Ok(token);
        }



        [Authorize(Roles = "admin")]
        [HttpGet("authTest")]
        public IActionResult AuthTest()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            //if (rol != "admin") return Unauthorized(); //<- Asi digo que un metodo solo puede ser usado por 1 rol
            //yupi aprendi a usar esta cosa
            //if (string.IsNullOrEmpty(rol)) return Unauthorized(); //<- Asi pido que el metodo solo sea accesible para usuarios con cualquier rol

            var auth = Request.Headers.Authorization.ToString();
            
            return Ok("Autorizacion Correcta");
        }
    }
}
