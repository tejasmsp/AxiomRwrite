using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using Aspose.Words;
using System.IO;
using Aspose.Words.Drawing;

namespace Axiom.Common
{
    public class CommonHelper
    {
        public static string SiteRootPathUrl
        {
            get
            {
                string msRootUrl;
                if (HttpContext.Current.Request.ApplicationPath != null && String.IsNullOrEmpty(HttpContext.Current.Request.ApplicationPath.Split('/')[1]))
                {
                    msRootUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host +
                                ":" +
                                HttpContext.Current.Request.Url.Port;
                }
                else
                {
                    msRootUrl = HttpContext.Current.Request.ApplicationPath;
                }

                return msRootUrl;
            }
        }

        public static string FormatAmount(string amount)
        {
            int Count = 74 - amount.Length;
            string a = "*"; string x = "";
            for (int i = 0; i < Count; i++)
            {
                x = String.Concat(x, a);
            }
            var Final = string.Concat(amount, "  ", x);
            return Final;
        }


        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static string GetCustomDateStringForXML(DateTime? date)        {            string ReturnDate = "";            if (date != null)            {                ReturnDate = date.Value.ToString("yyyy-MM-dd");            }            return ReturnDate;        }

        public static string EmailString(string emailString)
        {
            if (!string.IsNullOrEmpty(emailString))
            {
                var splitChart = emailString.FirstOrDefault(x => x == ';' || x == ',');

                string[] ccid = emailString.Split(splitChart);
                emailString = string.Join(",", ccid.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(x.Trim())).Select(x => x.Trim()).Distinct());

                return emailString;

            }
            return string.Empty;
        }

        public static string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }



        /// <summary>
        /// Insert Image in Word Document 
        /// </summary>
        /// <param name="SourceDocumentFilePath">Source/Input Document file's full path</param>
        ///// <param name="TargetDocumentFilePath">Target/Output Document file's full path</param>
        /// <param name="InputImagePath">Image full path(Which needs to be inserted in the document) </param>
        /// <param name="needtoAddHeader">Add header in page page if true  </param>
        /// <param name="ImageWidthDraw">Image-Width in the document</param>
        /// <param name="ImageHeightDraw">Image-Height in the document</param>
        /// <param name="Left">Left</param>
        /// <param name="Top">Top</param>
        /// <returns></returns>
        public static Document InsertHeaderLogo(string SourceDocumentFilePath, string InputImagePath, bool needtoAddHeader = false, double ImageWidthDraw = 120, double ImageHeightDraw = 44, double Left = 10, double Top = 10)
        //, string TargetDocumentFilePath,
        {
            License license1 = new License();
            license1.SetLicense("Aspose.Words.lic");
            Document doc = new Document(SourceDocumentFilePath);
            DocumentBuilder builder;
            RelativeHorizontalPosition h = RelativeHorizontalPosition.Page;
            RelativeVerticalPosition v = RelativeVerticalPosition.Page;
            WrapType w = WrapType.None;
            List<string> DocListNeedLog = new List<string>
            {
                 @"\Subpoenas\State\Michigan\FAX Request.doc"
                ,@"\Custodian Letters\All Letter Request Forms\FOIA or Letter Requests\FAX Request.doc"
                ,@"\Face Sheets\Face Sheet-DIGITAL ONLY - ONLINE FACE SHEET ONLY.doc"
                ,@"\Custodian Letters\All Location Letters & Faxes\Non-Compliance (HIPAA).doc"
            };

            var objDocumentPath = DocListNeedLog.FirstOrDefault(x => SourceDocumentFilePath.ToLower().Contains(x.ToLower()));
            if (objDocumentPath != null)
            {
                builder = new DocumentBuilder(doc);
                Shape shape ;

                switch (objDocumentPath.ToLower())
                {
                    case @"\subpoenas\state\michigan\fax request.doc":// 2 LOGO
                    case @"\custodian letters\all letter request forms\foia or letter requests\fax request.doc":
                    case @"\custodian letters\all location letters & faxes\non-compliance (hipaa).doc":

                        #region Header Logo common across three forms
                        ImageWidthDraw = 130;
                        ImageHeightDraw = 50;
                        Left = 45;
                        Top = 85;
                        builder.InsertImage(InputImagePath, h, Left, v, ConvertUtil.PixelToPoint(Top), ConvertUtil.PixelToPoint(ImageWidthDraw), ConvertUtil.PixelToPoint(ImageHeightDraw), w);
                        #endregion 

                        #region LargeLogo

                        builder.MoveToBookmark("LargeLogo");
                       
                   
                        if (objDocumentPath.ToLower() == @"\custodian letters\all location letters & faxes\non-compliance (hipaa).doc")
                        {
                            Left = 275;
                            ImageWidthDraw = 305;
                            ImageHeightDraw = 118;
                            Top = 50;
                        }
                        else //fax request
                        {
                            Left = 270;
                            ImageWidthDraw = 305;
                            ImageHeightDraw = 135;
                            Top = 90;
                        }

                        shape = builder.InsertImage(InputImagePath);
                        // Make the image float, put it behind text and center on the page
                        shape.WrapType = WrapType.TopBottom;
                        shape.BehindText = false;
                        shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
                        shape.HorizontalAlignment = HorizontalAlignment.Center;
                        shape.RelativeVerticalPosition = RelativeVerticalPosition.Page;
                        //shape.VerticalAlignment = VerticalAlignment.;
                        shape.Top = ConvertUtil.PixelToPoint(Top);
                        //shape.Left = ConvertUtil.PixelToPoint(Left);
                        shape.Width = ConvertUtil.PixelToPoint(ImageWidthDraw);
                        shape.Height = ConvertUtil.PixelToPoint(ImageHeightDraw);
                        #endregion 
                        
                        return doc;

                        break;
                  
                    case @"\face sheets\face sheet-digital only - online face sheet only.doc":// CENTER LOGO ONLY ONE LOGO
                        ImageWidthDraw = 305;
                        ImageHeightDraw = 110;
                        Left = 240;
                        Top = 170;

                        shape = builder.InsertImage(InputImagePath);
                        // Make the image float, put it behind text and center on the page
                        shape.WrapType = WrapType.TopBottom;
                        shape.BehindText = false;
                        shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
                        shape.HorizontalAlignment = HorizontalAlignment.Center;
                        shape.RelativeVerticalPosition = RelativeVerticalPosition.Page;
                        //shape.VerticalAlignment = VerticalAlignment.;
                        shape.Top = ConvertUtil.PixelToPoint(Top);
                        //shape.Left = ConvertUtil.PixelToPoint(Left);
                        shape.Width = ConvertUtil.PixelToPoint(ImageWidthDraw);
                        shape.Height = ConvertUtil.PixelToPoint(ImageHeightDraw); 
                        return doc;

                        break;
                     

                }
                return doc;
            }
            else
            {
                return doc;
            }


             builder = new DocumentBuilder(doc);
            if (needtoAddHeader)
            {
                Section currentSection = builder.CurrentSection;

                PageSetup pageSetup = currentSection.PageSetup;


                pageSetup.DifferentFirstPageHeaderFooter = false;


                // --- Create header for the first page. ---

                pageSetup.HeaderDistance = 100;

                builder.MoveToHeaderFooter(HeaderFooterType.HeaderFirst);

                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;


                // Set font properties for header text.

                builder.Font.Name = "Arial";

                builder.Font.Bold = true;

                builder.Font.Size = 14;


                // Specify header title for the first page.

                builder.Write("...Header Text Here....");


                // --- Create header for pages other than first. ---

                pageSetup.HeaderDistance = 20;

                builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);


                // Insert absolutely positioned image into the top/left corner of the header.

                // Distance from the top/left edges of the page is set to 10 points.

                if (File.Exists(InputImagePath))
                {

                    builder.InsertImage(InputImagePath, RelativeHorizontalPosition.Page, 10, RelativeVerticalPosition.Page, 10, ImageWidthDraw, ImageHeightDraw, WrapType.Through);
                }

                //doc.Save(TargetDocumentFilePath);
            }
            else
            {
               
                
                if (File.Exists(InputImagePath))
                {
                    
                    builder.InsertImage(InputImagePath, h, Left, v, Top, ImageWidthDraw, ImageHeightDraw, w);
                }
            }

            return doc;

        }
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



}
