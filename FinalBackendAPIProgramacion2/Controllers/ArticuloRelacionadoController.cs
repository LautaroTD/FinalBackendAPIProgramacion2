using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using Microsoft.AspNetCore.Mvc;
using FinalBackendAPIProgramacion2.DTO;

namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticuloRelacionadoController : ControllerBase
    {
        private readonly Final_Programacion_2Context _context;
        private readonly IArticuloRelacionadoService _articuloRelacionadoService;

        public ArticuloRelacionadoController(Final_Programacion_2Context context, IArticuloRelacionadoService articuloRelacionadoService)
        {
            _context = context;
            _articuloRelacionadoService = articuloRelacionadoService;
        }

        [HttpGet("getById/{id}")]
        public async Task<ActionResult<IEnumerable<DTOArticuloRelacionado?>>> GetById(int id)
        {
            var articuloRelacionado = await _articuloRelacionadoService.ObtenerPorId(id);
            if (articuloRelacionado is null)
            {
                return NotFound($"Articulo Relacionado con Id {id} no encontrado.");
            }
            return Ok(articuloRelacionado);
        }

        [HttpPost]
        public async Task<IActionResult> Post(DTOArticuloRelacionado articuloRelacionadoACrear)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("La relacion de articulo no llego al sistema correctamente, intente de nuevo.");
            }
            bool estado = await _articuloRelacionadoService.Crear(articuloRelacionadoACrear);
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
            bool estado = await _articuloRelacionadoService.Eliminar(id);
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
