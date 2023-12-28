using System.Security.Cryptography;

namespace ChatClient;

public class DiffieHellman
{
    private ECDiffieHellmanCng dh = new()
    {
        KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
        HashAlgorithm = CngAlgorithm.Sha256
    };

    public byte[] PublicKey => dh.PublicKey.ToByteArray();

    public byte[] GenerateKey(byte[] otherPublicKey)
    {
        using var otherPubKey = ECDiffieHellmanCngPublicKey.FromByteArray(otherPublicKey, CngKeyBlobFormat.EccPublicBlob);
        return dh.DeriveKeyMaterial(otherPubKey);
    }
}