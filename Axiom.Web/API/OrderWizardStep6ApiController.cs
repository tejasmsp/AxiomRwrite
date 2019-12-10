using Axiom.Common;
using Axiom.Data.Repository;
using Axiom.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderWizardStep6ApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderWizardStep5> _repository = new GenericRepository<OrderWizardStep5>();
        #endregion


        #region Step 6 Database Methods
        [HttpGet]
        [Route("GetLocationSearch")]
        public ApiResponse<OrderWizardStep6> GetLocationSearch()
        {
            var response = new ApiResponse<OrderWizardStep6>();
            try
            {
                var result = _repository.ExecuteSQL<OrderWizardStep6>("GetLocationSearch").ToList();
                if (result == null)
                {
                    result = new List<OrderWizardStep6>();
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


        //[HttpGet]
        //[Route("GetLocationByLocID")]
        //public ApiResponse<OrderWizardStep6> GetLocationByLocID(string LocationId)
        //{
        //    var response = new ApiResponse<OrderWizardStep6>();
        //    try
        //    {
        //        SqlParameter[] param = { new SqlParameter("LocationId", (object)LocationId ?? (object)DBNull.Value) };
        //        var result = _repository.ExecuteSQL<OrderWizardStep6>("GetLocationById",param).ToList();
        //        if (result == null)
        //        {
        //            result = new List<OrderWizardStep6>();
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

        [HttpGet]
        [Route("GetOrderLocationById")]
        public ApiResponse<OrderWizardStep6> GetOrderLocationById(int PartNo = 0, Int64 OrderNo = 0)
        {
            var response = new ApiResponse<OrderWizardStep6>();
            try
            {
                SqlParameter[] param = { new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                                           ,new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<OrderWizardStep6>("GetOrderLocationById", param).ToList();
                if (result == null)
                {
                    result = new List<OrderWizardStep6>();
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
        [Route("GetOrderWizardStep6Location")]
        public ApiResponse<OrderWizardStep6> GetOrderWizardStep6Location(Int64 orderId = 0,bool hideOldPart = false)
        {
            var response = new ApiResponse<OrderWizardStep6>();
            var result = new List<OrderWizardStep6>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)orderId ?? (object)DBNull.Value)
                            ,new SqlParameter("IsRequiredRecordType", (object)0 ?? (object)DBNull.Value)
                        ,new SqlParameter("hideOldPart", (object)hideOldPart ?? (object)DBNull.Value)
                };
                result = _repository.ExecuteSQL<OrderWizardStep6>("GetOrderWizardStep6Location", param).ToList();
                if (result == null)
                {
                    result = new List<OrderWizardStep6>();
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

        //[HttpGet]
        //[Route("DeleteOppositeAttorney")]
        //public BaseApiResponse DeleteOppositeAttorney(long OrderFirmAttorneyId)
        //{
        //    var response = new BaseApiResponse();

        //    try
        //    {
        //        SqlParameter[] param = { new SqlParameter("OrderFirmAttorneyId", (object)OrderFirmAttorneyId ?? (object)DBNull.Value) };
        //        var result = _repository.ExecuteSQL<int>("DeleteOppositeAttorney", param).FirstOrDefault();

        //        if (result > 0)
        //        {
        //            response.Success = true;
        //            response.InsertedId = result;
        //        }
        //        else
        //        {
        //            response.Success = false;
        //            response.InsertedId = 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add(ex.Message);
        //    }

        //    return response;
        //}

        [HttpPost]
        [Route("InsertNewLocation")]
        public BaseApiResponse InsertNewLocation(OrderWizardStep6 model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("LocID",(object)model.LocID ??(object)DBNull.Value)
                                        ,new SqlParameter("Name1",(object)model.Name1 ??(object)DBNull.Value)
                                        ,new SqlParameter("Name2",(object)model.Name2 ??(object)DBNull.Value)
                                        ,new SqlParameter("Dept",(object)model.Dept ??(object)DBNull.Value)
                                        ,new SqlParameter("Street1",(object)model.Street1 ??(object)DBNull.Value)
                                        ,new SqlParameter("Street2",(object)model.Street2 ??(object)DBNull.Value)
                                        ,new SqlParameter("City",(object)model.City ??(object)DBNull.Value)
                                        ,new SqlParameter("State",(object)model.State ??(object)DBNull.Value)
                                        ,new SqlParameter("Zip",(object)model.Zip ??(object)DBNull.Value)
                                        ,new SqlParameter("AreaCode1",(object)model.AreaCode1 ??(object)DBNull.Value)
                                        ,new SqlParameter("PhoneNo1",(object)model.PhoneNo1 ??(object)DBNull.Value)
                                        ,new SqlParameter("AreaCode2",(object)model.AreaCode2 ??(object)DBNull.Value)
                                        ,new SqlParameter("PhoneNo2",(object)model.PhoneNo2 ??(object)DBNull.Value)
                                        ,new SqlParameter("AreaCode3",(object)model.AreaCode3 ??(object)DBNull.Value)
                                        ,new SqlParameter("FaxNo",(object)model.FaxNo ??(object)DBNull.Value)
                                        ,new SqlParameter("Comment",(object)model.Comment ??(object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy",(object)model.EmpId ??(object)DBNull.Value)

                };
                var resultlocation = _repository.ExecuteSQL<string>("InsertOrUpdateNewLocationStep6", param).FirstOrDefault();
                if (resultlocation != "")
                {
                    response.Success = true;
                    response.str_ResponseData = resultlocation;
                }


            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }
        [HttpPost]
        [Route("UpdateOrderWizardStep6LocRush")]
        public BaseApiResponse UpdateOrderWizardStep6LocRush(string OrderNo, int PartNo, bool Rush)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo",(object)OrderNo ??(object)DBNull.Value)
                                        ,new SqlParameter("PartNo",(object)PartNo ??(object)DBNull.Value)
                                        ,new SqlParameter("Rush",(object)Rush??(object)DBNull.Value)

                };
                var result = _repository.ExecuteSQL<int>("UpdateOrderWizardStep6LocRush", param).FirstOrDefault();
                if (result > 0)
                {
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
        [Route("InsertOrUpdateOrderWizardStep6")]
        public async System.Threading.Tasks.Task<BaseApiResponse> InsertOrUpdateOrderWizardStep6(OrderWizardStep6 model)
        {

            // $scope.IsAuthorizationAry = [{ Id: true, Name: 'Authorization' }, { Id: false, Name: 'Subpoena Only' }];
            // $scope.RequestMeansAry = [{ Id: 1, Name: 'Create' }, { Id: 2, Name: 'Upload' }, { Id: 3, Name: 'Upload Batch' }];

            //if (model.RequestMeansId == 3) //BATCH UPLOAD
            //{
            //    model.AsgnTo = "ORDENE";
            //    model.Note = "Input Order : Files upload via batch.";
            //}
            //else
            //{
            //    if (model.IsAuthorization == false) // CREATE OR UPLOAD SUBPOENA // if(isCreateAuthSubp = 0) 
            //    {
            //       if(isSubpoena==1) 
            //       model.AsgnTo = "ORDENE";
            //        model.Note = "Subpoena.";
            //    }
            //    else if (model.IsAuthorization == true)
            //    {
            //        if (model.RequestMeansId == 1) // CREATE AUTHORIZATION
            //        { if(iscrateauthsub == 1,isAuth==1)
            //            model.AsgnTo = "AUTHSS";
            //            model.Note = "Input Order : Authorization sent to Palntiff for signature.";
            //        }
            //        if (model.RequestMeansId == 2) // UPLOAD AUTHORIZATION
            //        {if(iscrateauthsub == 0,isAuth==1)
            //            model.AsgnTo = "REQUES";
            //            model.Note = "Input Order : Send request to location.";
            //        }
            //    }
            //}
            model.PartNo = model.PartNo == null ? 0 : model.PartNo;

            int IsAdmin = model.RoleName == "Administrator" ? 1 : 0;
            var response = new BaseApiResponse();
            try
            {
                string linklocation = string.Empty;// model.LocID + "|" + model.RecordTypeId;
                if (model.PartNo == 0)
                {
                    SqlParameter[] linkparam = { new SqlParameter("LocId", (object)model.LocID ?? (object)DBNull.Value)
                                                ,new SqlParameter("RecordTypeId", (object)model.RecordTypeId ?? (object)DBNull.Value)
                    };
                    var locationresult = _repository.ExecuteSQL<string>("GetLinkLocation", linkparam).FirstOrDefault();
                    linklocation = linklocation + ',' + Convert.ToString(locationresult);

                }
                string[] splitlocation = linklocation.Trim(',').Split(',');
                int cnt = 0;
                string addedPart = string.Empty;
                foreach (string loc in splitlocation)
                {
                    cnt++;
                    string scope = model.Scope;
                    if (cnt > 1)
                    {
                        SqlParameter[] paramscope = { new SqlParameter("OrderNo", (object)model.OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("RecType", (object)loc.Split('|')[1] ?? (object)DBNull.Value)};
                        var resultscope = _repository.ExecuteSQL<string>("GetScopeForLocation", paramscope).ToList();
                        if (resultscope != null)
                        {
                            scope = Convert.ToString(resultscope[0]);
                        }
                        else
                        {
                            scope = "";
                        }

                    }
                    SqlParameter[] param1 = { new SqlParameter("LocId",(object)loc.Split('|')[0]??(object)DBNull.Value)
                                        ,new SqlParameter("OrderLocationId",(object)model.OrderLocationId ??(object)DBNull.Value)
                                        ,new SqlParameter("OrderId",(object)model.OrderId ??(object)DBNull.Value)
                                        ,new SqlParameter("IsAuthorization",(object)model.IsAuthorization ??(object)DBNull.Value)
                                        ,new SqlParameter("RequestMeansId",(object)model.RequestMeansId ??(object)DBNull.Value)
                                        ,new SqlParameter("IsRequireAdditionalService",(object)model.IsRequireAdditionalService ??(object)DBNull.Value)
                                        ,new SqlParameter("ScopeStartDate",(object)model.ScopeStartDate ??(object)DBNull.Value)
                                        ,new SqlParameter("ScopeEndDate",(object)model.ScopeEndDate ??(object)DBNull.Value)
                                        ,new SqlParameter("Comment",(object)model.Comment ??(object)DBNull.Value)
                                        ,new SqlParameter("IsOtherChecked",(object)model.IsOtherChecked ??(object)DBNull.Value)
                                        ,new SqlParameter("RecordTypeId",(object)loc.Split('|')[1] ??(object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy",(object)model.EmpId ??(object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId",(object)model.UserAccessId ??(object)DBNull.Value)
                                        ,new SqlParameter("EmpId",(object)model.EmpId ??(object)DBNull.Value)
                                        ,new SqlParameter("AsgnTo",(object)model.AsgnTo ??(object)DBNull.Value)
                                        ,new SqlParameter("Note",(object)model.Note ??(object)DBNull.Value)
                                        ,new SqlParameter("Scope",(object)scope ??(object)DBNull.Value)
                                        ,new SqlParameter("IsAdmin",(object)IsAdmin ??(object)DBNull.Value)
                                        ,new SqlParameter("Partno",(object)model.PartNo ??(object)DBNull.Value
                                      )

                                 };
                    var result = _repository.ExecuteSQL<int>("InsertOrUpdateOrderWizardStep6", param1).FirstOrDefault();
                    addedPart += result + ",";
                    if (result > 0)
                    {
                        if (model.DocumentFileList != null && model.DocumentFileList.Count > 0)
                        {
                            foreach (var docitem in model.DocumentFileList)
                            {
                                SqlParameter[] attachparam = {  new SqlParameter("OrderNo", (object)model.OrderId ?? (object)DBNull.Value),
                                new SqlParameter("PartNo", (object)result ?? (object)DBNull.Value),
                                new SqlParameter("FileName", (object)docitem.FileName ?? (object)DBNull.Value),
                                new SqlParameter("BatchId", (object)docitem.BatchId ?? (object)DBNull.Value),
                                new SqlParameter("LocID", (object)loc.Split('|')[0]?? (object)DBNull.Value),
                                new SqlParameter("FileTypeId", (object)(docitem.IsAuthSub==true?(int)FileType.Authorization:(int)FileType.Request)  ?? (object)DBNull.Value),
                                new SqlParameter("RecordTypeId", (object)loc.Split('|')[1] ?? (object)DBNull.Value),
                                new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                            };
                                _repository.ExecuteSQL<int>("InsertLocationFiles", attachparam).FirstOrDefault();
                            }
                        }
                        response.Success = true;
                        response.lng_InsertedId = result;
                    }
                }
                SqlParameter[] paramStatus = { new SqlParameter("OrderNo", (object)model.OrderId ?? (object)DBNull.Value) };
                bool isPartAddLater = _repository.ExecuteSQL<bool>("GetOrderStatus", paramStatus).FirstOrDefault();


                // HERE RIGHT CODE FOR SUBMIT
                // GetOrderStatus
                if (isPartAddLater)
                {

                    //SqlParameter[] param = {new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                    //                     ,new SqlParameter("UpdatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                    //                    ,new SqlParameter("UserAccessId", (object)model.UserAccessId  ?? (object)DBNull.Value)};

                    //var result = _repository.ExecuteSQL<int>("SubmitOrderAddedPart", param).FirstOrDefault();

                    //await new OrderProcess().AddQuickformsForNewOrder(Convert.ToInt32(model.OrderId), false, true, addedPart.Trim(','));
                    //await new OrderProcess().OrderSummaryEmail(Convert.ToInt32(model.OrderId), model.LoggedInUserEmail , Convert.ToInt32(isPartAddLater));
                    //await new OrderProcess().ESignature(Convert.ToInt32(model.OrderId));
                }

            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }
        [HttpPost]
        [Route("DeleteDBUploadedFile")]
        public BaseApiResponse DeleteDBUploadedFile(int FileId)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("FileId", (object)FileId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteFile", param).FirstOrDefault();
                response.Success = true;
            }
            catch (Exception ex) { }
            return response;
        }


        [HttpGet]
        [Route("GetLocationListWithSearch")]
        public ApiResponse<OrderSearchClientSide> GetLocationListWithSearch(string SearchCriteria, int SearchCondition = 1, string SearchText = "", int OrderId = 0)
        {
            var response = new ApiResponse<OrderSearchClientSide>();

            try
            {
                SqlParameter[] param = { new SqlParameter("SearchCriteria", (object)SearchCriteria ?? (object)DBNull.Value),
                                         new SqlParameter("SearchCondition", (object)SearchCondition ?? (object)DBNull.Value),
                                         new SqlParameter("SearchText", (object)SearchText ?? (object)DBNull.Value),
                                         new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<OrderSearchClientSide>("GetLocationListWithSearch", param).ToList();

                if (result == null)
                {
                    result = new List<OrderSearchClientSide>();
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
        [Route("GetLocationSearchFromPart")]
        public ApiResponse<OrderSearchClientSide> GetLocationSearchFromPart(string SearchCriteria, int SearchCondition = 1, string SearchText = "")
        {
            var response = new ApiResponse<OrderSearchClientSide>();

            try
            {
                SqlParameter[] param = { new SqlParameter("SearchCriteria", (object)SearchCriteria ?? (object)DBNull.Value),
                                         new SqlParameter("SearchCondition", (object)SearchCondition ?? (object)DBNull.Value),
                                         new SqlParameter("SearchText", (object)SearchText ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<OrderSearchClientSide>("GetLocationSearchFromPart", param).ToList();

                if (result == null)
                {
                    result = new List<OrderSearchClientSide>();
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
        [Route("SaveOrderCanvasRequest")]
        public BaseApiResponse SaveOrderCanvasRequest(OrderCanvasModel model)
        {
            var response = new BaseApiResponse();
            try
            {
                //  var modal = HttpContext.Current.Request.Form[0];
                //var data = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(modal);
                //if (data == null)
                //{
                //    response.Success = false;
                //}
                //GET LAST Inserted Part No in canvasrequest
                SqlParameter[] paramPartNO = { new SqlParameter("OrderId", (object)Convert.ToInt32(model.OrderId) ?? (object)DBNull.Value) };
                var resultPart = _repository.ExecuteSQL<int>("GetLastPartNoFromCanvas", paramPartNO).FirstOrDefault();
                int PartNo = Convert.ToInt32(resultPart);
                string sourcePath = ConfigurationManager.AppSettings["AttachPathLocal"].ToString();
                //string tempPath = ConfigurationManager.AppSettings["TempStorageDirectory"].ToString();
                string serverPath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                string subFolder = PartNo <= 0 ? "/" + model.OrderId.ToString() : "/" + model.OrderId.ToString() + "/" + PartNo;
                serverPath += subFolder;
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                //string tempFullPath = tempPath + subFolder;
                //if (!Directory.Exists(HttpContext.Current.Server.MapPath(tempFullPath)))
                //{
                //    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(tempFullPath));
                //}

                //if (HttpContext.Current.Request.Files.AllKeys.Any())
                if (model.CanvasFileList.Count > 0)
                {
                    Guid CreatedByGUID = new Guid(model.userGuid);
                    foreach (var canvasdocitem in model.CanvasFileList)
                    {
                        string FileDiskName = new Document().MoveLocalToServerFile(canvasdocitem.FileName, canvasdocitem.BatchId, sourcePath, serverPath, model.OrderId, PartNo.ToString());
                        //var httpPostedFile = HttpContext.Current.Request.Files[i];
                        string FileExtension = Path.GetExtension(canvasdocitem.FileName);

                        string FileName = canvasdocitem.FileName;
                        string fileGuid = Guid.NewGuid().ToString();
                        string fDiskName = fileGuid + FileExtension;
                        //httpPostedFile.SaveAs(HttpContext.Current.Server.MapPath(tempPath + "/" + subFolder + "/" + fileGuid + FileExtension));
                        //if (!string.IsNullOrEmpty(FileExtension) && (FileExtension.ToUpper() == ".DOC" || FileExtension.ToUpper() == ".DOCX"))
                        //{
                        //    FileExtension = ".Pdf";
                        //}
                        //string FileDiskName = fileGuid + FileExtension;   //DateTime.Now.ToString("yyyyMMddHHmmssfff") 

                        SqlParameter[] sqlparam = {
                            new SqlParameter("OrderId", (object)Convert.ToInt32(model.OrderId) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo", (object)PartNo)
                                                 ,new SqlParameter("FileName", (object)FileName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId", (object) (int)FileType.Authorization)
                                                 ,new SqlParameter("IsPublic", (object)1)
                                                 ,new SqlParameter("RecordTypeId", (object)0)
                                                 ,new SqlParameter("FileDiskName", (object)FileDiskName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)0 ?? (object)DBNull.Value)
                                                 ,new SqlParameter("CreatedBy", (object)CreatedByGUID ?? (object)DBNull.Value)
                                                 };
                        var fileresult = _repository.ExecuteSQL<int>("InsertFile", sqlparam).FirstOrDefault();
                        //if (result1 == 1)
                        //{
                        //    FileDiskName = fDiskName;
                        //    FileExtension = fileType;
                        //    Aspose.Pdf.License license = new Aspose.Pdf.License();
                        //    license.SetLicense("Aspose.Pdf.lic");

                        //    Aspose.Words.License license1 = new Aspose.Words.License();
                        //    license1.SetLicense("Aspose.Words.lic");
                        //    string tmpFullPath = tempPath + "/" + subFolder + "/" + FileDiskName;

                        //    if (!string.IsNullOrEmpty(FileExtension) && FileExtension.ToUpper() == ".DOC" || FileExtension.ToUpper() == ".DOCX")
                        //    {
                        //        Aspose.Words.Document doc = new Aspose.Words.Document(HttpContext.Current.Server.MapPath(tmpFullPath));
                        //        doc.Save(Path.Combine(serverPath, fileGuid + ".pdf"));
                        //        File.Delete(HttpContext.Current.Server.MapPath(tmpFullPath));
                        //    }
                        //    else if (!string.IsNullOrEmpty(FileExtension) && FileExtension.ToUpper() == ".PDF")
                        //    {
                        //        Aspose.Pdf.Document doc = new Aspose.Pdf.Document(HttpContext.Current.Server.MapPath(tmpFullPath));
                        //        doc.Save(Path.Combine(serverPath, fileGuid + ".pdf"));
                        //        File.Delete(HttpContext.Current.Server.MapPath(tmpFullPath));
                        //    }
                        //    else
                        //    {
                        //        File.Copy(HttpContext.Current.Server.MapPath(tmpFullPath), Path.Combine(serverPath, FileDiskName));
                        //        File.Delete(HttpContext.Current.Server.MapPath(tmpFullPath));

                        //    }
                        //}
                    }
                    //Directory.Delete(HttpContext.Current.Server.MapPath(tempPath + "/" + model.OrderId.ToString()), true);
                }
                SqlParameter[] param = {          new SqlParameter("OrderId", (object)Convert.ToInt32(model.OrderId) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)PartNo  ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PkgType",(object)Convert.ToInt32(model.PkgType)  ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PkgVal", (object)Convert.ToString(model.PkgVal).Trim(',') ?? (object)DBNull.Value)
                                                 ,new SqlParameter("ZipCode", (object)model.ZipCode.ToString() ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileCount", (object)Convert.ToInt32(model.FileCount)?? (object)DBNull.Value)
                                                 ,new SqlParameter("UserAccessId", (object)Convert.ToInt32(model.UserAccessId)  ?? (object)DBNull.Value)
                                                 };
                var result = _repository.ExecuteSQL<int>("SaveOrderCanvasRequest", param).FirstOrDefault();
                if (result == 1)
                {
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("GetCanvasList")]
        public ApiResponse<CanvasRequestEntity> GetCanvasList(int OrderId = 0)
        {
            var response = new ApiResponse<CanvasRequestEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<CanvasRequestEntity>("GetCanvasList", param).ToList();

                if (result == null)
                {
                    result = new List<CanvasRequestEntity>();
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
        [Route("DeleteCanvas")]
        public BaseApiResponse DeleteCanvas(int ID = 0)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("Id", (object)ID ?? (object)DBNull.Value),
                    new SqlParameter("FileTypeId", (object)FileType.Authorization ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("DeleteCanvas", param).FirstOrDefault();
                if (result == 1)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }
        [HttpPost]
        [Route("DeleteOrderLocation")]
        public BaseApiResponse DeleteOrderLocation(int PartNo = 0, int OrderNo = 0, int UserAccessID=0)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value),
                                         new SqlParameter("FileTypeId", (object)FileType.Request ?? (object)DBNull.Value)
                                         ,new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                         ,new SqlParameter("UserAccessID", (object)UserAccessID ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("DeleteOrderLocation", param).FirstOrDefault();
                if (result == 1)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }

        [HttpPost]
        [Route("UploadNewOrderDocument")]
        public ApiResponse<QuickFormUploadDocumentEntity> UploadNewOrderDocument(string CreatedBy)
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

        private static string MoveLocalToServerFile(string fileName, string batchId)
        {
            string fileDiskNameResult = "";
            string localStoragePath = ConfigurationManager.AppSettings["AttachPathLocal"].ToString();
            DirectoryInfo Localdirectory = new DirectoryInfo(localStoragePath);
            if (!Localdirectory.Exists)
            {
                Localdirectory = Directory.CreateDirectory(localStoragePath);
            }
            string serverStoragePath = ConfigurationManager.AppSettings["AttachPathServer"].ToString();
            if (!Directory.Exists(serverStoragePath))
            {
                Directory.CreateDirectory(serverStoragePath);
            }
            foreach (var file in Localdirectory.GetFiles())
            {

                var objFile = file.Name.Split('_');
                string objFileName = string.Empty;
                if (objFile.Length > 3)
                {
                    for (int i = 0; i < objFile.Length; i++)
                    {
                        if (i > 1)
                        {
                            objFileName += objFile[i] + "_";
                        }
                    }
                    objFileName = objFileName.Trim('_');
                }
                else
                {
                    objFileName = objFile[2];
                }

                if (objFile.Length > 0 && objFile[1] == batchId && objFileName == fileName)
                {

                    string fileDiskName = Guid.NewGuid().ToString() + file.Extension;
                    file.CopyTo(Path.Combine(serverStoragePath, fileDiskName));
                    fileDiskNameResult = fileDiskName;
                    file.Delete();
                }
            }
            return fileDiskNameResult;
        }


        [HttpPost]
        [Route("RushAllLocation")]
        public BaseApiResponse RushAllLocation(int OrderID, bool Status)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderID ?? (object)DBNull.Value),
                                         new SqlParameter("Status", (object)Status ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("AllLocationRushStatusChange", param).FirstOrDefault();
                if (result == 1)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }

            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("GetLocationTempFiles")]
        public ApiResponse<LocationFilesModel> GetLocationTempFiles(int OrderNo, int PartNo)
        {
            var response = new ApiResponse<LocationFilesModel>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value),
                    new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<LocationFilesModel>("GetLocationTempFiles", param).ToList();

                if (result == null)
                {
                    result = new List<LocationFilesModel>();
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
        #endregion
    }
}
