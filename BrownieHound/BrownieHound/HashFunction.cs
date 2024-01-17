using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace BrownieHound
{
    public class HashFunction
    {
        public string ComputeSHA256(string plainText)
        {
            string hash = String.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                // 指定された文字列のハッシュを計算します
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // バイト配列を文字列形式に変換します
                foreach (byte b in hashValue)
                {
                    hash += $"{b:X2}";
                }
            }
            return hash;
        }

        public string ComputeHash(string plainText)
        {
            string hash = String.Empty;
            string firstHash = "";

            firstHash = ComputeSHA256(plainText);
            hash = ComputeSHA256(firstHash + plainText);
            return hash;
        }

        //メール検証処理
        public bool verifyMail(string mailAddress, string pathToConf)
        {
            bool verifiableFlg = false;
            string firstHash = "";
            string verifiedHash = "";

            firstHash = ComputeSHA256(mailAddress);
            verifiedHash = ComputeSHA256(firstHash + mailAddress);
            using (StreamReader sr = new StreamReader(pathToConf, Encoding.GetEncoding("UTF-8")))
            {
                string savedHash = sr.ReadLine();
                if (savedHash == verifiedHash)
                {
                    verifiableFlg = true;
                }
            }
            return verifiableFlg;
        }
    }
}
