using FinalBackendAPIProgramacion2.DTO;

namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IArticuloRelacionadoService
    {
        Task<IEnumerable<DTOArticuloRelacionado?>> ObtenerPorId(int id); //se buscan todas las relaciones que tiene un articulo
        Task<bool> Crear(DTOArticuloRelacionado relacion);
        Task<bool> Eliminar(int id); 
    }
}
