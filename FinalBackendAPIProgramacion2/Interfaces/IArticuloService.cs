using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Models;

namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IArticuloService
    {
        Task<IEnumerable<DTOArticulo?>> ObtenerTodos();
        Task<DTOArticulo?> ObtenerPorId(int id);
        //falta el modelo DTO para crear un usuario
        Task<bool> Crear(DTOArticulo articulo);
        //falta el modelo DTO para editar un usuario
        Task<bool> Editar(DTOArticulo articulo);
        Task<bool> Eliminar(int id);
    }
}
