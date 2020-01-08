using Axiom.Common;
using Axiom.Entity;
using Axiom.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Text;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class QuickFormApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CountyEntity> _repository = new GenericRepository<CountyEntity>();

        //temp        
        #endregion


        [HttpGet]
        [Route("QuickFormGetPartDetail")]
        public ApiResponse<QuickFormPartDetailEntity> QuickFormGetPartDetail(string OrderNo)
        {
            var response = new ApiResponse<QuickFormPartDetailEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<QuickFormPartDetailEntity>("QuickFormGetPartDetail", param).ToList();
                if (result == null)
                {
                    result = new List<QuickFormPartDetailEntity>();
                }

                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("QuickFormGetFormList")]
        public ApiResponse<string> QuickFormGetFormList()
        {
            var response = new ApiResponse<string>();

            string DocumentRoot = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DocumentRoot"]);
            try
            {
                string[] directory = System.IO.Directory.GetDirectories(DocumentRoot);
                List<string> result = new List<string>();
                foreach (string item in directory)
                {
                    result.Add(System.IO.Path.GetFileName(item));
                }
                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("QuickFormGetDocumentListByType")]
        public ApiResponse<QuickFormDocumentListEntity> QuickFormGetDocumentListByType(string Type, int OrderNo, string PartNo)
        {
            var response = new ApiResponse<QuickFormDocumentListEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("Type", (object)Type ?? (object)DBNull.Value),
                 new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value ),
                 new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value )};

                var result = _repository.ExecuteSQL<QuickFormDocumentListEntity>("QuickFormGetDocumentListByType", param).ToList();
                if (result == null)
                {
                    result = new List<QuickFormDocumentListEntity>();
                }

                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }


        [HttpGet]
        [Route("QuickFormGetFileList")]
        public ApiResponse<QuickFormGetFileListEntity> QuickFormGetFileList()
        {
            var response = new ApiResponse<QuickFormGetFileListEntity>();
            try
            {
                var result = _repository.ExecuteSQL<QuickFormGetFileListEntity>("QuickFormGetFormList").ToList();
                if (result == null)
                {
                    result = new List<QuickFormGetFileListEntity>();
                }

                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("QuickFormGetOrderingAttorney")]
        public ApiResponse<QuickFormAttorneyListEntity> QuickFormGetOrderingAttorney(int OrderNo)
        {
            var response = new ApiResponse<QuickFormAttorneyListEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value) };

                var result = _repository.ExecuteSQL<QuickFormAttorneyListEntity>("QuickFormGetOrderingAttorney", param).ToList();
                if (result == null)
                {
                    result = new List<QuickFormAttorneyListEntity>();
                }

                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("QuickFormGetAttachFileList")]
        public ApiResponse<QuickFormDocumentAttachmentListEntity> QuickFormGetAttachFileList(int OrderNo, string PartNo)
        {
            var response = new ApiResponse<QuickFormDocumentAttachmentListEntity>();

            try
            {
                SqlParameter[] param = {new SqlParameter("OrderNumber", (object)OrderNo ?? (object)DBNull.Value ),
                 new SqlParameter("PartNumber", (object)PartNo ?? (object)DBNull.Value )};

                var result = _repository.ExecuteSQL<QuickFormDocumentAttachmentListEntity>("QuickFormGetAttachFileList", param).ToList();
                if (result == null)
                {
                    result = new List<QuickFormDocumentAttachmentListEntity>();
                }

                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route("QuickFormInsert")]
        public BaseApiResponse QuickFormInsert(List<QuickFormEntity> objQuickFormList)
        {
            var response = new BaseApiResponse();
            try
            {
                if (objQuickFormList != null)
                {
                    foreach (var item in objQuickFormList)
                    {

                        string xmlData = ConvertToXml<QuickFormEntity>.GetXMLString(new List<QuickFormEntity>() { item });
                        SqlParameter[] param = { new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value) };
                        int QuickFormID = _repository.ExecuteSQL<int>("QuickFormInsert", param).FirstOrDefault();
                        foreach (var docitem in item.documentFileList)
                        {
                            if (docitem.UploadType == 1)
                            {
                                docitem.IsSSN = item.IsSSN;
                            }
                            else if (docitem.UploadType == 2)
                            {
                                docitem.DocPath = docitem.FileDiskName;
                            }
                            else if (docitem.UploadType == 3)
                            {
                                string sourcePath = ConfigurationManager.AppSettings["AttachPathLocal"].ToString();
                                string destinationPath = ConfigurationManager.AppSettings["AttachPathServer"].ToString();
                                docitem.DocPath = new Document().MoveLocalToServerFile(docitem.FileName, docitem.BatchId, sourcePath, destinationPath, item.OrderNo, item.PartNo);
                            }
                            docitem.QuickFormID = QuickFormID;
                            docitem.OrderNo = item.OrderNo;
                            docitem.PartNo = item.PartNo;
                            docitem.DocName = docitem.FileName;
                            docitem.FileTypeId = item.FileTypeID;
                            docitem.RecordTypeID = item.RecordTypeID;
                            docitem.Pages = item.Pages;
                            docitem.IsPublic = item.IsPublic;
                            docitem.UserID = item.UserId;
                            docitem.Year1 = item.Year1;
                            docitem.Year2 = item.Year2;
                            docitem.Year3 = item.Year3;
                            docitem.Year4 = item.Year4;
                            docitem.Year5 = item.Year5;
                            docitem.Year6 = item.Year6;
                            docitem.Year7 = item.Year7;
                            docitem.Year8 = item.Year8;
                            docitem.CreatedBy = item.UserId;
                            docitem.IsSSN = item.IsSSN;
                            string xmlAttachData = ConvertToXml<QuickFormFileEntity>.GetXMLString(new List<QuickFormFileEntity>() { docitem });
                            SqlParameter[] attachparam = { new SqlParameter("xmlDataString", (object)xmlAttachData ?? (object)DBNull.Value) };
                            var result = _repository.ExecuteSQL<int>("QuickFormInsertAttachment", attachparam).FirstOrDefault();
                        }

                    }
                }
                response.Success = true;

            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        //private static string MoveLocalToServerFile(string fileName, string batchId)
        //{
        //    string fileDiskNameResult = "";
        //    string localStoragePath = ConfigurationManager.AppSettings["AttachPathLocal"].ToString();
        //    DirectoryInfo Localdirectory = new DirectoryInfo(localStoragePath);
        //    if (!Localdirectory.Exists)
        //    {
        //        Localdirectory = Directory.CreateDirectory(localStoragePath);
        //    }
        //    string serverStoragePath = ConfigurationManager.AppSettings["AttachPathServer"].ToString();
        //    if (!Directory.Exists(serverStoragePath))
        //    {
        //        Directory.CreateDirectory(serverStoragePath);
        //    }
        //    foreach (var file in Localdirectory.GetFiles())
        //    {

        //        var objFile = file.Name.Split('_');
        //        if (objFile.Length > 0 && objFile[1] == batchId && objFile[2] == fileName)
        //        {

        //            string fileDiskName = Guid.NewGuid().ToString() + file.Extension;
        //            file.CopyTo(Path.Combine(serverStoragePath, fileDiskName));
        //            fileDiskNameResult = fileDiskName;
        //            file.Delete();
        //        }
        //    }
        //    return fileDiskNameResult;
        //}

        [HttpPost]
        [Route("UploadDocumentAttachment")]
        public ApiResponse<QuickFormUploadDocumentEntity> UploadDocumentAttachment(string CreatedBy)
        {
            var response = new ApiResponse<QuickFormUploadDocumentEntity>();
            try
            {
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var httpPostedFile = HttpContext.Current.Request.Files[0];
                    var batchIdGUId = Guid.NewGuid();
                    var fileName = CreatedBy + "_" + batchIdGUId + "_" + httpPostedFile.FileName;
                    string tempStorageDirectory = ConfigurationManager.AppSettings["AttachPathLocal"].ToString();
                    if (!Directory.Exists(tempStorageDirectory))
                    {
                        Directory.CreateDirectory(tempStorageDirectory);
                    }
                    string FileSavePath = tempStorageDirectory + "/" + fileName;
                    httpPostedFile.SaveAs(FileSavePath);
                    var result = new List<QuickFormUploadDocumentEntity>();
                    result.Add(new QuickFormUploadDocumentEntity
                    {
                        batchId = batchIdGUId.ToString(),
                        name = httpPostedFile.FileName,
                        size = httpPostedFile.ContentLength,
                        type = httpPostedFile.ContentType,
                        CreatedBy = CreatedBy,
                        CreatedDate = DateTime.UtcNow
                    });
                    response.Data = result.ToList();
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route("DeleteUploadedDocument")]
        public BaseApiResponse DeleteUploadedDocument(string batchId, string fileName, string createdBy)
        {
            var response = new BaseApiResponse();
            try
            {
                string tempStorageDirectory = ConfigurationManager.AppSettings["AttachPathLocal"].ToString();
                if (!Directory.Exists(tempStorageDirectory))
                {
                    Directory.CreateDirectory(tempStorageDirectory);
                }
                var fname = createdBy + "_" + batchId + "_" + fileName;
                if (File.Exists(Path.Combine(tempStorageDirectory, fname)))
                {
                    File.Delete(Path.Combine(tempStorageDirectory, fname));
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;

        }
        [HttpPost]
        [Route("QuickFormGetPdf")]
        public HttpResponseMessage QuickFormGetPdf(QuickDocument model)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                string DocumentsRoot = ConfigurationManager.AppSettings["DocumentRoot"].ToString();
                var UploadRoot = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                var folderPath = model.Fullpath.Replace(">", "\\");
                string strQuery, strSubQuery = "";
                string filetype = "doc";
                int orderNo = model.OrderNo;
                string partId = model.PartIds;
                bool displaySSN = model.SSN;
                string revisedText = model.IsRevisedText;
                string noOfYears = !string.IsNullOrEmpty(model.Years) ? model.Years.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length.ToString() : "0";
                int amount = 0;

                //if (drForm["IsRevised"].ToString().ToLower() == "revised" || drForm["IsRevised"].ToString().ToLower() == "duplicate") //For Duplicate doc change
                //{
                //    IsRevised = 1;
                //    IsRevisedText = drForm["IsRevised"].ToString().Trim();
                //}
                //else
                //{
                //    IsRevised = 0;
                //    IsRevisedText = "";
                //}

                var documentName = string.Empty;
                string[] documentNames = model.FileName.Split('_');
                if (documentNames != null && documentNames.Length > 0)
                    documentName = documentNames[documentNames.Length - 1];

                var filePath = Path.Combine(DocumentsRoot, folderPath, /*model.FileName*/documentName);
                documentName = Path.GetFileName(filePath);



                string[] folders = model.FolderPath.Split('\\');
                var folderName = "";
                if (folders != null && folders.Length > 0)
                    folderName = folders[folders.Length - 1];

                folderName = folderName.Split('>')[0].ToString();
                var pdt = new OrderProcess().GetDocumentType(documentName, folderName);
                bool flag = pdt == QueryType.Common;
                if (pdt == QueryType.Common)
                {

                    bool isMichigan = false;
                    bool isRush = false;
                    flag = folders.Contains("Michigan");
                    if (flag)
                        isMichigan = true;
                    flag = filePath.Contains("RUSH");
                    if (flag)
                        isRush = true;
                    flag = (folders.Contains("Custodian Letters") | folders.Contains("Subpoenas"));
                    if (flag)
                        filetype = "doc";
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(1, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        flag = isMichigan && isRush;
                        if (flag)
                            strQuery = strQuery.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,7,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,7,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,7,GETDATE()), 107), 9) AS BigDate ");
                        else
                        {
                            flag = (isMichigan && !isRush);
                            if (flag)
                                strQuery = strQuery.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,14,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,14,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,14,GETDATE()), 107), 9) AS BigDate ");
                        }
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.Confirmation)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(2, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId).Replace("%%PartCnt%%", model.PartIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length.ToString());
                        if (!displaySSN)
                            strQuery = strQuery.Replace("Orders.SSN", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(Orders.SSN)),8,4) ");
                    }
                }
                else if (pdt == QueryType.FaceSheet)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(3, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.StatusLetters)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(4, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.Waiver)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(5, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.Interrogatories)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(7, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.TargetSheets)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(8, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                }
                else if (pdt == QueryType.StatusProgressReports)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(9, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId).Replace("%%PartCnt%%", model.PartIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length.ToString());
                }
                else if (pdt == QueryType.CerticicationNOD)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(10, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                }
                else if (pdt == QueryType.AttorneyOfRecords)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(6, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                }
                else if (pdt == QueryType.CollectionLetters)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(11, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                }
                else if (pdt == QueryType.Notices)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(12, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                    if (!displaySSN)
                        strQuery = new OrderProcess().ReplaceSSN(strQuery);
                }
                else if (pdt == QueryType.AttorneyForms)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(13, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId).Replace("%%ATTYNO%%", "'" + model.AttysIds + "'");
                }
                else
                    return this.Request.CreateResponse(HttpStatusCode.NotFound, "Unsupported document format.");

                var dtQuery = new OrderProcess().ExecuteSQLQuery(strQuery);
                MemoryStream ms = new MemoryStream();

                dtQuery.Columns.Add(new DataColumn("Year1", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year2", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year3", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year4", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year5", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year6", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year7", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year8", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Total_Cost", typeof(string)));

                if (dtQuery != null && dtQuery.Rows.Count > 0)
                {
                    try
                    {
                        IEnumerator enumerator = dtQuery.Rows.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            DataRow dr = (DataRow)enumerator.Current;
                            int arg_D4C_0 = 0;
                            int num = !string.IsNullOrEmpty(model.Years) ? (int)Math.Round(unchecked(Convert.ToDouble(model.Years.Split(new char[] { ',' }).Length.ToString()) - 1.0)) : 0;
                            int i = arg_D4C_0;
                            while (true)
                            {
                                int arg_D98_0 = i;
                                int num2 = num;
                                if (arg_D98_0 > num2)
                                {
                                    break;
                                }
                                if (!string.IsNullOrEmpty(model.Years))
                                {
                                    string[] year = model.Years.Split(new char[] { ',' });
                                    dr["Year" + Convert.ToString(i + 1)] = year[i];
                                }
                                i++;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator enumerator = null;
                        flag = (enumerator is IDisposable);
                        if (flag)
                            (enumerator as IDisposable).Dispose();
                    }


                    var fees = _repository.ExecuteSQL<int>("ServiceQuickFormIRSFeesByYear").FirstOrDefault();
                    if (fees > 0)
                        amount = Convert.ToInt32(noOfYears) * fees;
                    foreach (DataRow dr in dtQuery.Rows)
                    {
                        dr["Total_Cost"] = amount;
                    }

                }
                var dtsubQuery = new DataTable();
                if (pdt == QueryType.Confirmation || pdt == QueryType.TargetSheets || pdt == QueryType.StatusProgressReports)
                {
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
                    flag = !string.IsNullOrEmpty(strSubQuery);
                    if (flag)
                    {
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strSubQuery, orderNo, partId);
                        if (!displaySSN)
                            strSubQuery = new OrderProcess().ReplaceSSN(strSubQuery);
                    }
                    dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                    StringBuilder partInfo = new StringBuilder();
                    StringBuilder partInfo2 = new StringBuilder();
                    partInfo.Append("_____________________________________________________________________________\n\r");

                    if (dtsubQuery != null && dtsubQuery.Rows.Count > 0)
                    {
                        for (int a = 0; a < dtsubQuery.Rows.Count; a++)
                        {
                            string partInfoText;
                            flag = pdt == QueryType.Confirmation && documentName.ToUpper().Equals("ORDER CONFIRMATION.DOC");
                            if (flag)
                                partInfoText = Convert.ToString(dtsubQuery.Rows[a]["PartInfo1"]).Replace("scopehere", Convert.ToString(dtsubQuery.Rows[a]["scope"]));
                            else
                                partInfoText = Convert.ToString(dtsubQuery.Rows[a]["PartInfo1"]).Replace("scopehere", "");

                            partInfo.Append(partInfoText + "\n\r");
                            partInfo2.Append(Convert.ToString(dtsubQuery.Rows[a]["LocationHeader"]) + "\n");
                        }
                    }
                    var dt2 = dtQuery;
                    DataColumn dc2 = new DataColumn("PartInfo", typeof(string));
                    DataColumn dc3 = new DataColumn("PartInfo2", typeof(string));
                    dc2.AllowDBNull = true;
                    dc3.AllowDBNull = true;
                    dt2.Columns.Add(dc2);
                    dt2.Columns.Add(dc3);
                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {
                        DataRow dr = dt2.Rows[j];
                        dr["PartInfo"] = partInfo.ToString();
                        dr["PartInfo2"] = partInfo2.ToString();
                    }
                }
                else if (pdt == QueryType.Waiver)
                {
                    StringBuilder locationInfo = new StringBuilder();
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(5, "SubQuery");
                    if (!string.IsNullOrEmpty(strSubQuery))
                    {
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strSubQuery, orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int b = 0; b < dtsubQuery.Rows.Count; b++)
                            locationInfo.Append(Convert.ToString(dtsubQuery.Rows[b]["Location1"]) + '\n');

                    }
                    var dt3 = dtQuery;
                    dt3.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                    dt3.Columns.Add(new DataColumn("Location1", typeof(string)));
                    for (int k = 0; k < dt3.Rows.Count; k++)
                    {
                        DataRow dr4 = dt3.Rows[k];
                        dr4["Selected_Part"] = model.PartIds;
                        dr4["Location1"] = locationInfo;
                    }
                }
                else if (pdt == QueryType.CerticicationNOD)
                {
                    StringBuilder attyInfo = new StringBuilder();
                    attyInfo.Append('\n');
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(10, "SubQuery");
                    if (!string.IsNullOrEmpty(strSubQuery))
                    {
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strSubQuery, orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int c = 0; c < dtsubQuery.Rows.Count; c++)
                            attyInfo.Append(Convert.ToString(dtsubQuery.Rows[c]["Attorneys"]) + "\n");
                    }
                    DataTable dt4 = dtQuery;
                    dt4.Columns.Add(new DataColumn("Attorneys", typeof(string)));
                    for (int l = 0; l < dt4.Rows.Count; l++)
                    {
                        DataRow dr5 = dt4.Rows[l];
                        dr5["Attorneys"] = attyInfo;
                    }
                }
                else if (pdt == QueryType.AttorneyOfRecords)
                {
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(6, "SubQuery");
                    string[] strQueries = null;
                    StringBuilder attyInfo2 = new StringBuilder();
                    attyInfo2.Append('\n');
                    if (!string.IsNullOrEmpty(strSubQuery))
                    {
                        strQueries = strSubQuery.Split(new string[] { "--Split--" }, StringSplitOptions.RemoveEmptyEntries);
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strQueries[0], orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int d = 0; d < dtsubQuery.Rows.Count; d++)
                            attyInfo2.Append(System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(Convert.ToString(dtsubQuery.Rows[d]["AttyInfo"])));

                        attyInfo2.Append('\n');
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strQueries[1], orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int e = 0; e < dtsubQuery.Rows.Count; e++)
                            attyInfo2.Append(Convert.ToString(dtsubQuery.Rows[e]["AttyInfo"]) + "\n");
                    }
                    DataTable dt5 = dtQuery;
                    dt5.Columns.Add(new DataColumn("AttyInfo", typeof(string)));
                    for (int m = 0; m < dt5.Rows.Count; m++)
                    {
                        DataRow dr6 = dt5.Rows[m];
                        dr6["AttyInfo"] = attyInfo2;
                    }
                }
                else if (pdt == QueryType.AttorneyForms)
                {
                    StringBuilder locationInfo2 = new StringBuilder();
                    StringBuilder Part_LocName = new StringBuilder();
                    StringBuilder Part_RecordType = new StringBuilder();
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(13, "SubQuery");
                    if (!string.IsNullOrEmpty(strSubQuery))
                    {
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strSubQuery, orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int f = 0; f < dtsubQuery.Rows.Count; f++)
                        {
                            locationInfo2.Append(Convert.ToString(dtsubQuery.Rows[f]["Location1"]) + "\n");
                            Part_LocName.Append(Convert.ToString(dtsubQuery.Rows[f]["Part_LocName1"]) + "\n");
                            Part_RecordType.Append(Convert.ToString(dtsubQuery.Rows[f]["Part_RecordType"]) + "\n");
                        }

                    }
                    DataTable dt6 = dtQuery;
                    dt6.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                    dt6.Columns.Add(new DataColumn("Location1", typeof(string)));
                    dt6.Columns.Add(new DataColumn("Part_LocName1", typeof(string)));
                    dt6.Columns.Add(new DataColumn("Part_RecordType", typeof(string)));

                    for (int n = 0; n < dt6.Rows.Count; n++)
                    {
                        DataRow dr7 = dt6.Rows[n];
                        dr7["Selected_Part"] = partId;
                        dr7["Location1"] = locationInfo2;
                        dr7["Part_LocName1"] = Part_LocName;
                        dr7["Part_RecordType"] = Part_RecordType;
                    }
                }
                filePath = Path.Combine(DocumentsRoot, model.Fullpath.Replace(">", "\\"), /*model.FileName*/documentName);
                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense("Aspose.Words.lic");
                Aspose.Words.Document doc;
                try
                {
                    doc = new Aspose.Words.Document(filePath);
                }

                catch (Exception ex)
                {
                    return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");
                }
                try
                {
                    DataColumnCollection columns = dtQuery.Columns;
                    if (columns.Contains("Part_Scope"))
                    {
                        foreach (DataRow dr in dtQuery.Rows)
                        {
                            string str = Convert.ToString(dr["Part_Scope"]);
                            if (!string.IsNullOrEmpty(str))
                                dr["Part_Scope"] = new OrderProcess().ConvertScopeHTMLToString(str);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ServicLog(ex.ToString());
                }
                doc.MailMerge.Execute(dtQuery);
                flag = filetype == "doc" ? true : false;
                if (model.IsRevised == "Duplicate" || model.IsRevised == "Revised")
                {
                    InsertWatermarkText(doc, model.IsRevised);
                    doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Shading.BackgroundPatternColor = System.Drawing.Color.Empty;
                }
                // doc.Save(ms, Aspose.Words.SaveFormat.Pdf);
                if (flag)
                    doc.Save(ms, Aspose.Words.SaveFormat.Doc);
                else
                    doc.Save(ms, Aspose.Words.SaveFormat.Pdf);
                ms.Position = 0;
                string outputFileName = "printforms";
                string outputFileNameAddRevised = "";
                if (model.IsRevised == "Revised" || model.IsRevised == "Duplicate")
                    outputFileNameAddRevised = "-" + model.IsRevised;
                else
                    outputFileNameAddRevised = "";


                try
                {
                    Guid gid = Guid.NewGuid();
                    flag = (model.PartIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length > 1 || pdt == QueryType.AttorneyForms || (pdt == QueryType.Confirmation || pdt == QueryType.Waiver));
                    if (flag)
                    {
                        bool flag2 = pdt == QueryType.AttorneyForms;
                        if (flag2)
                        {
                            outputFileName = string.Format("{0}-{1}-{2}-{3}{4}", new object[]
                            {
                                        orderNo,
                                        model.PartIds.Replace(",", "-"),
                                        model.AttysIds.Replace("'", ""),
                                        Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-")+outputFileNameAddRevised,
                                        "." + filetype
                            });
                        }
                        else
                        {
                            outputFileName = string.Format("{0}-{1}-{2}{3}", new object[]
                            {
                                        orderNo,
                                        model.PartIds.Replace(",", "-"),
                                        Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-")+outputFileNameAddRevised,
                                        "." + filetype
                            });
                        }
                        flag2 = !model.EnableDocStorage;
                        if (flag2)
                        {
                            SqlParameter[] sqlParam = {  new SqlParameter("OrderId", (object)orderNo ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)0?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileName", (object)outputFileName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId",(object)model.FileTypeId?? (object)DBNull.Value)
                                                 ,new SqlParameter("IsPublic",(object)Convert.ToBoolean(model.isPublic)?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object) Convert.ToInt32(model.RecordTypeId) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileDiskName", (object)gid+"."+filetype ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)Convert.ToInt32(model.Pages) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("CreatedBy", (object)model.UserId?? (object)DBNull.Value)
                                                 };
                            _repository.ExecuteSQL<int>("InsertFile", sqlParam).FirstOrDefault();


                            flag2 = !Directory.Exists(Path.Combine(UploadRoot, Convert.ToString(orderNo)));
                            if (flag2)
                            {
                                Directory.CreateDirectory(Path.Combine(UploadRoot, Convert.ToString(orderNo)));
                            }
                            FileStream file = new FileStream(Path.Combine(UploadRoot, Convert.ToString(orderNo), gid.ToString() + "." + filetype), FileMode.Create, FileAccess.Write);

                            ms.WriteTo(file);
                            file.Close();



                        }
                        flag2 = !string.IsNullOrEmpty(model.Years);
                        if (flag2)
                        {
                            int arg_1E9D_0 = 0;
                            int num3 = (int)Math.Round(unchecked(Convert.ToDouble(model.PartIds.Split(new char[]
                            {
                                        ','
                            }).Length.ToString()) - 1.0));
                            int j = arg_1E9D_0;
                            while (true)
                            {
                                int arg_1ED9_0 = j;
                                int num2 = num3;
                                if (arg_1ED9_0 > num2)
                                {
                                    break;
                                }
                                string[] strpartIdArray = model.PartIds.Split(new char[] { ',' });
                                InsertIRSChecks(orderNo, strpartIdArray[j], amount, model.EmpId);
                                j++;
                            }
                        }
                        response.Content = new ByteArrayContent(ms.ToArray());
                        response.Content.Headers.Clear();
                        response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + outputFileName.Replace("(", "").Replace(")", "").Replace(",", "-"));
                        response.Content.Headers.Add("Content-Length", ms.Length.ToString());

                        flag2 = (string.Compare(filetype, "pdf", false) == 0);
                        if (flag2)
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));
                        else
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/msword"));

                    }
                    else
                    {
                        outputFileName = string.Format("{0}-{1}-{2}{3}", new object[]
                               {
                                    orderNo,
                                    model.PartIds,
                                    Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-")+outputFileNameAddRevised,
                                    "." + filetype
                               });
                        bool flag2 = !model.EnableDocStorage;
                        if (flag2)
                        {
                            SqlParameter[] sqlParam = {  new SqlParameter("OrderId", (object)orderNo ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)model.PartIds?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileName", (object)outputFileName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId",(object)model.FileTypeId?? (object)DBNull.Value)
                                                 ,new SqlParameter("IsPublic",(object)Convert.ToBoolean(model.isPublic)?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object) Convert.ToInt32(model.RecordTypeId) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileDiskName", (object)gid+"."+filetype ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)Convert.ToInt32(model.Pages) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("CreatedBy", (object)model.UserId?? (object)DBNull.Value)
                                                 };
                            _repository.ExecuteSQL<int>("InsertFile", sqlParam).FirstOrDefault();
                            flag2 = !Directory.Exists(Path.Combine(UploadRoot, Convert.ToString(orderNo), model.PartIds));
                            if (flag2)
                                Directory.CreateDirectory(Path.Combine(UploadRoot, Convert.ToString(orderNo), model.PartIds));
                            FileStream file2 = new FileStream(Path.Combine(UploadRoot, Convert.ToString(orderNo), model.PartIds, gid.ToString() + "." + filetype), System.IO.FileMode.Create, System.IO.FileAccess.Write);

                            ms.WriteTo(file2);
                            file2.Close();

                        }
                        flag2 = !string.IsNullOrEmpty(model.Years);
                        if (flag2)
                        {
                            int arg_21A5_0 = 0;
                            int num4 = (int)Math.Round(unchecked(Convert.ToDouble(model.PartIds.Split(new char[]
                            {
                                        ','
                            }).Length.ToString()) - 1.0));
                            int k = arg_21A5_0;
                            while (true)
                            {
                                int arg_21E1_0 = k;
                                int num2 = num4;
                                if (arg_21E1_0 > num2)
                                {
                                    break;
                                }
                                string[] partId2 = model.PartIds.Split(new char[]
                                {
                                            ','
                                });
                                InsertIRSChecks(orderNo, partId2[k], amount, model.EmpId);
                                k++;
                            }
                        }

                        flag2 = (string.Compare(filetype, "pdf", false) == 0);


                        response.Content = new ByteArrayContent(ms.ToArray());
                        response.Content.Headers.Clear();
                        response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + outputFileName.Replace("(", "").Replace(")", "").Replace(",", "-"));
                        response.Content.Headers.Add("Content-Length", ms.Length.ToString());


                        if (flag2)
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));
                        else
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/msword"));


                    }


                }
                catch (Exception ex)
                {

                }
                return response;

            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpGet]
        [Route("QuickFormGetPdfNew")]
        public HttpResponseMessage QuickFormGetPdfNew(string jsonObject)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            QuickDocument model = js.Deserialize<QuickDocument>(jsonObject);


            // QuickDocument model =             
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                string DocumentsRoot = ConfigurationManager.AppSettings["DocumentRoot"].ToString();
                var UploadRoot = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                var folderPath = model.Fullpath.Replace(">", "\\");
                string strQuery, strSubQuery = "";
                string filetype = "doc";
                int orderNo = model.OrderNo;
                string partId = model.PartIds;
                bool displaySSN = model.SSN;
                string noOfYears = !string.IsNullOrEmpty(model.Years) ? model.Years.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length.ToString() : "0";
                int amount = 0;

                var documentName = string.Empty;
                string[] documentNames = model.FileName.Split('_');
                if (documentNames != null && documentNames.Length > 0)
                    documentName = documentNames[documentNames.Length - 1];

                var filePath = Path.Combine(DocumentsRoot, folderPath, /*model.FileName*/documentName);
                documentName = Path.GetFileName(filePath);



                string[] folders = model.FolderPath.Split('\\');
                var folderName = "";
                if (folders != null && folders.Length > 0)
                    folderName = folders[folders.Length - 1];

                folderName = folderName.Split('>')[0].ToString();
                var pdt = new OrderProcess().GetDocumentType(documentName, folderName);
                bool flag = pdt == QueryType.Common;
                if (pdt == QueryType.Common)
                {

                    bool isMichigan = false;
                    bool isRush = false;
                    flag = folders.Contains("Michigan");
                    if (flag)
                        isMichigan = true;
                    flag = filePath.Contains("RUSH");
                    if (flag)
                        isRush = true;
                    flag = (folders.Contains("Custodian Letters") | folders.Contains("Subpoenas"));
                    if (flag)
                        filetype = "doc";
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(1, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        flag = isMichigan && isRush;
                        if (flag)
                            strQuery = strQuery.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,7,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,7,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,7,GETDATE()), 107), 9) AS BigDate ");
                        else
                        {
                            flag = (isMichigan && !isRush);
                            if (flag)
                                strQuery = strQuery.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,14,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,14,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,14,GETDATE()), 107), 9) AS BigDate ");
                        }
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.Confirmation)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(2, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId).Replace("%%PartCnt%%", model.PartIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length.ToString());
                        if (!displaySSN)
                            strQuery = strQuery.Replace("Orders.SSN", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(Orders.SSN)),8,4) ");
                    }
                }
                else if (pdt == QueryType.FaceSheet)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(3, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.StatusLetters)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(4, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.Waiver)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(5, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.Interrogatories)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(7, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                    {
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                        if (!displaySSN)
                            strQuery = new OrderProcess().ReplaceSSN(strQuery);
                    }
                }
                else if (pdt == QueryType.TargetSheets)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(8, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                }
                else if (pdt == QueryType.StatusProgressReports)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(9, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId).Replace("%%PartCnt%%", model.PartIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length.ToString());
                }
                else if (pdt == QueryType.CerticicationNOD)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(10, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                }
                else if (pdt == QueryType.AttorneyOfRecords)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(6, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                }
                else if (pdt == QueryType.CollectionLetters)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(11, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                }
                else if (pdt == QueryType.Notices)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(12, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId);
                    if (!displaySSN)
                        strQuery = new OrderProcess().ReplaceSSN(strQuery);
                }
                else if (pdt == QueryType.AttorneyForms)
                {
                    strQuery = new OrderProcess().GetQueryByQueryTypeId(13, "Query");
                    flag = !string.IsNullOrEmpty(strQuery);
                    if (flag)
                        strQuery = new OrderProcess().ReplaceOrderPartNo(strQuery, orderNo, partId).Replace("%%ATTYNO%%", "'" + model.AttysIds + "'");
                }
                else
                    return this.Request.CreateResponse(HttpStatusCode.NotFound, "Unsupported document format.");

                var dtQuery = new OrderProcess().ExecuteSQLQuery(strQuery);
                MemoryStream ms = new MemoryStream();

                dtQuery.Columns.Add(new DataColumn("Year1", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year2", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year3", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year4", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year5", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year6", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year7", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Year8", typeof(string)));
                dtQuery.Columns.Add(new DataColumn("Total_Cost", typeof(string)));

                if (dtQuery != null && dtQuery.Rows.Count > 0)
                {
                    try
                    {
                        IEnumerator enumerator = dtQuery.Rows.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            DataRow dr = (DataRow)enumerator.Current;
                            int arg_D4C_0 = 0;
                            int num = !string.IsNullOrEmpty(model.Years) ? (int)Math.Round(unchecked(Convert.ToDouble(model.Years.Split(new char[] { ',' }).Length.ToString()) - 1.0)) : 0;
                            int i = arg_D4C_0;
                            while (true)
                            {
                                int arg_D98_0 = i;
                                int num2 = num;
                                if (arg_D98_0 > num2)
                                {
                                    break;
                                }
                                if (!string.IsNullOrEmpty(model.Years))
                                {
                                    string[] year = model.Years.Split(new char[] { ',' });
                                    dr["Year" + Convert.ToString(i + 1)] = year[i];
                                }
                                i++;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator enumerator = null;
                        flag = (enumerator is IDisposable);
                        if (flag)
                            (enumerator as IDisposable).Dispose();
                    }


                    var fees = _repository.ExecuteSQL<int>("ServiceQuickFormIRSFeesByYear").FirstOrDefault();
                    if (fees > 0)
                        amount = Convert.ToInt32(noOfYears) * fees;
                    foreach (DataRow dr in dtQuery.Rows)
                    {
                        dr["Total_Cost"] = amount;
                    }

                }
                var dtsubQuery = new DataTable();
                if (pdt == QueryType.Confirmation || pdt == QueryType.TargetSheets || pdt == QueryType.StatusProgressReports)
                {
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
                    flag = !string.IsNullOrEmpty(strSubQuery);
                    if (flag)
                    {
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strSubQuery, orderNo, partId);
                        if (!displaySSN)
                            strSubQuery = new OrderProcess().ReplaceSSN(strSubQuery);
                    }
                    dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                    StringBuilder partInfo = new StringBuilder();
                    StringBuilder partInfo2 = new StringBuilder();
                    partInfo.Append("_____________________________________________________________________________\n\r");

                    if (dtsubQuery != null && dtsubQuery.Rows.Count > 0)
                    {
                        for (int a = 0; a < dtsubQuery.Rows.Count; a++)
                        {
                            string partInfoText;
                            flag = pdt == QueryType.Confirmation && documentName.ToUpper().Equals("ORDER CONFIRMATION.DOC");
                            if (flag)
                                partInfoText = Convert.ToString(dtsubQuery.Rows[a]["PartInfo1"]).Replace("scopehere", Convert.ToString(dtsubQuery.Rows[a]["scope"]));
                            else
                                partInfoText = Convert.ToString(dtsubQuery.Rows[a]["PartInfo1"]).Replace("scopehere", "");

                            partInfo.Append(partInfoText + "\n\r");
                            partInfo2.Append(Convert.ToString(dtsubQuery.Rows[a]["LocationHeader"]) + "\n");
                        }
                    }
                    var dt2 = dtQuery;
                    DataColumn dc2 = new DataColumn("PartInfo", typeof(string));
                    DataColumn dc3 = new DataColumn("PartInfo2", typeof(string));
                    dc2.AllowDBNull = true;
                    dc3.AllowDBNull = true;
                    dt2.Columns.Add(dc2);
                    dt2.Columns.Add(dc3);
                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {
                        DataRow dr = dt2.Rows[j];
                        dr["PartInfo"] = partInfo.ToString();
                        dr["PartInfo2"] = partInfo2.ToString();
                    }
                }
                else if (pdt == QueryType.Waiver)
                {
                    StringBuilder locationInfo = new StringBuilder();
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(5, "SubQuery");
                    if (!string.IsNullOrEmpty(strSubQuery))
                    {
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strSubQuery, orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int b = 0; b < dtsubQuery.Rows.Count; b++)
                            locationInfo.Append(Convert.ToString(dtsubQuery.Rows[b]["Location1"]) + '\n');

                    }
                    var dt3 = dtQuery;
                    dt3.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                    dt3.Columns.Add(new DataColumn("Location1", typeof(string)));
                    for (int k = 0; k < dt3.Rows.Count; k++)
                    {
                        DataRow dr4 = dt3.Rows[k];
                        dr4["Selected_Part"] = model.PartIds;
                        dr4["Location1"] = locationInfo;
                    }
                }
                else if (pdt == QueryType.CerticicationNOD)
                {
                    StringBuilder attyInfo = new StringBuilder();
                    attyInfo.Append('\n');
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(10, "SubQuery");
                    if (!string.IsNullOrEmpty(strSubQuery))
                    {
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strSubQuery, orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int c = 0; c < dtsubQuery.Rows.Count; c++)
                            attyInfo.Append(Convert.ToString(dtsubQuery.Rows[c]["Attorneys"]) + "\n");
                    }
                    DataTable dt4 = dtQuery;
                    dt4.Columns.Add(new DataColumn("Attorneys", typeof(string)));
                    for (int l = 0; l < dt4.Rows.Count; l++)
                    {
                        DataRow dr5 = dt4.Rows[l];
                        dr5["Attorneys"] = attyInfo;
                    }
                }
                else if (pdt == QueryType.AttorneyOfRecords)
                {
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(6, "SubQuery");
                    string[] strQueries = null;
                    StringBuilder attyInfo2 = new StringBuilder();
                    attyInfo2.Append('\n');
                    if (!string.IsNullOrEmpty(strSubQuery))
                    {
                        strQueries = strSubQuery.Split(new string[] { "--Split--" }, StringSplitOptions.RemoveEmptyEntries);
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strQueries[0], orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int d = 0; d < dtsubQuery.Rows.Count; d++)
                            attyInfo2.Append(System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(Convert.ToString(dtsubQuery.Rows[d]["AttyInfo"])));

                        attyInfo2.Append('\n');
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strQueries[1], orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int e = 0; e < dtsubQuery.Rows.Count; e++)
                            attyInfo2.Append(Convert.ToString(dtsubQuery.Rows[e]["AttyInfo"]) + "\n");
                    }
                    DataTable dt5 = dtQuery;
                    dt5.Columns.Add(new DataColumn("AttyInfo", typeof(string)));
                    for (int m = 0; m < dt5.Rows.Count; m++)
                    {
                        DataRow dr6 = dt5.Rows[m];
                        dr6["AttyInfo"] = attyInfo2;
                    }
                }
                else if (pdt == QueryType.AttorneyForms)
                {
                    StringBuilder locationInfo2 = new StringBuilder();
                    StringBuilder Part_LocName = new StringBuilder();
                    StringBuilder Part_RecordType = new StringBuilder();
                    strSubQuery = new OrderProcess().GetQueryByQueryTypeId(13, "SubQuery");
                    if (!string.IsNullOrEmpty(strSubQuery))
                    {
                        strSubQuery = new OrderProcess().ReplaceOrderPartNo(strSubQuery, orderNo, partId);
                        dtsubQuery = new OrderProcess().ExecuteSQLQuery(strSubQuery);
                        for (int f = 0; f < dtsubQuery.Rows.Count; f++)
                        {
                            locationInfo2.Append(Convert.ToString(dtsubQuery.Rows[f]["Location1"]) + "\n");
                            Part_LocName.Append(Convert.ToString(dtsubQuery.Rows[f]["Part_LocName1"]) + "\n");
                            Part_RecordType.Append(Convert.ToString(dtsubQuery.Rows[f]["Part_RecordType"]) + "\n");
                        }

                    }
                    DataTable dt6 = dtQuery;
                    dt6.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                    dt6.Columns.Add(new DataColumn("Location1", typeof(string)));
                    dt6.Columns.Add(new DataColumn("Part_LocName1", typeof(string)));
                    dt6.Columns.Add(new DataColumn("Part_RecordType", typeof(string)));

                    for (int n = 0; n < dt6.Rows.Count; n++)
                    {
                        DataRow dr7 = dt6.Rows[n];
                        dr7["Selected_Part"] = partId;
                        dr7["Location1"] = locationInfo2;
                        dr7["Part_LocName1"] = Part_LocName;
                        dr7["Part_RecordType"] = Part_RecordType;
                    }
                }
                filePath = Path.Combine(DocumentsRoot, model.Fullpath.Replace(">", "\\"), /*model.FileName*/documentName);
                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense("Aspose.Words.lic");
                Aspose.Words.Document doc = new Aspose.Words.Document();
                try
                {
                    doc = Common.CommonHelper.InsertHeaderLogo(filePath, string.Format("{0}logo-axiom_{1}.png", HttpContext.Current.Server.MapPath(@"~/assets/images/"), model.CompNo));
                }

                catch (Exception ex)
                {
                    Log.ServicLog(ex.ToString());
                }
                try
                {
                    DataColumnCollection columns = dtQuery.Columns;
                    if (columns.Contains("Part_Scope"))
                    {
                        foreach (DataRow dr in dtQuery.Rows)
                        {
                            string str = Convert.ToString(dr["Part_Scope"]);
                            if (!string.IsNullOrEmpty(str))
                                dr["Part_Scope"] = new OrderProcess().ConvertScopeHTMLToString(str);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ServicLog(ex.ToString());
                }
                doc.MailMerge.Execute(dtQuery);
                flag = filetype == "doc" ? true : false;
                string revText = string.IsNullOrEmpty(model.IsRevisedText) ? "" : model.IsRevisedText.ToLower();
                if (revText == "duplicate" || revText == "revised")
                {
                    InsertWatermarkText(doc, model.IsRevisedText);
                    doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Shading.BackgroundPatternColor = System.Drawing.Color.Empty;
                }
                // doc.Save(ms, Aspose.Words.SaveFormat.Pdf);
                if (flag)
                    doc.Save(ms, Aspose.Words.SaveFormat.Doc);
                else
                    doc.Save(ms, Aspose.Words.SaveFormat.Pdf);
                ms.Position = 0;
                string outputFileName = "printforms";
                string outputFileNameAddRevised = "";
                if (model.IsRevised == "Revised" || model.IsRevised == "Duplicate")
                    outputFileNameAddRevised = "-" + model.IsRevised;
                else
                    outputFileNameAddRevised = "";


                try
                {
                    Guid gid = Guid.NewGuid();
                    flag = (model.PartIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length > 1 || pdt == QueryType.AttorneyForms || (pdt == QueryType.Confirmation || pdt == QueryType.Waiver));
                    if (flag)
                    {
                        bool flag2 = pdt == QueryType.AttorneyForms;
                        if (flag2)
                        {
                            outputFileName = string.Format("{0}-{1}-{2}-{3}{4}", new object[]
                            {
                                        orderNo,
                                        model.PartIds.Replace(",", "-"),
                                        model.AttysIds.Replace("'", ""),
                                        Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-")+outputFileNameAddRevised,
                                        "." + filetype
                            });
                        }
                        else
                        {
                            outputFileName = string.Format("{0}-{1}-{2}{3}", new object[]
                            {
                                        orderNo,
                                        model.PartIds.Replace(",", "-"),
                                        Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-")+outputFileNameAddRevised,
                                        "." + filetype
                            });
                        }
                        flag2 = !model.EnableDocStorage;
                        if (flag2)
                        {
                            SqlParameter[] sqlParam = {  new SqlParameter("OrderId", (object)orderNo ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)0?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileName", (object)outputFileName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId",(object)model.FileTypeId?? (object)DBNull.Value)
                                                 ,new SqlParameter("IsPublic",(object)Convert.ToBoolean(model.isPublic)?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object) Convert.ToInt32(model.RecordTypeId) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileDiskName", (object)gid+"."+filetype ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)Convert.ToInt32(model.Pages) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("CreatedBy", (object)model.UserId?? (object)DBNull.Value)
                                                 };
                            _repository.ExecuteSQL<int>("InsertFile", sqlParam).FirstOrDefault();


                            flag2 = !Directory.Exists(Path.Combine(UploadRoot, Convert.ToString(orderNo)));
                            if (flag2)
                            {
                                Directory.CreateDirectory(Path.Combine(UploadRoot, Convert.ToString(orderNo)));
                            }
                            FileStream file = new FileStream(Path.Combine(UploadRoot, Convert.ToString(orderNo), gid.ToString() + "." + filetype), FileMode.Create, FileAccess.Write);

                            ms.WriteTo(file);
                            file.Close();



                        }
                        flag2 = !string.IsNullOrEmpty(model.Years);
                        if (flag2)
                        {
                            int arg_1E9D_0 = 0;
                            int num3 = (int)Math.Round(unchecked(Convert.ToDouble(model.PartIds.Split(new char[]
                            {
                                        ','
                            }).Length.ToString()) - 1.0));
                            int j = arg_1E9D_0;
                            while (true)
                            {
                                int arg_1ED9_0 = j;
                                int num2 = num3;
                                if (arg_1ED9_0 > num2)
                                {
                                    break;
                                }
                                string[] strpartIdArray = model.PartIds.Split(new char[] { ',' });
                                InsertIRSChecks(orderNo, strpartIdArray[j], amount, model.EmpId);
                                j++;
                            }
                        }
                        response.Content = new ByteArrayContent(ms.ToArray());
                        response.Content.Headers.Clear();
                        response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + outputFileName.Replace("(", "").Replace(")", "").Replace(",", "-"));
                        response.Content.Headers.Add("Content-Length", ms.Length.ToString());

                        flag2 = (string.Compare(filetype, "pdf", false) == 0);
                        if (flag2)
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));
                        else
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/msword"));

                    }
                    else
                    {
                        outputFileName = string.Format("{0}-{1}-{2}{3}", new object[]
                               {
                                    orderNo,
                                    model.PartIds,
                                    Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-")+outputFileNameAddRevised,
                                    "." + filetype
                               });
                        bool flag2 = !model.EnableDocStorage;
                        if (flag2)
                        {
                            SqlParameter[] sqlParam = {  new SqlParameter("OrderId", (object)orderNo ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)model.PartIds?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileName", (object)outputFileName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId",(object)model.FileTypeId?? (object)DBNull.Value)
                                                 ,new SqlParameter("IsPublic",(object)Convert.ToBoolean(model.isPublic)?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object) Convert.ToInt32(model.RecordTypeId) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileDiskName", (object)gid+"."+filetype ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)Convert.ToInt32(model.Pages) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("CreatedBy", (object)model.UserId?? (object)DBNull.Value)
                                                 };
                            _repository.ExecuteSQL<int>("InsertFile", sqlParam).FirstOrDefault();
                            flag2 = !Directory.Exists(Path.Combine(UploadRoot, Convert.ToString(orderNo), model.PartIds));
                            if (flag2)
                                Directory.CreateDirectory(Path.Combine(UploadRoot, Convert.ToString(orderNo), model.PartIds));
                            FileStream file2 = new FileStream(Path.Combine(UploadRoot, Convert.ToString(orderNo), model.PartIds, gid.ToString() + "." + filetype), System.IO.FileMode.Create, System.IO.FileAccess.Write);

                            ms.WriteTo(file2);
                            file2.Close();

                        }
                        flag2 = !string.IsNullOrEmpty(model.Years);
                        if (flag2)
                        {
                            int arg_21A5_0 = 0;
                            int num4 = (int)Math.Round(unchecked(Convert.ToDouble(model.PartIds.Split(new char[]
                            {
                                        ','
                            }).Length.ToString()) - 1.0));
                            int k = arg_21A5_0;
                            while (true)
                            {
                                int arg_21E1_0 = k;
                                int num2 = num4;
                                if (arg_21E1_0 > num2)
                                {
                                    break;
                                }
                                string[] partId2 = model.PartIds.Split(new char[]
                                {
                                            ','
                                });
                                InsertIRSChecks(orderNo, partId2[k], amount, model.EmpId);
                                k++;
                            }
                        }

                        flag2 = (string.Compare(filetype, "pdf", false) == 0);


                        response.Content = new ByteArrayContent(ms.ToArray());
                        response.Content.Headers.Clear();
                        response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + outputFileName.Replace("(", "").Replace(")", "").Replace(",", "-"));
                        response.Content.Headers.Add("Content-Length", ms.Length.ToString());


                        if (flag2)
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));
                        else
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/msword"));


                    }


                }
                catch (Exception ex)
                {

                }
                return response;

            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }
        private static void InsertWatermarkText(Aspose.Words.Document doc, string watermarkText)
        {


            Aspose.Words.Drawing.Shape watermark = new Aspose.Words.Drawing.Shape(doc, Aspose.Words.Drawing.ShapeType.TextPlainText);
            watermark.TextPath.Text = watermarkText;
            watermark.TextPath.FontFamily = "Arial";
            watermark.Width = 500;
            watermark.Height = 100;

            watermark.Rotation = -40;


            watermark.Fill.Color = System.Drawing.Color.LightGray;
            watermark.StrokeColor = System.Drawing.Color.LightGray;
            doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Shading.BackgroundPatternColor = System.Drawing.Color.Empty;

            watermark.RelativeHorizontalPosition = Aspose.Words.Drawing.RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = Aspose.Words.Drawing.RelativeVerticalPosition.Page;
            watermark.WrapType = Aspose.Words.Drawing.WrapType.None;
            watermark.VerticalAlignment = Aspose.Words.Drawing.VerticalAlignment.Center;
            watermark.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;


            Aspose.Words.Paragraph watermarkPara = new Aspose.Words.Paragraph(doc);
            watermarkPara.AppendChild(watermark);

            ArrayList hfTypes = new ArrayList();
            hfTypes.Add(Aspose.Words.HeaderFooterType.HeaderPrimary);
            hfTypes.Add(Aspose.Words.HeaderFooterType.HeaderFirst);
            hfTypes.Add(Aspose.Words.HeaderFooterType.HeaderEven);
            foreach (Aspose.Words.Section sect in doc.Sections)
            {
                foreach (Aspose.Words.HeaderFooterType hftype in hfTypes)
                {

                    if (sect.HeadersFooters[hftype] == null)
                    {

                        if (hftype == Aspose.Words.HeaderFooterType.HeaderPrimary ||
                            hftype == Aspose.Words.HeaderFooterType.HeaderFirst && sect.PageSetup.DifferentFirstPageHeaderFooter ||
                            hftype == Aspose.Words.HeaderFooterType.HeaderEven && sect.PageSetup.OddAndEvenPagesHeaderFooter)
                        {
                            Aspose.Words.HeaderFooter hf = new Aspose.Words.HeaderFooter(doc, hftype);
                            hf.AppendChild(watermarkPara.Clone(true));
                            sect.HeadersFooters.Add(hf);
                        }
                    }
                    else
                    {

                        sect.HeadersFooters[hftype].AppendChild(watermarkPara.Clone(true));
                    }
                }
            }

        }

        private void InsertIRSChecks(int orderNo, string partNo, decimal amount, string empId)
        {

            SqlParameter[] param = {
                                         new SqlParameter("AcctNo", (object)5060 ?? (object)DBNull.Value),
                                         new SqlParameter("ChkAmt", (object)amount ?? (object)DBNull.Value ),
                                         new SqlParameter("ChkNo", (object)"" ?? (object)DBNull.Value ),
                                         new SqlParameter("ChngBy", (object)empId ?? (object)DBNull.Value ),
                                         new SqlParameter("ClrDate", (object)new DateTime(599266080000000000L) ?? (object)DBNull.Value ),
                                         new SqlParameter("EntBy", (object)empId ?? (object)DBNull.Value ),
                                         new SqlParameter("FirmID", (object)"" ?? (object)DBNull.Value ),
                                         new SqlParameter("InvNo", (object)0 ?? (object)DBNull.Value ),
                                         new SqlParameter("Memo", (object)"" ?? (object)DBNull.Value ),
                                         new SqlParameter("OrderNo", (object)orderNo ?? (object)DBNull.Value ),
                                         new SqlParameter("PartNo", (object)partNo ?? (object)DBNull.Value ),
                                         new SqlParameter("VoidDate", (object)new DateTime(599266080000000000L) ?? (object)DBNull.Value )
                                       };
            _repository.ExecuteSQL<int>("InsertIRSChecks", param).FirstOrDefault();

        }

        //[HttpPost]
        //[Route("InsertAttorney")]
        //public BaseApiResponse InsertAttorney(AttorneyMasterEntity model, string LoginEmpId, int UserID)
        //{
        //    var response = new BaseApiResponse();
        //    try
        //    {
        //        string xmlData = ConvertToXml<AttorneyMasterEntity>.GetXMLString(new List<AttorneyMasterEntity>() { model });

        //        SqlParameter[] param = {
        //                                 new SqlParameter("LoginEmpId", LoginEmpId ?? (object)DBNull.Value),
        //                                 new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value ),
        //                                 new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value)
        //                               };
        //        var result = _repository.ExecuteSQL<int>("InsertAttorney", param).FirstOrDefault();
        //        if (result > 0)
        //        {
        //            response.Success = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add(ex.Message);
        //    }
        //    return response;
        //}

        //[HttpPost]
        //[Route("UpdateAttorney")]
        //public ApiResponse<AttorneyMasterEntity> UpdateAttorney(AttorneyMasterEntity model, string LoginEmpId, int UserID)
        //{
        //    var response = new ApiResponse<AttorneyMasterEntity>();
        //    try
        //    {
        //        string xmlData = ConvertToXml<AttorneyMasterEntity>.GetXMLString(new List<AttorneyMasterEntity>() { model });

        //        SqlParameter[] param = {  new SqlParameter("LoginEmpId", (object)LoginEmpId ?? (object)DBNull.Value)
        //                                    ,new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value)
        //                                 ,new SqlParameter("LocID", (object)model.AttyID ?? (object)DBNull.Value)
        //                                 ,new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value)
        //                                };

        //        var result = _repository.ExecuteSQL<int>("UpdateAttorney", param).FirstOrDefault();
        //        if (result >  0)
        //        {
        //            // result = new List<AttorneyMasterEntity>();
        //            response.Success = true;
        //        }

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add(ex.Message);
        //    }
        //    return response;
        //}

        //[HttpGet]
        //[Route("GetAttorneyFormsList")]
        //public ApiResponse<AttorneyFormEntity> GetAttorneyFormsList(string AttyID, string FormType )
        //{
        //    var response = new ApiResponse<AttorneyFormEntity>();

        //    try
        //    {
        //        SqlParameter[] param = {    new SqlParameter("AttyID", (object)AttyID ?? (object)DBNull.Value),
        //                                    new SqlParameter("FormType", (object)FormType ?? (object)DBNull.Value) };
        //        var result = _repository.ExecuteSQL<AttorneyFormEntity>("GetAttorneyFormsList", param).ToList();
        //        if (result == null)
        //        {
        //            result = new List<AttorneyFormEntity>();
        //        }

        //        response.Success = true;
        //        response.Data = result;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add(ex.Message);
        //    }

        //    return response;

        //}

        //[HttpPost]
        //[Route("InsertAttorneyForm")]
        //public BaseApiResponse InsertAttorneyForm(AttorneyFormEntity model)
        //{
        //    var response = new BaseApiResponse();
        //    try
        //    {

        //        SqlParameter[] param = {
        //                                 new SqlParameter("AttyID", (object)model.AttyID ?? (object)DBNull.Value),
        //                                 new SqlParameter("FormType", (object)model.FormType ?? (object)DBNull.Value),
        //                                 new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value),
        //                                 new SqlParameter("FolderPath", (object)model.FolderPath ?? (object)DBNull.Value),
        //                                 new SqlParameter("FolderName", (object)model.FolderName ?? (object)DBNull.Value),
        //                                 new SqlParameter("DocFileName", (object)model.DocFileName ?? (object)DBNull.Value)
        //                               };
        //        var result = _repository.ExecuteSQL<int>("InsertAttorneyForm", param).FirstOrDefault();
        //        if (result > 0)
        //        {
        //            response.Success = true;
        //        }
        //        else if (result == -1)
        //        {
        //            response.Success = false;
        //            response.Message.Add("The document already exists for this location.");
        //        }
        //        else if (result == -2)
        //        {
        //            response.Success = false;
        //            response.Message.Add("The document does not exist in the DB(table: documents).");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add(ex.Message);
        //    }
        //    return response;

        //}


        //[HttpPost]
        //[Route("DeleteAttorneyForm")]
        //public BaseApiResponse DeleteAttorneyForm(int AttyFormID)
        //{
        //    var response = new BaseApiResponse();
        //    try
        //    {

        //        SqlParameter[] param = {
        //                                 new SqlParameter("AttyFormID", (object)AttyFormID ?? (object)DBNull.Value)

        //                               };
        //        var result = _repository.ExecuteSQL<int>("DeleteAttorneyForm", param).FirstOrDefault();
        //        if (result > 0)
        //        {
        //            response.Success = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add(ex.Message);
        //    }
        //    return response;

        //}
    }
}