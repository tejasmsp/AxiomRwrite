using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;

namespace UploadSweepService
{
    class CommonClass
    {
    }
    public class BillToAttorneyEntity
    {
        public string AttyId { get; set; }
        public string AttorneyName { get; set; }
        public string Warning { get; set; }
    }
    public class BillToAttorneyDetailsEntity
    {
        public Int64 OrderNo { get; set; }
        public string BillingFirmID { get; set; }
        public string BillingAttorneyID { get; set; }
        public string AttorneyName { get; set; }
        public string FirmName { get; set; }
        public string PatientName { get; set; }
        public string LocID { get; set; }
        public string LocationName { get; set; }
    }
    public class SoldToAttorneyDetailsEntity
    {
        public Int64 OrderNo { get; set; }
        public string OrderingAttorney { get; set; }
        public string FirmID { get; set; }
        public string FirmName { get; set; }
        public string AttorneyFirstName { get; set; }
        public string AttorneyLastName { get; set; }
        public string AttyID { get; set; }
        public string PatientName { get; set; }
    }
    public class ConvertToXml<T> where T : class, new()
    {
        static ConvertToXml()
        {


        }

        public static string GetXMLString(List<T> sourceList, string csvSelectedProperties = "")
        {

            //All numeric values in created xml was with dot symbol instead of comma
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            if (sourceList != null)
            {


                StringBuilder xmlString = new StringBuilder();
                xmlString.Append(@"<table>");

                Type sourceType;
                foreach (T item in sourceList)
                {
                    xmlString.Append("<row>");
                    sourceType = item.GetType();

                    foreach (PropertyInfo p in sourceType.GetProperties().Where(p => string.IsNullOrEmpty(csvSelectedProperties) || csvSelectedProperties.Split(',').Contains(p.Name)))
                    {
                        xmlString.Append("<" + p.Name + ">");
                        xmlString.Append(EncodeSpecialCharacter(p.GetValue(item, null)));
                        xmlString.Append("</" + p.Name + ">");
                    }
                    xmlString.Append("</row>");
                }
                xmlString.Append("</table>");

                return xmlString.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Replace Special Character as following 
        /// 1) &  =   &amp;
        /// 2) <  =   &lt;
        /// 3) >  =   &gt;
        /// 4) "  =   &quot;
        /// 5) '  =   &#39;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object EncodeSpecialCharacter(object value)
        {
            if (value != null)
            {
                string strValue = value as string;

                if (!string.IsNullOrEmpty(strValue))
                {
                    strValue = strValue.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace(@"""", "&quot;").Replace("'", "&#39;");
                    return strValue;

                }
            }
            return value;
        }
    }
    public class SoldAttorneyEntity
    {
        public string AttyId { get; set; }
        public string AttyType { get; set; }
    }
    public class GenerateInvoiceParameter
    {
        public Int64 OrderNo { get; set; }
        public int PartNo { get; set; }
        public string BillToAttorney { get; set; }
        public int CompanyNo { get; set; }
        public List<SoldAttorneyEntity> SoldAtty { get; set; }
        public int RecordTypeID { get; set; }
        public int FileVersionID { get; set; }
        public bool isFromBillingProposal { get; set; }
    }
    public class GenerateInvoiceEntity
    {
        public int InvNo { get; set; }
        public decimal OrigRate { get; set; }
        public decimal StdFee1 { get; set; }
        public decimal StdFee2 { get; set; }
        public decimal StdFee3 { get; set; }
        public decimal StdFee4 { get; set; }
        public decimal StdFee6 { get; set; }
        public int Copies { get; set; }
        public int ItemNo { get; set; }
        public int RcvdID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountForPatientAtty { get; set; }
        public string SoldAtty { get; set; }
        public string Email { get; set; }
        public string LocID { get; set; }
        public string LocationName { get; set; }
        public string DoctorName { get; set; }
        public string Descr { get; set; }
        public string BillingAttorneyID { get; set; }
        public string OrderingAttorney { get; set; }
        public string Patient { get; set; }
        public string Caption { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirmName { get; set; }
        public string InvoiceId { get; set; }
        public string WaiverAttyFirstName { get; set; }
        public string WaiverAttyLastName { get; set; }
        public string WaiverAttyID { get; set; }
        public int Pages { get; set; }
    }
    public class BillingProposalAttorneyEntity
    {
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string AttyID { get; set; }
        public string AttorneyFirstname { get; set; }
        public string AttorneyLastName { get; set; }
        public string AttorneyEmail { get; set; }
    }
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
