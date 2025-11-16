using System.Security.Cryptography;
using System.Text;

namespace CryptoLab.Services
{
    public class CryptoService
    {
        // ===== ХЭШИ =====
        public string ComputeHash(string text, string algo = "SHA256")
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] hash = algo.ToUpperInvariant() switch
            {
                "SHA1" => SHA1.HashData(data),
                "SHA256" => SHA256.HashData(data),
                "SHA384" => SHA384.HashData(data),
                "SHA512" => SHA512.HashData(data),
                "MD5" => MD5.HashData(data),
                _ => SHA256.HashData(data)
            };
            return Convert.ToHexString(hash);
        }

        // ===== AES-GCM =====
        // Возвращает: base64( nonce || tag || ciphertext )
        public string AesGcmEncrypt(string plaintext, string base64Key)
        {
            var key = Convert.FromBase64String(base64Key);
            using var aes = new AesGcm(key);
            var nonce = RandomNumberGenerator.GetBytes(12);
            var pt = Encoding.UTF8.GetBytes(plaintext);
            var ct = new byte[pt.Length];
            var tag = new byte[16];
            aes.Encrypt(nonce, pt, ct, tag);
            var blob = nonce.Concat(tag).Concat(ct).ToArray();
            return Convert.ToBase64String(blob);
        }

        public string AesGcmDecrypt(string base64Blob, string base64Key)
        {
            var key = Convert.FromBase64String(base64Key);
            var blob = Convert.FromBase64String(base64Blob);
            var nonce = blob[..12];
            var tag = blob[12..28];
            var ct = blob[28..];

            using var aes = new AesGcm(key);
            var pt = new byte[ct.Length];
            aes.Decrypt(nonce, ct, tag, pt);
            return Encoding.UTF8.GetString(pt);
        }

        public string GenerateAesKeyBase64(int bits = 256)
        {
            if (bits is not (128 or 192 or 256)) bits = 256;
            var key = RandomNumberGenerator.GetBytes(bits / 8);
            return Convert.ToBase64String(key);
        }

        // ===== RSA =====
        public (string publicPem, string privatePem) RsaGenerate(int keySize = 2048)
        {
            using var rsa = RSA.Create(keySize);
            var pub = rsa.ExportSubjectPublicKeyInfoPem();
            var priv = rsa.ExportPkcs8PrivateKeyPem();
            return (pub, priv);
        }

        // Шифрование/дешифрование малых сообщений (демо)
        public string RsaEncrypt(string plaintext, string publicPem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicPem);
            var data = Encoding.UTF8.GetBytes(plaintext);
            var enc = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(enc);
        }

        public string RsaDecrypt(string base64Cipher, string privatePem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privatePem);
            var enc = Convert.FromBase64String(base64Cipher);
            var dec = rsa.Decrypt(enc, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(dec);
        }

        // Подпись/проверка подписи
        public string RsaSign(string text, string privatePem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privatePem);
            var data = Encoding.UTF8.GetBytes(text);
            var sig = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(sig);
        }

        public bool RsaVerify(string text, string base64Signature, string publicPem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicPem);
            var data = Encoding.UTF8.GetBytes(text);
            var sig = Convert.FromBase64String(base64Signature);
            return rsa.VerifyData(data, sig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
