using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.Services;
using FinalBackendAPIProgramacion2.Interfaces;
using System.Threading.Tasks;
using FinalBackendAPIProgramacion2.DTO;


namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticuloController : ControllerBase
    {
        private readonly Final_Programacion_2Context _context;
        private readonly IArticuloService _articuloService;

        public ArticuloController(Final_Programacion_2Context context, IArticuloService articuloService)
        {
            _context = context;
            _articuloService = articuloService;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<DTOArticulo>>> GetAll()
        { //nota: Te dara un error silencioso en el swagger si usas un controlador NO async con un metodo ASYNC en el SERVICIO.
            var articulo = await _articuloService.ObtenerTodos();
            if (articulo is null)
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
            return Ok(articulo);
        }

        [HttpGet("getById/{id}")]
        public async Task<ActionResult<DTOArticulo>> GetById(int id)
        {
            var usuario = await _articuloService.ObtenerPorId(id);

            if (usuario is null)
            {
                return NotFound($"Usuario con Id {id} no encontrado.");
            }

            return Ok(usuario);
        }

        [HttpPost("create")] // aca falta un modelo DTO para no exponer la entidad directamente
        public async Task<IActionResult> Post(DTOArticulo articuloCrear)
        { // No te olvides de poner "async" cuando uses await en este metodo tambien (si es que usas async en los metodos del servicio que usas, obvio).
            //si tenes que devolver algo tipo bool, usa IActionResult, o Task<IActionResult> si es async.
            if (!ModelState.IsValid)
            {
                return BadRequest("El articulo no fue rellenado correctamente, intente de nuevo.");
            }

            bool estado = await _articuloService.Crear(articuloCrear);

            if (estado)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
        }

        [HttpPut("edit/{articulo.Id}")]
        public async Task<IActionResult> Edit(DTOArticulo articulo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("El articulo no fue rellenado correctamente, intente de nuevo.");
            }

            bool estado = await _articuloService.Editar(articulo);

            if (estado)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool estado = await _articuloService.Eliminar(id);

            if (estado)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
        }
    }
}
