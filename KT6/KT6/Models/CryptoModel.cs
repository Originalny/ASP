namespace KT6.Models
{
    public class CryptoModel
    {
        public string UserInput { get; set; }
        public string Key { get; set; }
        public string EncryptedText { get; set; }
        public string DecryptedText { get; set; }
        public string Algorythm { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public CryptoModel() { }

        public CryptoModel(string userInput, string key, string encryptedText, string decryptedText, string algorythm, string publicKey, string privateKey)
        {
            UserInput = userInput;
            Key = key;
            EncryptedText = encryptedText;
            DecryptedText = decryptedText;
            Algorythm = algorythm;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }
}
