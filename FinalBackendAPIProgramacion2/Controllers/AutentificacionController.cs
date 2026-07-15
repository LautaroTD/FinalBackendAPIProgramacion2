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
        public IActionResult Login([FromBody] LoginRequest request)
        {
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
    }
}
