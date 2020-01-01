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
            Log.ServicLog(" TEST FOR LOGO : " + SourceDocumentFilePath);
            Document doc = new Document(SourceDocumentFilePath);
            try
            {
                License license1 = new License();
                license1.SetLicense("Aspose.Words.lic");
                
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

                var objDocumentPath = DocListNeedLog.FirstOrDefault(x => SourceDocumentFilePath.ToLower().Replace('/','\\').Contains(x.ToLower()));
                if (objDocumentPath != null)
                {
                    builder = new DocumentBuilder(doc);
                    Shape shape;
                    Log.ServicLog(objDocumentPath.ToLower());
                    switch (objDocumentPath.ToLower().Replace('/','\\'))
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
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
                return doc;
            }
        }
    }
}
