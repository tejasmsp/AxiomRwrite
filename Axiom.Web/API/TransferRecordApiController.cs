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
 

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class TransferRecordApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CourtEntity> _repository = new GenericRepository<CourtEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetEntityListForTrasferDropdown")]
        public ApiResponse<DropDownEntity> GetEntityListForTrasferDropdown(int EnitityTypeId,int CompanyNo =0 )
        {
            var response = new ApiResponse<DropDownEntity>();

            try
            {
               
                SqlParameter[] param = {  new SqlParameter("EnitityTypeId", (object)EnitityTypeId ?? (object)DBNull.Value) 
                                         ,new SqlParameter("CompanyNo"   , (object)CompanyNo ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<DropDownEntity>("GetEntityListForTrasferDropdown", param).ToList();
               
                if (result == null)
                {
                    result = new List<DropDownEntity>();
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
        [Route("SubmitRecordsToTransfer")]
        public ApiResponse<TransferEntity> SubmitRecordsToTransfer(TransferEntity objTransferEntity)
        {
            var response = new ApiResponse<TransferEntity>();

            //string xmlData = ConvertToXml<BillingRateDetailEntity>.GetXMLString(BillingRateList, "ID,RateMainID,RateName,RegRate,DcntRate");

            try
            {
                if (objTransferEntity.EnitityTypeId > 0 && !string.IsNullOrEmpty(objTransferEntity.SourceEntityId) && !string.IsNullOrEmpty(objTransferEntity.TargetEntityId))
                {
                        SqlParameter[] param = {
                         new SqlParameter("EnitityTypeId" , (object)objTransferEntity.EnitityTypeId ?? (object)DBNull.Value)
                        ,new SqlParameter("SourceEntityId", (object)objTransferEntity.SourceEntityId ?? (object)DBNull.Value)
                        ,new SqlParameter("TargetEntityId", (object)objTransferEntity.TargetEntityId ?? (object)DBNull.Value)
                        ,new SqlParameter("UserId"        , (object)objTransferEntity.UserId ?? (object)DBNull.Value)
                    };
                        var result = _repository.ExecuteSQL<TransferEntity>("TransferRecords", param).ToList();
                        // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                        if (result == null)
                        {
                            result = new List<TransferEntity>();
                        }

                        response.Success = true;
                        response.Data = result;
                }
                else // ((objTransferEntity.EnitityTypeId == 0 || string.IsNullOrEmpty(objTransferEntity.SourceEntityId) || string.IsNullOrEmpty(objTransferEntity.TargetEntityId)) )
                {
                    response.Success = false;
                    response.Data = new List<TransferEntity>();
                    response.Message.Add("Please select entity type, source & target entity.");
                }
                 
                
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }
 
        #endregion

       
    }

    #region Class Entity

    public class DropDownEntity
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
    }
    public class TransferEntity
    {
        public int EnitityTypeId { get; set; }
        public string SourceEntityId { get; set; }
        public string TargetEntityId { get; set; }
        public string UserId { get; set; }
        public int? AffectRecordCount { get; set; }
    }
    #endregion
}