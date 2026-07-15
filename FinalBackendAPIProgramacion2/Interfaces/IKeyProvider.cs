using System.Security.Cryptography;

namespace FinalBackendAPIProgramacion2.Interfaces
{
    public interface IKeyProvider
    {
        RSA PrivateKey { get; }
        RSA PublicKey { get; }
    }
}
