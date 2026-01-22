using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Models;
namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IImagenService
    {
        Task<IEnumerable<Imagen?>> ObtenerTodos(int objetoId, string TipoDeObjeto);
        Task<bool> Crear(Imagen imagenNueva);
        Task<bool> Eliminar(int id);
    }
}
