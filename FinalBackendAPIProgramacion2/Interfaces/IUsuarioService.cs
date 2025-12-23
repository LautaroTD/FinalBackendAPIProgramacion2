using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Models;

namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IUsuarioService
    {
        //osea, una interfaz sirve para varias cosas, pero en este caso es para definir los metodos que va a tener el servicio de usuario
        //osea, es una memoria de los metodos que va a tener el servicio de usuario y tenerlo todo resumido y organizado. Muy comodo.
        Task<IEnumerable<DTOUsuario?>> ObtenerTodos();
        Task<DTOUsuario?> ObtenerPorId(int id);
        //falta el modelo DTO para crear un usuario
        Task<bool> Crear(DTOUsuario usuario);
        //falta el modelo DTO para editar un usuario
        Task<bool> Editar(DTOUsuario usuario);
        Task<bool> Eliminar(int id);
    }
}
