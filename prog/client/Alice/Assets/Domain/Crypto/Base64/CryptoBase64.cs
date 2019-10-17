using System;
using System.Text;

namespace Zoo.Crypto
{

    /// <summary>
    /// Base64を使って簡易暗号化する
    /// </summary>
    public class CryptoBase64 : ICrypto
    {
        /// <summary>
        /// 復号する
        /// </summary>
        /// <param name="cipher"></param>
        /// <returns></returns>
        public string Decrypt(string cipher)
        {
            var bytes = Convert.FromBase64String(cipher);
            var text = Encoding.UTF8.GetString(bytes);
            return text;
        }

        /// <summary>
        /// 暗号化する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Encrypt(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var cipher = Convert.ToBase64String(bytes);
            return cipher;
        }
    }
}
