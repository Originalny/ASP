using CryptoLab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoLab.Controllers.Api
{
    [ApiController]
    [Route("api/crypto")]
    public class CryptoApiController : ControllerBase
    {
        private readonly CryptoService _c;
        public CryptoApiController(CryptoService c) => _c = c;

        [HttpPost("hash")]
        public IActionResult Hash([FromBody] HashReq req)
            => Ok(new { Algorithm = req.Algorithm, Hash = _c.ComputeHash(req.Text ?? "", req.Algorithm ?? "SHA256") });

        [HttpGet("aes/key")]
        public IActionResult AesKey([FromQuery] int bits = 256)
            => Ok(new { Key = _c.GenerateAesKeyBase64(bits) });

        [HttpPost("aes/encrypt")]
        public IActionResult AesEncrypt([FromBody] AesReq req)
            => Ok(new { Cipher = _c.AesGcmEncrypt(req.Plaintext ?? "", req.Key!) });

        [HttpPost("aes/decrypt")]
        public IActionResult AesDecrypt([FromBody] AesReq req)
            => Ok(new { Plain = _c.AesGcmDecrypt(req.Cipher!, req.Key!) });

        [HttpGet("rsa/keypair")]
        public IActionResult RsaKeys([FromQuery] int size = 2048)
        { var k = _c.RsaGenerate(size); return Ok(new { PublicPem = k.publicPem, PrivatePem = k.privatePem }); }

        [HttpPost("rsa/encrypt")]
        public IActionResult RsaEncrypt([FromBody] RsaEncReq req)
            => Ok(new { Cipher = _c.RsaEncrypt(req.Plaintext!, req.PublicPem!) });

        [HttpPost("rsa/decrypt")]
        public IActionResult RsaDecrypt([FromBody] RsaDecReq req)
            => Ok(new { Plain = _c.RsaDecrypt(req.Cipher!, req.PrivatePem!) });

        [HttpPost("rsa/sign")]
        public IActionResult RsaSign([FromBody] RsaSignReq req)
            => Ok(new { Signature = _c.RsaSign(req.Message!, req.PrivatePem!) });

        [HttpPost("rsa/verify")]
        public IActionResult RsaVerify([FromBody] RsaVerifyReq req)
            => Ok(new { Valid = _c.RsaVerify(req.Message!, req.Signature!, req.PublicPem!) });

        public record HashReq(string? Text, string? Algorithm);
        public record AesReq(string? Key, string? Plaintext, string? Cipher);
        public record RsaEncReq(string? PublicPem, string? Plaintext);
        public record RsaDecReq(string? PrivatePem, string? Cipher);
        public record RsaSignReq(string? PrivatePem, string? Message);
        public record RsaVerifyReq(string? PublicPem, string? Message, string? Signature);
    }
}
