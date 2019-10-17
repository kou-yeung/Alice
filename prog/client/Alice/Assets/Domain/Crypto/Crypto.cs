using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo.Crypto
{
    public interface ICrypto
    {
        /// <summary>
        /// 復号する
        /// </summary>
        /// <param name="cipher"></param>
        /// <returns></returns>
        string Decrypt(string cipher);

        /// <summary>
        /// 暗号化する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string Encrypt(string text);
    }
}
