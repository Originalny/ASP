using CryptoLab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoLab.Controllers
{
    public class CryptoController : Controller
    {
        private readonly CryptoService _crypto;
        public CryptoController(CryptoService crypto) => _crypto = crypto;

        // ---------- HASH ----------
        [HttpGet]
        public IActionResult Hash()
        {
            ViewBag.Input = TempData["Hash_Input"];
            ViewBag.Algorithm = TempData["Hash_Alg"];
            ViewBag.Result = TempData["Hash_Result"];
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Hash(string input, string algorithm)
        {
            TempData["Hash_Input"] = input;
            TempData["Hash_Alg"] = algorithm;
            TempData["Hash_Result"] = string.IsNullOrEmpty(input) ? null : _crypto.ComputeHash(input, algorithm);
            return RedirectToAction(nameof(Hash)); // PRG
        }

        // ---------- AES ----------
        [HttpGet]
        public IActionResult Aes()
        {
            // достаём из TempData то, что положили после POST
            ViewBag.Key   = TempData["Aes_Key"]   ?? _crypto.GenerateAesKeyBase64();
            ViewBag.Plain = TempData["Aes_Plain"];
            ViewBag.Cipher= TempData["Aes_Cipher"];
            ViewBag.Error = TempData["Aes_Error"];
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult AesEncrypt(string key, string plaintext)
        {
            try
            {
                var cipher = _crypto.AesGcmEncrypt(plaintext, key);
                TempData["Aes_Key"] = key;
                TempData["Aes_Plain"] = plaintext;
                TempData["Aes_Cipher"] = cipher;
            }
            catch (Exception ex)
            {
                TempData["Aes_Error"] = ex.Message;
                TempData["Aes_Key"] = key;
                TempData["Aes_Plain"] = plaintext;
            }
            return RedirectToAction(nameof(Aes)); // PRG
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult AesDecrypt(string key, string cipher)
        {
            try
            {
                var plain = _crypto.AesGcmDecrypt(cipher, key);
                TempData["Aes_Key"] = key;
                TempData["Aes_Cipher"] = cipher;
                TempData["Aes_Plain"] = plain;
            }
            catch (Exception ex)
            {
                TempData["Aes_Error"] = ex.Message;
                TempData["Aes_Key"] = key;
                TempData["Aes_Cipher"] = cipher;
            }
            return RedirectToAction(nameof(Aes)); // PRG
        }

        // ---------- RSA ----------
        [HttpGet]
        public IActionResult Rsa()
        {
            // из TempData или сгенерировать с нуля
            ViewBag.PublicPem  = TempData["Rsa_PublicPem"]  ?? _crypto.RsaGenerate().publicPem;
            ViewBag.PrivatePem = TempData["Rsa_PrivatePem"] ?? _crypto.RsaGenerate().privatePem;
            ViewBag.Plain      = TempData["Rsa_Plain"];
            ViewBag.Cipher     = TempData["Rsa_Cipher"];
            ViewBag.Message    = TempData["Rsa_Message"];
            ViewBag.Signature  = TempData["Rsa_Signature"];
            ViewBag.Valid      = TempData["Rsa_Valid"];
            ViewBag.Error      = TempData["Rsa_Error"];
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult RsaEncrypt(string publicPem, string plaintext)
        {
            try
            {
                TempData["Rsa_PublicPem"] = publicPem;
                TempData["Rsa_Plain"]     = plaintext;
                TempData["Rsa_Cipher"]    = _crypto.RsaEncrypt(plaintext, publicPem);
            }
            catch (Exception ex) { TempData["Rsa_Error"] = ex.Message; }
            return RedirectToAction(nameof(Rsa)); // PRG
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult RsaDecrypt(string privatePem, string cipher)
        {
            try
            {
                TempData["Rsa_PrivatePem"] = privatePem;
                TempData["Rsa_Cipher"]     = cipher;
                TempData["Rsa_Plain"]      = _crypto.RsaDecrypt(cipher, privatePem);
            }
            catch (Exception ex) { TempData["Rsa_Error"] = ex.Message; }
            return RedirectToAction(nameof(Rsa));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult RsaSign(string privatePem, string message)
        {
            try
            {
                TempData["Rsa_PrivatePem"] = privatePem;
                TempData["Rsa_Message"]    = message;
                TempData["Rsa_Signature"]  = _crypto.RsaSign(message, privatePem);
            }
            catch (Exception ex) { TempData["Rsa_Error"] = ex.Message; }
            return RedirectToAction(nameof(Rsa));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult RsaVerify(string publicPem, string message, string signature)
        {
            try
            {
                TempData["Rsa_PublicPem"] = publicPem;
                TempData["Rsa_Message"]   = message;
                TempData["Rsa_Signature"] = signature;
                TempData["Rsa_Valid"]     = _crypto.RsaVerify(message, signature, publicPem);
            }
            catch (Exception ex) { TempData["Rsa_Error"] = ex.Message; }
            return RedirectToAction(nameof(Rsa));
        }
    }
}
