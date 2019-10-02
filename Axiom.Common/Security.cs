namespace AXIOM.Common
{
    using System;

    /// <summary>
    /// Class Security.
    /// </summary>
    public class Security
    {
        #region CONST

        /// <summary>
        /// The string tamper proof key
        /// </summary>
        private const string StrTamperProofKey = "!trustmaster";

        #endregion

        /// <summary>
        /// Dispose object to release memory
        /// </summary>
        /// <param name="object">The object.</param>
        public static void DisposeOf(object @object)
        {
            IDisposable obj = @object as IDisposable;
            if (obj != null)
            {
                obj.Dispose();
            }

            @object = null;
        }

        #region Encryption / Decryption
        
        /// <summary>
        /// Encode string
        /// </summary>
        /// <param name="strValue">This is string parameter</param>
        /// <returns>returns a string value</returns>
        public static string Encrypt(string strValue)
        {
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return string.Empty;
            }

            return TamperProofStringEncode(strValue, StrTamperProofKey);
        }

        /// <summary>
        /// Decode String
        /// </summary>
        /// <param name="strValue">This is string value</param>
        /// <returns>returns a string value</returns>
        public static string Decrypt(string strValue)
        {
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return string.Empty;
            }

            return TamperProofStringDecode(strValue, StrTamperProofKey);
        }

        /// <summary>
        /// The tamper proof string encode.
        /// </summary>
        /// <param name="strValue">The string value.</param>
        /// <param name="strKey">The string key.</param>
        /// <returns>The <see cref="string" />.</returns>
        private static string TamperProofStringEncode(string strValue, string strKey)
        {
            System.Security.Cryptography.MACTripleDES mac3Des = new System.Security.Cryptography.MACTripleDES();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            mac3Des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strKey));
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(strValue)) + Convert.ToChar("-") + Convert.ToBase64String(mac3Des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strValue)));
        }

        /// <summary>
        /// The tamper proof string decode.
        /// </summary>
        /// <param name="strValue">The string value.</param>
        /// <param name="strKey">The string key.</param>
        /// <returns>The <see cref="string" />.</returns>
        /// <exception cref="ArgumentException">exception Argument Exception</exception>
        private static string TamperProofStringDecode(string strValue, string strKey)
        {
            string strDataValue;
            string strCalcHash;
            strValue = strValue.Trim();
            strValue = strValue.Replace(" ", "+");

            System.Security.Cryptography.MACTripleDES mac3Des = new System.Security.Cryptography.MACTripleDES();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            mac3Des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strKey));

            try
            {
                strDataValue = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(strValue.Split(Convert.ToChar("-"))[0]));
                strCalcHash = System.Text.Encoding.UTF8.GetString(mac3Des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strDataValue)));

                Console.Write(strCalcHash);
            }
            catch (Exception)
            {
                return strValue;
            }

            return strDataValue;
        }

        #endregion
    }
}