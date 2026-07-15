using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FinalBackendAPIProgramacion2.Services
{
    public class JwtService
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

        public string GenerateToken(string username)
        {
            var credentials =
                new SigningCredentials(
                    new RsaSecurityKey(_keys.PrivateKey),
                    SecurityAlgorithms.RsaSha256
                );

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
