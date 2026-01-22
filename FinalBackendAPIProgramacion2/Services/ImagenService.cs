using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalBackendAPIProgramacion2.Services
{
    public class ImagenService : IImagenService
    {
        private readonly Final_Programacion_2Context _context;
        private readonly ILogger<ImagenService> _logger;

        public ImagenService(Final_Programacion_2Context context, ILogger<ImagenService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Imagen>> ObtenerTodos(int objetoId, string TipoDeObjeto)
        {
            return await _context.Imagen
                .Where(i => i.TipoDeRelacion == TipoDeObjeto
                     && i.IdRelacionado == objetoId)
            .ToListAsync();
        }

        public async Task<bool> Crear(Imagen imagenNueva)
        {
            if (string.IsNullOrEmpty(imagenNueva.Ruta) || string.IsNullOrEmpty(imagenNueva.TipoDeRelacion))
                throw new ArgumentException($"La imagen no pudo ser agregada, todos los campos son obligatorios, rellene los campos y vuelva a intentarlo.");

            bool resultado = false;

            switch (imagenNueva.TipoDeRelacion)
            {
                case "Resena":
                    var resultadoResena = await _context.Resena.FindAsync(imagenNueva.IdRelacionado);
                    if (resultadoResena is not null)
                        resultado = true;
                    break;
                case "Articulo":
                    var resultadoArticulo = await _context.Articulo.FindAsync(imagenNueva.IdRelacionado);
                    if (resultadoArticulo is not null)
                        resultado = true;
                    break;
                case "ArticuloRelacionado":
                    var resultadoArticuloRelacionado = await _context.ArticuloRelacionado.FindAsync(imagenNueva.IdRelacionado);
                    if (resultadoArticuloRelacionado is not null)
                        resultado = true;
                    break;
                case "Usuario":
                    var resultadoUsuario = await _context.Usuario.FindAsync(imagenNueva.IdRelacionado);
                    if (resultadoUsuario is not null)
                        resultado = true;
                    break;

                default:
                    _logger.LogError($"Error al crear la imagen {imagenNueva.Id}. Entro a default en el Switch del servicio.");
                    break;
            }

            if(resultado == false)
                throw new KeyNotFoundException($"No pudimos crear la imagen, hubo un error del lado del servidor, intentelo de nuevo mas tarde.");

            bool ocupado = true;
            var random = new Random(); //esto es importante, el "new Random();" debe estar FUERA de la repeticion do{}while. y el random.Next debe estar DENTRO de la repeticion.

            do
            {
                int numero = random.Next();

                var imagen = _context.Imagen.Find(numero);

                if (imagen is null)
                {
                    ocupado = false;
                    imagenNueva.Id = numero;
                }

            } while (ocupado == true);

            _context.Imagen.Add(imagenNueva);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear la imagen {imagenNueva.Id}");
                return false;
            }

            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var imagenAEliminar = await _context.Imagen.FindAsync(id);
            if (imagenAEliminar is null)
            {
                throw new KeyNotFoundException($"Imagen con id {id} no encontrado.");
            }
            _context.Imagen.Remove(imagenAEliminar);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la imagen con id {id}");
                return false;
            }
            return true;
        }
    }
}
