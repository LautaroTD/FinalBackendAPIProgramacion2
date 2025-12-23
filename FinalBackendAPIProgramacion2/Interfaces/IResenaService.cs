using FinalBackendAPIProgramacion2.DTO;

namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IResenaService
    {
        Task<IEnumerable<DTOResena?>> ObtenerTodos();
        Task<DTOResena?> ObtenerPorId(int id);
        Task<bool> Crear(DTOResena resena);
        Task<bool> Editar(DTOResena resena);
        Task<bool> Eliminar(int id);
    }
}
