using QuickFormService.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;
using System.Configuration;
using Aspose.BarCode;
using Aspose.Words.Drawing;
using License = Aspose.Words.License;

namespace QuickFormService
{
    public static class Common
    {

        public static string GetQueryToExecute(QueryType pdt, DataRow drForm, string[] folders, int orderNo, string partIds, bool displaySSN, string partCnt)
        {
            string query = "";
            if (pdt == QueryType.Common)
            {
                bool isMichigan = false;
                bool isRush = false;
                if (folders.Contains("Michigan"))
                    isMichigan = true;
                if (Convert.ToString(drForm["DocName"]).Contains("RUSH"))
                    isRush = true;
                //if (folders.Contains("Custodian Letters") || folders.Contains("Subpoenas"))
                //    filetype = "pdf";
                query = DbAccess.GetQueryByQueryTypeId(1, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (isMichigan && isRush)
                        query = query.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,7,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,7,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,7,GETDATE()), 107), 9) AS BigDate ");
                    else if (isMichigan && !isRush)
                        query = query.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,14,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,14,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,14,GETDATE()), 107), 9) AS BigDate ");
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else if (pdt == QueryType.Confirmation)
            {
                query = DbAccess.GetQueryByQueryTypeId(2, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds).Replace("%%PartCnt%%", Convert.ToString(partCnt));
                if (!displaySSN)
                {
                    // query = query.Replace("Orders.SSN", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(Orders.SSN)),8,4) ");
                    query = query.Replace("OP.ssn", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(OP.ssn)),8,4) ");
                }
            }
            else if (pdt == QueryType.FaceSheet)
            {
                string attyId = Convert.ToString(drForm["DocName"]).Split('_')[2];
                query = DbAccess.GetQueryByQueryTypeId(14, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (!string.IsNullOrEmpty(attyId))
                    {
                        query = query.Replace("%%ATTYNO%%", @"" + attyId + "");
                    }
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else if (pdt == QueryType.StatusLetters)
            {
                query = DbAccess.GetQueryByQueryTypeId(4, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else if (pdt == QueryType.Waiver)
            {
                query = DbAccess.GetQueryByQueryTypeId(5, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                if (!displaySSN)
                    query = ReplaceSSN(query);
            }
            else if (pdt == QueryType.Interrogatories)
            {
                query = DbAccess.GetQueryByQueryTypeId(7, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                if (!displaySSN)
                    query = ReplaceSSN(query);
            }
            else if (pdt == QueryType.TargetSheets)
            {
                query = DbAccess.GetQueryByQueryTypeId(8, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
            }
            else if (pdt == QueryType.StatusProgressReports)
            {
                query = DbAccess.GetQueryByQueryTypeId(9, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds).Replace("%%PartCnt%%", Convert.ToString(partCnt));
            }
            else if (pdt == QueryType.CerticicationNOD)
            {
                query = DbAccess.GetQueryByQueryTypeId(10, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
            }
            else if (pdt == QueryType.AttorneyOfRecords)
            {
                query = DbAccess.GetQueryByQueryTypeId(6, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
            }
            else if (pdt == QueryType.CollectionLetters)
            {
                query = DbAccess.GetQueryByQueryTypeId(11, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
            }
            else if (pdt == QueryType.Notices)
            {
                query = DbAccess.GetQueryByQueryTypeId(12, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else if (pdt == QueryType.AttorneyForms)
            {
                string attyId = Convert.ToString(drForm["DocName"]).Split('_')[2];
                query = DbAccess.GetQueryByQueryTypeId(13, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (!string.IsNullOrEmpty(attyId))
                    {
                        query = query.Replace("%%ATTYNO%%", @"" + attyId + "");
                    }
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else
                Log.ServicLog(Convert.ToString(drForm["DocName"]) + " is failed to process.");
            return query;
        }
        public static void AddFilesToPart(FileObject fileObj, string fileDiskName)
        {
            try
            {
                var obj = new FilesToPartEntity();
                obj.OrderNo = fileObj.OrderNo;
                obj.PartNo = fileObj.PartNo;
                obj.FileName = fileObj.FileName;
                obj.FileTypeId = fileObj.FileType;
                obj.RecordTypeId = fileObj.RecordType;
                obj.UserId = fileObj.UserId;
                obj.IsPublic = fileObj.IsPublic;
                obj.Pages = fileObj.Pages;
                obj.FileDiskName = fileDiskName;
                DbAccess.AddFilesToPart(obj, "ServiceQuickFormAddFilesToPart");
            }
            catch (Exception exception)
            {
                Log.ServicLog(exception.Message);
            }
        }
        public static string ReplaceOrderPartNo(string query, int OrderNo, string PartNo)
        {
            return query.Replace("%%ORDERNO%%", Convert.ToString(OrderNo)).Replace("%%PARTNO%%", Convert.ToString(PartNo));
        }
        public static string ReplaceSSN(string query)
        {
            // return query.Replace("LTRIM(RTRIM(Orders.SSN))", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(Orders.SSN)),8,4) ");
            return query.Replace("OP.ssn", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(OP.ssn)),8,4) ");

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
            DocumentBuilder builder = new DocumentBuilder(doc);
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
                RelativeHorizontalPosition h = RelativeHorizontalPosition.Page;
                RelativeVerticalPosition v = RelativeVerticalPosition.Page;
                WrapType w = WrapType.None;
                if (File.Exists(InputImagePath))
                {

                    builder.InsertImage(InputImagePath, h, 10, v, 10, ImageWidthDraw, ImageHeightDraw, w);
                }
            }

            return doc;

        }
    }
}
