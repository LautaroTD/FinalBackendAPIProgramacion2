using FinalBackendAPIProgramacion2.DTO;

namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IJwtService
    {
        DTOLoginResponse GenerateToken(string username, string Rol);
    }
}
