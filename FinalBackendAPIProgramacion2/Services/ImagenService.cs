using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace FinalBackendAPIProgramacion2.Services
{
    public class ImagenService : IImagenService
    {
        private readonly Final_Programacion_2Context _context;
        private readonly ILogger<ImagenService> _logger;

        //Visto en el Video
        private IWebHostEnvironment environment;
        public ImagenService(Final_Programacion_2Context context, ILogger<ImagenService> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            environment = env;
        }
         
        public async Task<IEnumerable<Imagen>> ObtenerTodos(int objetoId, string TipoDeObjeto)
        {
            return await _context.Imagen
                .Where(i => i.TipoDeRelacion == TipoDeObjeto
                     && i.IdRelacionado == objetoId)
            .ToListAsync();
        }

        public async Task<Tuple<bool,string>> Crear(Imagen imagenNueva)
        {
            if (string.IsNullOrEmpty(imagenNueva.Ruta) || string.IsNullOrEmpty(imagenNueva.TipoDeRelacion) || imagenNueva.archivoDeImagen is null)
                throw new ArgumentException($"La imagen no pudo ser agregada, todos los campos son obligatorios, rellene los campos y vuelva a intentarlo.");

            var ext = Path.GetExtension(imagenNueva.archivoDeImagen.FileName);
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".JPG", ".JPEG", ".PNG" };
            if (!allowedExtensions.Contains(ext))
            {
                string msg = string.Format("Solo se permiten las siguientes extensiones: {0}", string.Join(",", allowedExtensions));
                return new Tuple<bool, string>(false, msg);
            }

            if(imagenNueva.TipoDeRelacion == "Usuario" && imagenNueva.IdRelacionado != imagenNueva.IdUsuario)
            {
                string msg = $"Error, no puede subir una imagen de un usuario que no sea usted mismo"; //el valor IdUsuario deberia ser enviado automaticamente por el sistema cliente, no deberia fallar.
                _logger.LogError($"Error, se intento crear una imagen de otro usuario, IdUsuario: {imagenNueva.IdUsuario}, IdRelacionado {imagenNueva.IdRelacionado}.");
                return new Tuple<bool, string>(false, msg);
            }

            var usuario = await _context.Usuario.FindAsync(imagenNueva.IdUsuario);

            if (usuario is null)
            {
                string msg = $"Error, intentelo de nuevo mas tarde"; //el valor IdUsuario deberia ser enviado automaticamente por el sistema cliente, no deberia fallar.
                _logger.LogError($"Error, no se encontro el usuario de ID {imagenNueva.IdUsuario} en la base de datos, se intento crear la imagen de IdRelacionado {imagenNueva.IdRelacionado} y TipoDeRelacion {imagenNueva.TipoDeRelacion}.");
                return new Tuple<bool, string>(false, msg);
            }

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
                    _logger.LogError($"Error al crear la imagen con IdRelacionada {imagenNueva.IdRelacionado} y TipoDeRelacion: {imagenNueva.TipoDeRelacion}. Entro a default en el Switch del servicio.");
                    break;
            }

            if(resultado == false)
            {
                string msg = $"Error, no se encontro el registro al que hace referencia la imagen en la base de datos, intente de nuevo mas tarde..";
                _logger.LogError(msg, $"idRelacionada de imagen:{ imagenNueva.IdRelacionado}, TipoDeRelacion : {imagenNueva.TipoDeRelacion}");
                return new Tuple<bool, string>(false, msg);
            }

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
                string msg = $"Error al guardar el registro de la imagen {imagenNueva.Id} en la base de datos, intente de nuevo mas tarde.";
                _logger.LogError(ex, msg);
                return new Tuple<bool, string>(false, msg);
            }

            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "wwwroot", "Imagenes", imagenNueva.Ruta);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string uniqueString = Guid.NewGuid().ToString();

            var newFileName = uniqueString + $"Tipo{imagenNueva.TipoDeRelacion}" + $"IdRelacionado{imagenNueva.IdRelacionado}" + $"Usuario{imagenNueva.IdUsuario}" + ext;
            var fileWithPath = Path.Combine(path, newFileName);

            try
            {
                using var stream = new FileStream(fileWithPath, FileMode.Create);
                imagenNueva.archivoDeImagen.CopyTo(stream);
                return new Tuple<bool, string>(true, newFileName);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, $"Error Input/Output al guardar la imagen, id: {imagenNueva.Id}");
                return new Tuple<bool, string>(false, "Error en la entrada de la imagen, intente de nuevom as tarde.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al guardar la imagen, id: {imagenNueva.Id}");
                return new Tuple<bool, string>(false, "Error al guardar la imagen, intente de nuevo mas tarde.");
            }
        }

        public async Task<Tuple<bool,string>> Eliminar(int id, string nombreDeImagen) //falta asegurarme que el que borre la imagen sea admin o el usuario que la creo
        {
            var imagen = await _context.Imagen.FindAsync(id);
            
            //Nota: esta seccion se accede a traves de una IU, se supone que el usuario ve la imagen antes de borrarla y todas sus interacciones son limitadas y a traves de botones, NO deberian ocurrir excepciones.

            if(imagen is null) {                 
                string msg = $"Error, no se encontro el registro de la imagen en la base de datos.";
                _logger.LogError(msg, $"se intento buscar la ID de imagen: {id}");
                return Tuple.Create(false, msg);
            }

            var wwwPath = environment.WebRootPath;
            var path = Path.Combine("wwwroot", "Imagenes", imagen.Ruta, nombreDeImagen);
            if (!System.IO.File.Exists(path))
            {
                string msg = $"Error, no se encontro la imagen en el sistema de archivos.";
                _logger.LogError(msg, $"ID de imagen: {id}.");
                return new Tuple<bool, string>(false, msg);
            }
                
            try
            {
                System.IO.File.Delete(path);
            }
            catch (IOException ex)
            {
                string msg = $"Error al eliminar la imagen del sistema de archivos.";
                _logger.LogError(ex, $"Error IO, la ID de la imagen es {id}.");
                return new Tuple<bool, string>(false, msg);
            }
            catch (Exception ex)
            {
                string msg = $"Error inesperado al eliminar la imagen del sistema de archivos.";
                _logger.LogError(ex, $"Id de imagen: {id}");
                return Tuple.Create(false, msg); //nota: la forma mas moderna de devolver una tupla es con "return (item1,item2)", pero son equivalentes. Pero como use Tuple<bool,string> en la firma del metodo, uso estas formas.
                //nota: para poder usar (item1,item2) se debe usar en el emtodo "public (bool x,string y) Metodo()". En teoria.
            }

            var imagenAEliminar = await _context.Imagen.FindAsync(id);
            if (imagenAEliminar is null)
            {
                string msg = $"Error, no se encontro el registro de la imagen en la base de datos al intentar eliminarlo.";
                _logger.LogError(msg, $"se intento buscar la ID de imagen: {id} para eliminarla de la base de datos.");
                return Tuple.Create(false, msg);
            }
            _context.Imagen.Remove(imagenAEliminar);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string msg = $"Error al eliminar el registro de la imagen de la base de datos.";
                _logger.LogError(ex, $"Error al eliminar la imagen con id {id}");
                return Tuple.Create(false, msg);
            }
            return Tuple.Create(true, "200");
        }
    }
}
