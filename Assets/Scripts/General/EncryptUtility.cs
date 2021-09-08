using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Encrypter
{
    public class EncryptUtility
    {
        public static readonly string InitEncryptKey = "0123456789ABCDEF0123456789ABCDEF";

        /// <summary>
        /// AES暗号化(Base64形式)
        /// </summary>
        public static void EncryptAesBase64(string json, string encryptKey, string iv, out string base64)
        {
            byte[] src = Encoding.UTF8.GetBytes(json);
            byte[] dst;
            EncryptAes(src, encryptKey, iv, out dst);
            base64 = Convert.ToBase64String(dst);
        }

        /// <summary>
        /// AES複合化(Base64形式)
        /// </summary>
        public static void DecryptAesBase64(string base64, string encryptKey, string iv, out string json)
        {
            byte[] src = Convert.FromBase64String(base64);
            byte[] dst;
            DecryptAes(src, encryptKey, iv, out dst);
            json = Encoding.UTF8.GetString(dst).Trim('\0');
        }

        /// <summary>
        /// AES暗号化
        /// </summary>
        public static void EncryptAes(byte[] src, string encryptKey, string iv, out byte[] dst)
        {
            dst = null;
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.Mode = CipherMode.CBC;
                rijndael.KeySize = 256;
                rijndael.BlockSize = 128;

                byte[] key = Encoding.UTF8.GetBytes(encryptKey);
                byte[] vec = Encoding.UTF8.GetBytes(iv);

                using (ICryptoTransform encryptor = rijndael.CreateEncryptor(key, vec))
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(src, 0, src.Length);
                    cs.FlushFinalBlock();
                    dst = ms.ToArray();
                }
            }
        }

        /// <summary>
        /// AES複合化
        /// </summary>
        public static void DecryptAes(byte[] src, string encryptKey, string iv, out byte[] dst)
        {
            dst = new byte[src.Length];
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.Mode = CipherMode.CBC;
                rijndael.KeySize = 256;
                rijndael.BlockSize = 128;

                byte[] key = Encoding.UTF8.GetBytes(encryptKey);
                byte[] vec = Encoding.UTF8.GetBytes(iv);

                using (ICryptoTransform decryptor = rijndael.CreateDecryptor(key, vec))
                using (MemoryStream ms = new MemoryStream(src))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    cs.Read(dst, 0, dst.Length);
                }
            }
        }

        /// <summary>
        /// 指定された文字列をMD5でハッシュ化
        /// </summary>
        /// <param name="srcStr">入力文字列</param>
        /// <returns>入力文字列のMD5ハッシュ値</returns>
        public static byte[] CalcMd5(string srcStr)
        {
            MD5 md5 = MD5.Create();

            byte[] srcBytes = Encoding.UTF8.GetBytes(srcStr);
            byte[] destBytes = md5.ComputeHash(srcBytes);

            return destBytes;
        }

        /// <summary>
        /// 指定された文字列をMD5でハッシュ化し先頭から一定文字数までを返す
        /// </summary>
        /// <param name="srcStr">入力文字列</param>
        /// <param name="len">文字数</param>
        /// <returns>入力文字列のMD5ハッシュ値の文字列</returns>
        public static string CalcMd5Str(string srcStr, int len)
        {
            MD5 md5 = MD5.Create();

            byte[] srcBytes = Encoding.UTF8.GetBytes(srcStr);
            byte[] destBytes = md5.ComputeHash(srcBytes);

            StringBuilder destStrBuilder;
            destStrBuilder = new StringBuilder();
            foreach (byte curByte in destBytes)
            {
                destStrBuilder.Append(curByte.ToString("x2"));
            }

            return destStrBuilder.ToString().Substring(0, len);
        }
    }
}