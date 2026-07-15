using System.Security.Cryptography;
using FinalBackendAPIProgramacion2.Interfaces;

namespace FinalBackendAPIProgramacion2.Services
{
    public class PemKeyProvider : IKeyProvider
    {
        public RSA PrivateKey { get; }
        public RSA PublicKey { get; }

        public PemKeyProvider(IConfiguration config)
        {
            var privatePem = File.ReadAllText(config["JwtConfig:PrivateKeyPath"]!);

            var publicPem = File.ReadAllText(config["JwtConfig:PublicKeyPath"]!);

            PrivateKey = RSA.Create();
            PrivateKey.ImportFromPem(privatePem);

            PublicKey = RSA.Create();
            PublicKey.ImportFromPem(publicPem);
        }
    }
}
