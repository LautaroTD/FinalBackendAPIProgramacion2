using Azure.Core;
using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FinalBackendAPIProgramacion2.Services
{
    public class JwtService : IJwtService
    {
        private readonly PemKeyProvider _keys;
        private readonly IConfiguration _config;

        public JwtService(
            PemKeyProvider keys,
            IConfiguration config)
        {
            _keys = keys;
            _config = config;
        }

        public DTOLoginResponse GenerateToken(string username, string Rol)
        {
            var credentials =
                new SigningCredentials(
                    new RsaSecurityKey(_keys.PrivateKey),
                    SecurityAlgorithms.RsaSha256
                );

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, Rol)
        };

            var token = new JwtSecurityToken(
                issuer: _config["JwtConfig:Issuer"],
                audience: _config["JwtConfig:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(s: _config["JwtConfig:ExpirationMinutes"] ?? "15")),
                signingCredentials: credentials
            );

            var respuesta = new DTOLoginResponse(AccessToken: new JwtSecurityTokenHandler().WriteToken(token));

            return respuesta;
        }
    }
}
