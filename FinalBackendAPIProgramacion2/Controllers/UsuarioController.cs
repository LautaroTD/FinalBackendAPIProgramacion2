using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.Services;
using FinalBackendAPIProgramacion2.Interfaces;
using System.Threading.Tasks;
using FinalBackendAPIProgramacion2.DTO;

namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        // este controlador se encarga del CRUD de la entidad Usuario, no de la autentificacion.
        private readonly Final_Programacion_2Context _context;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(Final_Programacion_2Context context, IUsuarioService usuarioService)
        {
            _context = context;
            _usuarioService = usuarioService;
        }

        // GET: api/Usuario/getAll
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<DTOUsuario>>> GetAll()
        { //nota: Te dara un error silencioso en el swagger si usas un controlador NO async con un metodo ASYNC en el SERVICIO.
            var usuarios = await _usuarioService.ObtenerTodos();
            if (usuarios is null) 
            {
                return StatusCode(500,"Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
            return Ok(usuarios);
        }

        // GET: api/Usuario/getById/{id}
        [HttpGet("getById/{nombre}")]
        public async Task<ActionResult<DTOUsuario>> GetById(int id)
        {
            var usuario = await _usuarioService.ObtenerPorId(id);

            if(usuario is null)
            {
                return NotFound($"Usuario con Id {id} no encontrado.");
            }
            
            return Ok(usuario);
        }

        // POST: api/Usuario/create
        [HttpPost("create")] // aca falta un modelo DTO para no exponer la entidad directamente
        public async Task<IActionResult> Post(DTOUsuario usuarioCrear)
        { // No te olvides de poner "async" cuando uses await en este metodo tambien (si es que usas async en los metodos del servicio que usas, obvio).
            //si tenes que devolver algo tipo bool, usa IActionResult, o Task<IActionResult> si es async.
            if(!ModelState.IsValid) 
            { 
                return BadRequest("El usuario no fue rellenado correctamente, intente de nuevo.");
            }

            bool estado = await _usuarioService.Crear(usuarioCrear);

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
        [HttpPut("edit/{nombre}")] // aca falta un modelo DTO para no exponer la entidad directamente
        public async Task<IActionResult> Edit(DTOUsuario usuario)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("El usuario no fue rellenado correctamente, intente de nuevo.");
            }

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
        [HttpDelete("delete/{nombre}")]
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

