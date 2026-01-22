using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenController : ControllerBase
    {
        private readonly Final_Programacion_2Context _context;
        private readonly IImagenService _imagenService;

        public ImagenController(Final_Programacion_2Context context, IImagenService imagenService)
        {
            _context = context;
            _imagenService = imagenService;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Imagen>>> GetAll(int objetoId, string tipoDeObjeto)
        {
            var listaDeImagenes = await _imagenService.ObtenerTodos(objetoId, tipoDeObjeto);
            if (listaDeImagenes is null)
            {
                return StatusCode(500, "Ocurrio un error del lado del servidor, intente de nuevo mas tarde.");
            }
            return Ok(listaDeImagenes);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Post(Imagen imagenNueva)
        { // No te olvides de poner "async" cuando uses await en este metodo tambien (si es que usas async en los metodos del servicio que usas, obvio).
            //si tenes que devolver algo tipo bool, usa IActionResult, o Task<IActionResult> si es async.
            if (!ModelState.IsValid)
            {
                return BadRequest("La imagen no llego al sistema correctamente, intente de nuevo.");
            }

            bool estado = await _imagenService.Crear(imagenNueva);

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
            bool estado = await _imagenService.Eliminar(id);
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
