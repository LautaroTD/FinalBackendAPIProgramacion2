using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        // este controlador se encarga del CRUD de la entidad Usuario, no de la autentificacion.
        private readonly Final_Programacion_2Context _context;
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(Final_Programacion_2Context context, IUsuarioService usuarioService, ILogger<UsuarioController> logger)
        {
            _context = context;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        // GET: api/Usuario/getAll
        [Authorize(Roles = "admin")]
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<DTOUsuario>>> GetAll()
        { //nota: Te dara un error silencioso en el swagger si usas un controlador NO async con un metodo ASYNC en el SERVICIO.
            
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            if (rol != "admin") return Unauthorized();

            var usuarios = await _usuarioService.ObtenerTodos();
            if (usuarios is null) 
            {
                return StatusCode(500,"Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
            return Ok(usuarios);
        }

        // GET: api/Usuario/getById/{id}
        [Authorize(Roles = "admin")]
        [HttpGet("getById/{nombre}")]
        public async Task<ActionResult<DTOUsuario>> GetById(int id)
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            if (rol != "admin") return Unauthorized();

            var usuario = await _usuarioService.ObtenerPorId(id);

            if(usuario is null)
            {
                return NotFound($"Usuario con Id {id} no encontrado.");
            }
            
            return Ok(usuario);
        }

        // POST: api/Usuario/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> Post(DTOUsuario usuarioNuevo)
        { // No te olvides de poner "async" cuando uses await en este metodo tambien (si es que usas async en los metodos del servicio que usas, obvio).
            //si tenes que devolver algo tipo bool, usa IActionResult, o Task<IActionResult> si es async.
            if(!ModelState.IsValid) 
            { 
                return BadRequest("El usuario no fue rellenado correctamente, intente de nuevo.");
            }

            bool estado = await _usuarioService.Crear(usuarioNuevo);

            if (estado) 
            {
                return Ok();
            }
            else 
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
        }

        // POST: api/Usuario/crear
        [Authorize(Roles = "admin")]
        [HttpPost("crear")]
        public async Task<IActionResult> PostComoAdmin(DTOUsuario usuarioNuevo)
        { // No te olvides de poner "async" cuando uses await en este metodo tambien (si es que usas async en los metodos del servicio que usas, obvio).
            //si tenes que devolver algo tipo bool, usa IActionResult, o Task<IActionResult> si es async.
            
            if (!ModelState.IsValid)
            {
                return BadRequest("El usuario no fue rellenado correctamente, intente de nuevo.");
            }

            bool estado = await _usuarioService.CrearComoAdmin(usuarioNuevo);

            if (estado)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
        }

        // PUT: api/Usuario/edit/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("edit/{usuario.Id}")] 
        public async Task<IActionResult> Edit(DTOUsuario usuario)
        {

            bool estado = await _usuarioService.Editar(usuario);

            if(estado)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
        }

        // DELETE: api/Usuario/delete/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            
            bool estado = await _usuarioService.Eliminar(id);

            if(estado)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intentelo de nuevo mas tarde.");
            }
        }

    }
}

