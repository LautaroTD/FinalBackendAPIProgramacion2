using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalBackendAPIProgramacion2.Services
{
    public class AutentificacionService : IAutentificacionService
    {
        private readonly IConfiguration _config;
        private readonly Final_Programacion_2Context _context;
        private readonly ILogger<AutentificacionService> _logger;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly JwtService _jwtService;
        public AutentificacionService(Final_Programacion_2Context context, ILogger<AutentificacionService> logger, IPasswordHasher<Usuario> passwordHasher, IConfiguration config, JwtService jwtService)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _config = config;
            _jwtService = jwtService;
        }

        public DTOLoginResponse? Login(string _nombre, string _contrasena)
        {
            if(_nombre is null || _contrasena is null)
            {
               return null;
            }

            var usuario = _context.Usuario.FirstOrDefault(e => e.Nombre == _nombre);
            if(usuario is null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Contrasena, _contrasena);

            if (result != PasswordVerificationResult.Success)
            {
                return null;
            }

            return _jwtService.GenerateToken(_nombre);
        }

    }
}
