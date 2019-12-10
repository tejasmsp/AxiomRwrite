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
using System.Web;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Ionic.Zip;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderDocumentApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();
        #endregion
        //InsertOrderDocument
        #region Method

        [HttpGet]
        [Route("CheckFileDownloadAccess")]
        public BaseApiResponse CheckFileDownloadAccess(int OrderNo, int PartNo, string UserID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {  new SqlParameter("OrderId", (object)OrderNo?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)PartNo?? (object)DBNull.Value)
                                                 ,new SqlParameter("UserID",(object)UserID?? (object)DBNull.Value)
                                         };
                var result = _repository.ExecuteSQL<bool>("CheckFileDownloadAccess", param).FirstOrDefault();

                if (result == null)
                {
                    result = false;
                }
                response.Success = true;
                response.str_ResponseData = result ? "true" : "false";
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }


        [HttpGet]
        [Route("GetFile")]
        public ApiResponse<FileEntity> GetFile(int OrderId = 0, int PartNo = 0)
        {
            var response = new ApiResponse<FileEntity>();
            try
            {
                SqlParameter[] param = {  new SqlParameter("OrderId", (object)OrderId?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)PartNo?? (object)DBNull.Value)
                                         };
                var result = _repository.ExecuteSQL<FileEntity>("GetFile", param).ToList();

                if (result == null)
                {
                    result = new List<FileEntity>();
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
        [Route("DownloadFileMultiple")]
        public HttpResponseMessage DownloadFileMultiple(List<DownloadMultipleFile> objFile)
        {



            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            List<DownloadMultipleFile> resultFileList = new List<DownloadMultipleFile>();
            foreach (DownloadMultipleFile item in objFile)
            {

                SqlParameter[] param = {
                    new SqlParameter("OrderId", (object)item.OrderNo ?? (object)DBNull.Value),
                    new SqlParameter("PartNo", (object)item.PartNo ?? (object)DBNull.Value),
                    new SqlParameter("FileTypeId", (object)item.FileTypeID ?? (object)DBNull.Value) };

                var result = _repository.ExecuteSQL<FileEntity>("GetClientFileByFileType", param).ToList();
                foreach (FileEntity itemFile in result)
                {
                    DownloadMultipleFile var = new DownloadMultipleFile();
                    var.FileDiskName = itemFile.FileDiskName;
                    var.FileName = itemFile.FileName;
                    var.FileTypeID = objFile[0].FileTypeID;
                    var.PartNo = Convert.ToInt32(itemFile.PartNo);
                    var.OrderNo = Convert.ToInt32(objFile[0].OrderNo);
                    resultFileList.Add(var);

                }
                if (result == null)
                {
                    result = new List<FileEntity>();
                }
            }
            ZipFile zf = new ZipFile();
            int counter = 1;
            foreach (DownloadMultipleFile item in resultFileList)
            {

                string strOrignalfileName = string.Empty;

                string fileType = Path.GetExtension(item.FileName);

                if (!string.IsNullOrEmpty(fileType) && (fileType.ToUpper() == ".DOC" || fileType.ToUpper() == ".DOCX"))
                {
                    strOrignalfileName = Path.GetFileNameWithoutExtension(item.FileName) + ".pdf";
                }
                string filePath = ConfigurationManager.AppSettings["UploadRoot"].ToString();

                if (item.PartNo > 0)
                    filePath += "/" + item.OrderNo + "/" + item.PartNo + "/" + item.FileDiskName;
                else
                    filePath += "/" + item.OrderNo + "/" + item.FileDiskName;

                try
                {
                    if (System.IO.File.Exists(filePath))
                    {

                        using (FileStream fileStream = File.OpenRead(filePath))
                        {
                            MemoryStream memStream = new MemoryStream();
                            memStream.SetLength(fileStream.Length);
                            fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);

                            if (zf.EntryFileNames.Contains(item.FileName))
                            {
                                string strFileName = item.FileName.Substring(0, item.FileName.Length - 4) + "(" + counter.ToString() + ")" + item.FileName.Substring(item.FileName.Length - 4);
                                zf.AddEntry(strFileName, memStream.GetBuffer());
                                counter++;
                            }
                            else
                            {
                                zf.AddEntry(item.FileName, memStream.GetBuffer());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            string OrderNumber = string.Empty;
            string strDate = DateTime.Now.ToString("MM/dd/yyyyHHmmss");
            try
            {
                OrderNumber = Convert.ToString(objFile[0].OrderNo);
            }
            catch (Exception ex)
            {

            }


            string ZipFileName = OrderNumber + ".zip";

            string zipFileLocation = ConfigurationManager.AppSettings["ZipFileLocation"].ToString();
            if (!System.IO.Directory.Exists(zipFileLocation))
                System.IO.Directory.CreateDirectory(zipFileLocation);

            try
            {
                string strSavepath = System.IO.Path.Combine(zipFileLocation, ZipFileName);
                zf.Save(strSavepath);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream file = new FileStream(strSavepath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);


                        httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                        httpResponseMessage.Content.Headers.Add("x-filename", ZipFileName);
                        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        httpResponseMessage.Content.Headers.ContentDisposition.FileName = ZipFileName;
                        httpResponseMessage.StatusCode = HttpStatusCode.OK;

                    }
                }
                System.IO.File.Delete(strSavepath);
            }
            catch (Exception ex)
            {

            }

            return httpResponseMessage;
        }

        [HttpGet]
        [Route("DownloadFile")]
        public HttpResponseMessage DownloadFile(string fileDiskName, string fileName, int orderNo, int partNo = 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileDiskName))
                {
                    return new Document().DownloadFileFromServer(fileDiskName, fileName, orderNo, partNo);
                }
                return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }



        [HttpPost]
        [Route("UploadDocument")]
        public BaseApiResponse UploadDocument(int CompanyNo)
        {
            var response = new BaseApiResponse();
            int FileversionID = 0;
            try
            {
                var modal = HttpContext.Current.Request.Form[0];
                var data = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(modal);
                if (data == null)
                {
                    response.Success = false;
                }
                bool isFileUploaded = HttpContext.Current.Request.Files.AllKeys.Any();
                int rcvdid = 0;
                if (Convert.ToInt32(data["PartNo"]) > 0 && Convert.ToInt32(data["FileTypeId"]) == 11)
                {

                    if (Convert.ToInt32(data["RecordTypeId"]) != 41 && Convert.ToInt32(data["RecordTypeId"]) != 137)
                    {
                        int compdateResult = 0;
                        try
                        {
                            SqlParameter[] paramCompDate = {  new SqlParameter("OrderId", (object)Convert.ToInt32(data["OrderId"]) ?? (object)DBNull.Value)
                                                        ,new SqlParameter("PartNo",(object)Convert.ToInt32(data["PartNo"])?? (object)DBNull.Value)};
                            compdateResult = _repository.ExecuteSQL<int>("UpdateCompDate", paramCompDate).FirstOrDefault();
                        }
                        catch (Exception ex)
                        {

                        }
                    }


                    try
                    {


                        SqlParameter[] param = {  new SqlParameter("OrderId", (object)Convert.ToInt32(data["OrderId"]) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)Convert.ToInt32(data["PartNo"])?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object)Convert.ToInt32(data["RecordTypeId"])  ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)Convert.ToInt32(data["PageNo"]) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("EmpId", (object)data["EmpId"].ToString() ?? (object)DBNull.Value)
                                                 ,new SqlParameter("isFileUploaded", (object)isFileUploaded ?? (object)DBNull.Value)

                                                 };
                        rcvdid = _repository.ExecuteSQL<int>("InsertFileRCVD", param).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {

                    }
                }

                string tempPath = ConfigurationManager.AppSettings["TempStorageDirectory"].ToString();
                string serverPath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                string subFolder = Convert.ToInt32(data["PartNo"]) <= 0 ? "/" + data["OrderId"].ToString() : "/" + data["OrderId"].ToString() + "/" + data["PartNo"].ToString();
                serverPath += subFolder;
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                string tempFullPath = tempPath + subFolder;
                if (!Directory.Exists(tempFullPath))
                {
                    Directory.CreateDirectory(tempFullPath);
                }
                Guid CreatedByGUID = new Guid(data["CreatedBy"].ToString());
                int RecordTypeID = 0;

                try
                {
                    RecordTypeID = Convert.ToInt32(data["RecordTypeId"]);
                }
                catch (Exception Ex)
                {

                }



                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var httpPostedFile = HttpContext.Current.Request.Files[i];
                        string FileExtension = System.IO.Path.GetExtension(httpPostedFile.FileName);
                        string fileType = FileExtension;
                        string FileName = httpPostedFile.FileName;
                        string fileGuid = Guid.NewGuid().ToString();

                        string fDiskName = fileGuid + FileExtension;
                        httpPostedFile.SaveAs(tempPath + "/" + subFolder + "/" + fileGuid + FileExtension);
                        if (!string.IsNullOrEmpty(FileExtension) && (FileExtension.ToUpper() == ".DOC" || FileExtension.ToUpper() == ".DOCX"))
                        {
                            FileExtension = ".Pdf";
                        }
                        string FileDiskName = fileGuid + FileExtension;   //DateTime.Now.ToString("yyyyMMddHHmmssfff") 


                        SqlParameter[] param = {  new SqlParameter("OrderId", (object)Convert.ToInt32(data["OrderId"]) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)Convert.ToInt32(data["PartNo"])?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileName", (object)FileName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId",(object)Convert.ToInt32( data["FileTypeId"])?? (object)DBNull.Value)
                                                 ,new SqlParameter("IsPublic",(object)Convert.ToBoolean( data["IsPublic"])?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object) Convert.ToInt32(data["RecordTypeId"]) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileDiskName", (object)FileDiskName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)Convert.ToInt32(data["PageNo"]) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("CreatedBy", (object)CreatedByGUID?? (object)DBNull.Value)
                        };

                        var result = _repository.ExecuteSQL<int>("InsertFile", param).FirstOrDefault();
                        FileversionID = result;
                        if (result > 0)
                        {
                            FileDiskName = fDiskName;
                            FileExtension = fileType;
                            response.lng_InsertedId = result;
                            response.Success = true;

                            #region SaveFileTypeRecordBillFirm
                            SqlParameter[] paramBillFirm = { new SqlParameter("OrderId", (object)Convert.ToInt32(data["OrderId"]) ?? (object)DBNull.Value) };
                            var resultbillFirm = _repository.ExecuteSQL<FileTypeRecordBillFirm>("GetDetailforFileTypeRecordBillFirm", paramBillFirm).FirstOrDefault();
                            if (resultbillFirm != null)//(Convert.ToInt32(data["RecordTypeId"]) != 0)
                            {
                                string AttyName = resultbillFirm.AttorneyName;
                                string BillFirm = resultbillFirm.OrderingFirmID;
                                string ClaimNo = resultbillFirm.BillingClaimNo;
                                if (BillFirm == null || BillFirm == "")
                                {
                                    // continue;
                                }
                                System.IO.MemoryStream ms = new System.IO.MemoryStream();


                                Aspose.Pdf.License license = new Aspose.Pdf.License();
                                license.SetLicense("Aspose.Pdf.lic");

                                Aspose.Words.License license1 = new Aspose.Words.License();
                                license1.SetLicense("Aspose.Words.lic");
                                string tmpFullPath = tempPath + "/" + subFolder + "/" + FileDiskName;

                                if (!string.IsNullOrEmpty(FileExtension) && FileExtension.ToUpper() == ".DOC" || FileExtension.ToUpper() == ".DOCX")
                                {
                                    Aspose.Words.Document doc = new Aspose.Words.Document((tmpFullPath));
                                    doc.Save(Path.Combine(serverPath, fileGuid + ".pdf"));
                                    File.Delete((tmpFullPath));

                                }
                                else if (!string.IsNullOrEmpty(FileExtension) && FileExtension.ToUpper() == ".PDF")
                                {
                                    Aspose.Pdf.Document doc = new Aspose.Pdf.Document((tmpFullPath));
                                    doc.Save(Path.Combine(serverPath, fileGuid + ".pdf"));
                                    File.Delete((tmpFullPath));

                                }
                                else
                                {
                                    File.Copy((tmpFullPath), Path.Combine(serverPath, FileDiskName));
                                    File.Delete((tmpFullPath));

                                }

                                string TStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                string _storageRoot = string.Empty;
                                string FilePath = string.Empty;

                                if (ClaimNo == "")
                                {
                                    ClaimNo = "0";
                                }
                                if (FileExtension == "doc" || FileExtension == "DOC" || FileExtension == "docx" || FileExtension == "DOCX")
                                {
                                    FileExtension = "pdf";
                                }

                                if (BillFirm == "GRANCO01")
                                {
                                    _storageRoot = ConfigurationManager.AppSettings["GrangeRoot"].ToString();
                                    FilePath = _storageRoot + string.Format("{0}-{1}-{2}-{3}", ClaimNo, TStamp, data["OrderId"].ToString(), data["PartNo"].ToString() + "." + FileExtension);
                                }
                                if (BillFirm == "HANOAA01")
                                {
                                    _storageRoot = ConfigurationManager.AppSettings["HanoverRoot"].ToString();
                                    FilePath = _storageRoot + string.Format("{0}_{1}_{2}_{3}-{4}", ClaimNo, AttyName, TStamp, data["OrderId"].ToString(), data["PartNo"].ToString() + "." + FileExtension);
                                }
                                if (BillFirm == "GRANCO01" || BillFirm == "HANOAA01")
                                {
                                    System.IO.DirectoryInfo dis = new System.IO.DirectoryInfo(_storageRoot);
                                    if (!dis.Exists)
                                    {
                                        dis.Create();
                                    }
                                    int count = 1;
                                    string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                    string extension = Path.GetExtension(FilePath);
                                    string path = Path.GetDirectoryName(FilePath);
                                    string newFullPath = FilePath;

                                    while (System.IO.File.Exists(newFullPath))
                                    {
                                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                        newFullPath = Path.Combine(path, tempFileName + extension);
                                    }

                                    FileStream fs = new FileStream(newFullPath, FileMode.Create, FileAccess.Write);
                                    ms.WriteTo(fs);
                                    fs.Close();
                                }
                            }
                            #endregion
                        }
                    }
                    Directory.Delete((tempPath + "/" + data["OrderId"].ToString()), true);



                    //TODO:Send Email             
                    //http://localhost:51617/Secured/Data/GetMRBillingData.aspx/SendMailOnUploadDocument 
                    try
                    {
                        BillingApiController bc = new BillingApiController();
                        int OrderID = Convert.ToInt32(data["OrderId"]);
                        int PartNo = Convert.ToInt32(data["PartNo"]);
                        ApiResponse<BillToAttorneyDetailsEntity> objBillToDetails = bc.GetBillToAttorneyDetailsByOrderId(OrderID.ToString(), PartNo.ToString());
                        ApiResponse<SoldToAttorneyDetailsEntity> objSoldToDetails = bc.GetSoldToAttorneyDetailsByOrderId(OrderID.ToString(), PartNo.ToString());

                        ApiResponse<BillToAttorneyEntity> objBillToAttorney = new ApiResponse<BillToAttorneyEntity>();
                        ApiResponse<BillToAttorneyEntity> objSoldtoAttorney = new ApiResponse<BillToAttorneyEntity>();

                        if (objSoldToDetails.Data.Count > 0)
                        {
                            objSoldtoAttorney = bc.GetSoldToAttorneyByOrderNo(OrderID.ToString(), PartNo.ToString());
                        }

                        if (objBillToDetails.Data.Count > 0)
                        {
                            objBillToAttorney = bc.GetBillToAttorneyByFirmId(objBillToDetails.Data[0].BillingFirmID);
                        }
                        string strBilltoAttorney = objBillToAttorney.Data.Count > 0 ? objBillToAttorney.Data[0].AttyId : "";

                        List<SoldAttorneyEntity> soldAttorneyList = new List<SoldAttorneyEntity>();


                        foreach (var item in objSoldtoAttorney.Data)
                        {
                            soldAttorneyList.Add(new SoldAttorneyEntity { AttyId = item.AttyId, AttyType = "Ordering" });
                        }

                        bc.GenerateInvoice(OrderID, PartNo, strBilltoAttorney, CompanyNo, soldAttorneyList, RecordTypeID, FileversionID);

                    }
                    catch (Exception ex)
                    {
                        Log.ServicLog("========== GENERATE BILL AFTER UPLOADING DOCUMENT ==================");
                        Log.ServicLog(ex.ToString());
                    }

                }

                // GENERATE BILL FOR THESE RECORD TYPE EVEN IF NO FILE IS UPLOADED
                // RecordTypeID = 50(Cd Of Films) 
                // RecordTypeID = 41(Cancelled)
                // RecordTypeID = 168(Custodian Fee)
                else if (isFileUploaded || RecordTypeID == 50 || RecordTypeID == 41 || RecordTypeID == 168)
                {


                    #region --- GENERATE BILL ---
                    try
                    {
                        BillingApiController bc = new BillingApiController();
                        int OrderID = Convert.ToInt32(data["OrderId"]);
                        int PartNo = Convert.ToInt32(data["PartNo"]);
                        ApiResponse<BillToAttorneyDetailsEntity> objBillToDetails = bc.GetBillToAttorneyDetailsByOrderId(OrderID.ToString(), PartNo.ToString());
                        ApiResponse<SoldToAttorneyDetailsEntity> objSoldToDetails = bc.GetSoldToAttorneyDetailsByOrderId(OrderID.ToString(), PartNo.ToString());

                        ApiResponse<BillToAttorneyEntity> objBillToAttorney = new ApiResponse<BillToAttorneyEntity>();
                        ApiResponse<BillToAttorneyEntity> objSoldtoAttorney = new ApiResponse<BillToAttorneyEntity>();

                        if (objSoldToDetails.Data.Count > 0)
                        {
                            objSoldtoAttorney = bc.GetSoldToAttorneyByOrderNo(OrderID.ToString(), PartNo.ToString());
                        }

                        if (objBillToDetails.Data.Count > 0)
                        {
                            objBillToAttorney = bc.GetBillToAttorneyByFirmId(objBillToDetails.Data[0].BillingFirmID);
                        }
                        string strBilltoAttorney = objBillToAttorney.Data.Count > 0 ? objBillToAttorney.Data[0].AttyId : "";

                        List<SoldAttorneyEntity> soldAttorneyList = new List<SoldAttorneyEntity>();


                        foreach (var item in objSoldtoAttorney.Data)
                        {
                            soldAttorneyList.Add(new SoldAttorneyEntity { AttyId = item.AttyId, AttyType = "Ordering" });
                        }

                        bc.GenerateInvoice(OrderID, PartNo, strBilltoAttorney, CompanyNo, soldAttorneyList, RecordTypeID, FileversionID);

                    }
                    catch (Exception ex)
                    {
                        Log.ServicLog("========== GENERATE BILL AFTER UPLOADING DOCUMENT ==================");
                        Log.ServicLog(ex.ToString());
                    }

                    #endregion

                    response.Success = true;
                }

                #region Send Mail
                if (Convert.ToInt32(data["FileTypeId"]) == 11 && isFileUploaded)
                {
                    try
                    {
                        SqlParameter[] param = {  new SqlParameter("OrderId", (object)Convert.ToInt64(data["OrderId"]) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)Convert.ToInt32(data["PartNo"])?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId",(object)Convert.ToInt32(data["FileTypeId"])?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object)Convert.ToInt32(data["RecordTypeId"])?? (object)DBNull.Value)
                                                 };
                        var result = _repository.ExecuteSQL<AssistContactEmail>("GetAssistContactEmailList", param).ToList();
                        if (result != null && result.Any(x => x.NewRecordAvailable))
                        {
                            CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(CompanyNo);
                            string subject = "Your Records Are Available " + Convert.ToString(data["OrderId"]) + "-" + Convert.ToString(data["PartNo"]);
                            string LiveSiteURL = ConfigurationManager.AppSettings["LiveSiteURL"].ToString();

                            foreach (AssistContactEmail item in result.Where(x => x.NewRecordAvailable && !string.IsNullOrEmpty(x.AssistantEmail)))
                            {
                                System.Text.StringBuilder body = new System.Text.StringBuilder();
                                using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/BillingRecords.html")))
                                {
                                    body.Append(reader.ReadToEnd());
                                }

                                body = body.Replace("{UserName}", "Hello " + item.AssistantName + ",");
                                body = body.Replace("{LOCATION}", item.LocationName + " (" + item.LocID + ")");
                                body = body.Replace("{PATIENT}", item.PatientName);
                                body = body.Replace("{CLAIMNO}", item.BillingClaimNo);
                                body = body.Replace("{ORDERNO}", data["OrderId"] + "-" + data["PartNo"]);
                                body = body.Replace("{InvHdr}", item.InvHdr);
                                body = body.Replace("{Pages}", Convert.ToString(data["PageNo"]));
                                body = body.Replace("{LINK}", Convert.ToString(objCompany.SiteURL) + "/PartDetail?OrderId=" + data["OrderId"] + "&PartNo=" + data["PartNo"]);
                                body = body.Replace("{LogoURL}", objCompany.Logopath);
                                body = body.Replace("{ThankYou}", objCompany.ThankYouMessage);
                                body = body.Replace("{CompanyName}", objCompany.CompName);
                                body = body.Replace("{Link}", objCompany.SiteURL);

                                EmailHelper.Email.Send(CompanyNo: objCompany.CompNo
                                    , mailTo: item.AssistantEmail
                                    , body: body.ToString()
                                    , subject: subject
                                    , ccMail: ""
                                    , bccMail: "autharchive@axiomcopy.com,tejaspadia@gmail.com");

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Log.ServicLog("========== Email for UPLOADING DOCUMENT ==================");
                        Log.ServicLog(ex.ToString());
                    }
                }
                #endregion
                if (!isFileUploaded)
                    response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            
            return response;
        }

        [HttpPost]
        [Route("UpdateFileStatus")]
        public BaseApiResponse UpdateFileStatus(int FileId, bool IsPublic)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {  new SqlParameter("FileId", (object)FileId ?? (object)DBNull.Value)
                                            ,new SqlParameter("IsPublic", (object)IsPublic ?? (object)DBNull.Value)
                                        };

                var result = _repository.ExecuteSQL<int>("UpdateFileStatus", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        #endregion
    }
}
