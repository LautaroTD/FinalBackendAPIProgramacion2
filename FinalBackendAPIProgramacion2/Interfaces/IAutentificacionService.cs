namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IAutentificacionService
    {
        string? Login(string nombre, string contrasena);
    }
}
