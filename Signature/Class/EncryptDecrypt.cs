using System;
using System.Collections.Generic;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Viewer
{
    public class EncryptDecrypt
    {
        //Private strKey As [String] = "1234-13241324-1234-123"
        public static String strKey = "U2A9/R*41FD412+4-123";

        /// <summary>
        /// To encrypt the string data 
        /// </summary>
        /// <param name="strData"> string which user want to encrypt </param>
        /// <returns> Encrypted String </returns>
        /// <remarks></remarks>
        public static string Encrypt(string strData)
        {
            string strValue = "";
            if (!string.IsNullOrEmpty(strKey))
            {
                if (strKey.Length < 16)
                {
                    char c = "XXXXXXXXXXXXXXXX"[16];
                    strKey = strKey + strKey.Substring(0, 16 - strKey.Length);
                }

                if (strKey.Length > 16)
                {
                    strKey = strKey.Substring(0, 16);
                }

                // create encryption keys
                byte[] byteKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(strKey.Substring(strKey.Length - 8, 8));

                // convert data to byte array
                byte[] byteData = Encoding.UTF8.GetBytes(strData);

                // encrypt 
                DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                MemoryStream objMemoryStream = new MemoryStream();
                CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateEncryptor(byteKey, byteVector), CryptoStreamMode.Write);
                objCryptoStream.Write(byteData, 0, byteData.Length);
                objCryptoStream.FlushFinalBlock();

                // convert to string and Base64 encode
                strValue = Convert.ToBase64String(objMemoryStream.ToArray());
            }
            else
            {
                strValue = strData;
            }

            return strValue;
        }

        /// <summary>
        /// To Decrypt the Data
        /// </summary>
        /// <param name="strData"> Encrypted string which user want to Decrypt. </param>
        /// <returns> Original string </returns>
        /// <remarks></remarks>
        public static string Decrypt(string strData)
        {
            string strValue = "";
            if (!string.IsNullOrEmpty(strKey))
            {
                // convert key to 16 characters for simplicity
                if (strKey.Length < 16)
                {
                    strKey = strKey + strKey.Substring(0, 16 - strKey.Length);

                }

                if (strKey.Length > 16)
                {
                    strKey = strKey.Substring(0, 16);

                }

                // create encryption keys
                byte[] byteKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(strKey.Substring(strKey.Length - 8, 8));

                if (!string.IsNullOrEmpty(strData))
                {
                    // convert data to byte array and Base64 decode
                    byte[] byteData = new byte[strData.Length + 1];
                    try
                    {
                        byteData = Convert.FromBase64String(strData);
                    }
                    catch
                    {
                        strValue = strData;
                    }

                    // decrypt
                    DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                    MemoryStream objMemoryStream = new MemoryStream();
                    CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateDecryptor(byteKey, byteVector), CryptoStreamMode.Write);
                    objCryptoStream.Write(byteData, 0, byteData.Length);
                    objCryptoStream.FlushFinalBlock();

                    // convert to string
                    System.Text.Encoding objEncoding = System.Text.Encoding.UTF8;
                    strValue = objEncoding.GetString(objMemoryStream.ToArray());
                }
            }
            else
            {
                strValue = strData;
            }

            return strValue;
        }
    }
}