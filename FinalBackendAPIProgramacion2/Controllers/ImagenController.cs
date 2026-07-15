using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace FinalBackendAPIProgramacion2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenController : ControllerBase
    {
        private readonly Final_Programacion_2Context _context;
        private readonly IImagenService _imagenService;
        private readonly IWebHostEnvironment _env;

        public ImagenController(Final_Programacion_2Context context, IImagenService imagenService, IWebHostEnvironment env)
        {
            _context = context;
            _imagenService = imagenService;
            _env = env;
        }

        [HttpGet("search/{tipoDeObjeto}/{objetoId}")]
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
        public async Task<IActionResult> Post([FromForm]Imagen imagenNueva)
        { // No te olvides de poner "async" cuando uses await en este metodo tambien (si es que usas async en los metodos del servicio que usas, obvio).
            //si tenes que devolver algo tipo bool, usa IActionResult, o Task<IActionResult> si es async.
            if (!ModelState.IsValid)
            {
                return BadRequest("La imagen no llego al sistema correctamente, intente de nuevo.");
            }

            Tuple<bool,string> estado = await _imagenService.Crear(imagenNueva);

            if (estado.Item1)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, estado.Item2);
            }
        }

        [HttpDelete("delete/{id}/{nombreDeImagen}")]
        public async Task<IActionResult> Delete(int id,string nombreDeImagen)
        {
            Tuple<bool,string> estado = await _imagenService.Eliminar(id, nombreDeImagen);
            if (estado.Item1)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, estado.Item2);
            }
        }
    }
}
