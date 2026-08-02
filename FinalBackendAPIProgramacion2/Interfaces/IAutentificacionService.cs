using FinalBackendAPIProgramacion2.DTO;

namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IAutentificacionService
    {
        DTOLoginResponse? Login(string nombre, string contrasena);
    }
}
