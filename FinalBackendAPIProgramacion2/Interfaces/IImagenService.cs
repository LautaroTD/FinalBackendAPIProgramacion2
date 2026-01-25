using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Models;
namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IImagenService
    {
        Task<IEnumerable<Imagen?>> ObtenerTodos(int objetoId, string TipoDeObjeto);
        Task<Tuple<bool,string>> Crear(Imagen imagenNueva);
        Task<Tuple<bool,string>> Eliminar(int id, string nombreDeImagen);

        //Visto en el Video
        //public Tuple<bool, string> GuardarImagen(IFormFile archivoDeImagen); //Ya combine esto con el crear que usaba antes, esto esta para la memoria, NO IMPLEMENTAR.
        //public bool DeleteImage(string imageFileName);
    }
}
