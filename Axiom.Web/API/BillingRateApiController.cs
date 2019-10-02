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
    public class BillingRateApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CourtEntity> _repository = new GenericRepository<CourtEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetBillingRateList")]
        public ApiResponse<BillingRateEntity> GetBillingRateList(string MemberID)
        {
            var response = new ApiResponse<BillingRateEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("MemberID", (object)MemberID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<BillingRateEntity>("GetBillingRateList", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<BillingRateEntity>();
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
        [Route("GetBillingRateListByID")]
        public ApiResponse<BillingRateEntity> GetBillingRateListByID(string MemberID, int RecordType)
        {
            var response = new ApiResponse<BillingRateEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("MemberID", (object)MemberID ?? (object)DBNull.Value),
                                         new SqlParameter("RecordType", (object)RecordType ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<BillingRateEntity>("GetBillingRateListByID", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<BillingRateEntity>();
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
        [Route("UpdateBillingRate")]
        public ApiResponse<BillingRateDetailEntity> UpdateBillingRate(List<BillingRateDetailEntity> BillingRateList)
        {
            var response = new ApiResponse<BillingRateDetailEntity>();

            string xmlData = ConvertToXml<BillingRateDetailEntity>.GetXMLString(BillingRateList, "ID,RateMainID,RateName,RegRate,DcntRate");

            try
            {
                SqlParameter[] param = { new SqlParameter("xmlData", (object)xmlData ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<BillingRateDetailEntity>("UpdateBillingRate", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<BillingRateDetailEntity>();
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
        [Route("InsertBillingRate")]
        public ApiResponse<BillingRateDetailEntity> InsertBillingRate(List<BillingRateDetailEntity> BillingRateList)
        {
            var response = new ApiResponse<BillingRateDetailEntity>();

            string xmlData = ConvertToXml<BillingRateDetailEntity>.GetXMLString(BillingRateList, "MemberID,RecordTypeID,RateName,BillType,StartPage,EndPage,RegRate,DcntRate");

            try
            {
                SqlParameter[] param = { new SqlParameter("xmlData", (object)xmlData ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<BillingRateDetailEntity>("InsertBillingRate", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<BillingRateDetailEntity>();
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
        [Route("InsertRecordType")]
        public ApiResponse<RecordType> InsertRecordType(RecordType recordType)
        {
            var response = new ApiResponse<RecordType>();

            try
            {
                SqlParameter[] param = {
                    new SqlParameter("Descr", (object)recordType.Descr ?? (object)DBNull.Value),
                    new SqlParameter("isDisable", (object)!recordType.isDisable ?? (object)DBNull.Value),
                    new SqlParameter("isOnRatepage", (object)recordType.isOnRatePage ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<RecordType>("InsertRecordType", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<RecordType>();
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
        [Route("GetRecordTypeList")]
        public ApiResponse<RecordType> GetRecordTypeList()
        {
            var response = new ApiResponse<RecordType>();

            try
            {
                var result = _repository.ExecuteSQL<RecordType>("GetRecordTypeList").ToList();
                if (result == null)
                {
                    result = new List<RecordType>();
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