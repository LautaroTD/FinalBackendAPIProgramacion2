using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResenaController : Controller
    {
        private readonly Final_Programacion_2Context _context;
        private readonly IResenaService _resenaService;

        public ResenaController(Final_Programacion_2Context context, IResenaService resenaService)
        {
            _context = context;
            _resenaService = resenaService;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<DTOResena>>> GetAll()
        {
            var resena = await _resenaService.ObtenerTodos();
            if (resena is null)
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
            return Ok(resena);
        }

        [HttpGet("getById/{id}")]
        public async Task<ActionResult<DTOResena>> GetById(int id)
        {
            var resena = await _resenaService.ObtenerPorId(id);
            if (resena is null)
            {
                return NotFound($"Reseña con Id {id} no encontrado.");
            }
            return Ok(resena);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Post(DTOResena resenaACrear)
        { // No te olvides de poner "async" cuando uses await en este metodo tambien (si es que usas async en los metodos del servicio que usas, obvio).
            //si tenes que devolver algo tipo bool, usa IActionResult, o Task<IActionResult> si es async.
            if (!ModelState.IsValid)
            {
                return BadRequest("La reseña no llego al sistema correctamente, intente de nuevo.");
            }

            bool estado = await _resenaService.Crear(resenaACrear);

            if (estado)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Edit(int id, int calificacion, string descripcion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("La reseña no fue rellenado correctamente, intente de nuevo.");
            }

            bool estado = await _resenaService.Editar(id, calificacion, descripcion);

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
            bool estado = await _resenaService.Eliminar(id);
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
