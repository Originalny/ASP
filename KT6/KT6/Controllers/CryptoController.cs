using Microsoft.AspNetCore.Mvc;
using KT6.Models;
using System.Security.Cryptography;
using System.Text;

namespace KT6.Controllers
{
    public class CryptoController : Controller
    {
        public IActionResult Index()
        {
            return View(new CryptoModel()); 
        }

        [HttpPost]
        public IActionResult Index(CryptoModel model)
        {
            if (model.Algorythm == "AES")
            {
                if (string.IsNullOrEmpty(model.Key))
                {
                    model.Key = GenerateRandomKey();
                }

                model.EncryptedText = EncryptAES(model.UserInput, model.Key);
                model.DecryptedText = DecryptAES(model.EncryptedText, model.Key);
            }
            else if (model.Algorythm == "RSA")
            {
                string privateKey, publicKey;
                
                model.EncryptedText = EncryptRSA(model.UserInput, out publicKey, out privateKey);

                model.PublicKey = publicKey;
                model.PrivateKey = privateKey;

                model.DecryptedText = DecryptRSA(model.EncryptedText, model.PrivateKey);
            }

            return View(model);
        }

        private string GenerateRandomKey()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();

            return Convert.ToBase64String(aes.Key);
        }

        private string EncryptAES(string input, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.GenerateIV();

            var iv = aes.IV;
            var encryptor = aes.CreateEncryptor(aes.Key, iv);
            var plainBytes = Encoding.UTF8.GetBytes(input);

            var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var result = new byte[iv.Length + encrypted.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        private string DecryptAES(string text, string key)
        {
            var fullCypher = Convert.FromBase64String(text);

            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);

            var iv = new byte[aes.BlockSize / 8];
            var cypher = new byte[fullCypher.Length - iv.Length];

            Buffer.BlockCopy(fullCypher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCypher, iv.Length, cypher, 0, cypher.Length);

            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decrypted = decryptor.TransformFinalBlock(cypher, 0, cypher.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        private string EncryptRSA(string text, out string publicKey, out string privateKey)
        {
            using var rsa = RSA.Create();

            publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

            var bytes = Encoding.UTF8.GetBytes(text);
            var encrypted = rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(encrypted);
        }

        public string DecryptRSA(string encryptedText, string privateKey)
        {
            using var rsa = RSA.Create();

            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

            var bytes = Convert.FromBase64String(encryptedText);
            var decrypted = rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
