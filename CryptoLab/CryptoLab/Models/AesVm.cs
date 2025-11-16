namespace CryptoLab.Models;

public class AesVm
{
    public string? PlainText { get; set; }
    public string? CipherBase64 { get; set; }
    public string? KeyBase64 { get; set; }
    public string? NonceBase64 { get; set; }

    // Для вывода результата на этой же странице
    public string? Result { get; set; }
    public string? Error { get; set; }
}
