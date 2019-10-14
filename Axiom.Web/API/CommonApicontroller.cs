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
    public class CommonApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<StateEntity> _repository = new GenericRepository<StateEntity>();

        //temp        
        #endregion

        [HttpGet]
        [Route("StateDropdown")]
        public ApiResponse<StateEntity> StateDropdown()
        {
            var response = new ApiResponse<StateEntity>();
            try
            {
                var result = _repository.ExecuteSQL<StateEntity>("StateDropdown").ToList();
                if (result == null)
                {
                    result = new List<StateEntity>();
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
        [Route("DistrictDropdown")]
        public ApiResponse<DistrictEntity> DistrictDropdown(string StateID)
        {
            var response = new ApiResponse<DistrictEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("StateID", (object)StateID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<DistrictEntity>("DistrictDropdown", param).ToList();
                if (result == null)
                {
                    result = new List<DistrictEntity>();
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
        [Route("CountyDropdown")]
        public ApiResponse<CountyEntity> CountyDropdown(string StateID)
        {
            var response = new ApiResponse<CountyEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("StateID", (object)StateID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<CountyEntity>("CountyDropdown", param).ToList();
                if (result == null)
                {
                    result = new List<CountyEntity>();
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
        [Route("CourtDropdown")]
        public ApiResponse<CourtEntity> CourtDropdown(string StateID)
        {
            var response = new ApiResponse<CourtEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("StateID", (object)StateID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<CourtEntity>("CourtDropdown", param).ToList();
                if (result == null)
                {
                    result = new List<CourtEntity>();
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
        [Route("DivisionDropdown")]
        public ApiResponse<CourtEntity> DivisionDropdown(string DistirctId, string StateId)
        {
            var response = new ApiResponse<CourtEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("DistirctName", (object)DistirctId ?? (object)DBNull.Value)
                                        ,new SqlParameter("StateId", (object)StateId ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<CourtEntity>("DivisionDropdown", param).ToList();
                if (result == null)
                {
                    result = new List<CourtEntity>();
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
        [Route("DepartmentDropdown")]
        public ApiResponse<DepartmentEntity> DepartmentDropdown()
        {
            var response = new ApiResponse<DepartmentEntity>();
            try
            {
                var result = _repository.ExecuteSQL<DepartmentEntity>("DepartmentDropdown").ToList();
                if (result == null)
                {
                    result = new List<DepartmentEntity>();
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
        [Route("MemberDropdown")]
        public ApiResponse<MemberEntity> MemberDropdown()
        {
            var response = new ApiResponse<MemberEntity>();
            try
            {
                var result = _repository.ExecuteSQL<MemberEntity>("MemberDropdown").ToList();
                if (result == null)
                {
                    result = new List<MemberEntity>();
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
        [Route("GetAttorneyByFirmID")]
        public ApiResponse<AttorneyEntity> GetAttorneyByFirmID(string FirmId)
        {
            var response = new ApiResponse<AttorneyEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AttorneyEntity>("GetAttorneyByFirmID", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyEntity>();
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
        [Route("GetAttorneyFormUserId")]
        public ApiResponse<AttorneyEntity> GetAttorneyFormUserId(string UserId)
        {
            var response = new ApiResponse<AttorneyEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AttorneyEntity>("GetAttorneyFormUserId", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyEntity>();
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
        [Route("GetAttorneyByFirmIDForclient")]
        public ApiResponse<AttorneyEntity> GetAttorneyByFirmIDForclient(string FirmId, string UserId, int CompNo, bool isShowMore = false)
        {
            var response = new ApiResponse<AttorneyEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value)
                                        ,new SqlParameter("isShowMore", (object)isShowMore ?? (object)DBNull.Value)
                                        ,new SqlParameter("CompanyNo", (object)CompNo ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<AttorneyEntity>("GetAttorneyByFirmIDForclient", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyEntity>();
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
        [Route("GetAttorneyByFirmIDForclientAndAdmin")]
        public ApiResponse<AttorneyEntity> GetAttorneyByFirmIDForclientAndAdmin(string FirmId, string UserId, string CompNo, bool isShowMore = false)
        {
            var response = new ApiResponse<AttorneyEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value)
                                        ,new SqlParameter("isShowMore", (object)isShowMore ?? (object)DBNull.Value)
                                        ,new SqlParameter("CompanyNo", (object)CompNo ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<AttorneyEntity>("GetAttorneyByFirmIDForclientAndAdmin", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyEntity>();
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


        #region PartStatusGroupsDropdown
        [HttpGet]
        [Route("PartStatusGroupsDropdown")]
        public ApiResponse<PartStatusGroups> PartStatusGroupsDropdown()
        {
            var response = new ApiResponse<PartStatusGroups>();
            try
            {
                var result = _repository.ExecuteSQL<PartStatusGroups>("PartStatusGroupsDropdown").ToList();
                if (result == null)
                {
                    result = new List<PartStatusGroups>();
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

        #region RateRecordTypeDropdown
        [HttpGet]
        [Route("RateRecordTypeDropdown")]
        public ApiResponse<RecordType> RateRecordTypeDropdown(string MemberID)
        {
            var response = new ApiResponse<RecordType>();
            try
            {
                SqlParameter[] param = { new SqlParameter("StateID", (object)MemberID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<RecordType>("RateRecordTypeDropdown", param).ToList();
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

        #region Term list
        [HttpGet]
        [Route("GetTermDropDown")]
        public ApiResponse<TermEntity> GetTermDropDown()
        {
            var response = new ApiResponse<TermEntity>();
            try
            {
                var result = _repository.ExecuteSQL<TermEntity>("GetTermDropDown").ToList();
                if (result == null)
                {
                    result = new List<TermEntity>();
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

        #region Company 


        [HttpGet]
        [Route("GetCompanyDropDown")]
        public ApiResponse<CompanyEntity> GetCompanyDropDown()
        {
            var response = new ApiResponse<CompanyEntity>();
            try
            {
                var result = _repository.ExecuteSQL<CompanyEntity>("GetCompanyDropDown").ToList();
                if (result == null)
                {
                    result = new List<CompanyEntity>();
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

        #region Account
        [HttpGet]
        [Route("GetAccountDropDown")]
        public ApiResponse<AccountEntity> GetAccountDropDown()
        {
            var response = new ApiResponse<AccountEntity>();
            try
            {
                var result = _repository.ExecuteSQL<AccountEntity>("GetAccoutDropDown").ToList();
                if (result == null)
                {
                    result = new List<AccountEntity>();
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

        #region FirmDefaultScope 

        [HttpGet]
        [Route("RecordTypeDropDown")]
        public ApiResponse<RecordTypeEntity> RecordTypeDropDown()
        {
            var response = new ApiResponse<RecordTypeEntity>();
            try
            {
                var result = _repository.ExecuteSQL<RecordTypeEntity>("GetRecordTypeDropDown").ToList();
                if (result == null)
                {
                    result = new List<RecordTypeEntity>();
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
        [Route("FirmForDropdown")]
        public ApiResponse<FirmEntity> FirmForDropdown(string search, int CompanyNo)
        {
            var response = new ApiResponse<FirmEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("search", (object)search ?? (object)DBNull.Value),
                new SqlParameter("CompanyNo", (object)CompanyNo ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<FirmEntity>("FirmForDropdown", param).ToList();
                if (result == null)
                {
                    result = new List<FirmEntity>();
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
        [Route("AttorneyForDropdown")]
        public ApiResponse<AttorneyScopeEntity> AttorneyForDropdown(string search, int CompanyNo)
        {
            var response = new ApiResponse<AttorneyScopeEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("search", (object)search ?? (object)DBNull.Value),
                    new SqlParameter("CompanyNo", (object)CompanyNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<AttorneyScopeEntity>("AttorneyForDropdown", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyScopeEntity>();
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
        [Route("GetNewSequenceNumber")]
        public ApiResponse<string> GetNewSequenceNumber(string tableName, string firstField, string secondField = "")
        {
            var response = new ApiResponse<string>();
            try
            {
                SqlParameter[] param = { new SqlParameter("tableName", (object)tableName ?? (object)DBNull.Value)
                                        ,new SqlParameter("firstField", (object)firstField ?? (object)DBNull.Value)
                                        ,new SqlParameter("secondField", (object)secondField ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<string>("GetNewSequenceNumber", param).ToList();
                if (result == null)
                {
                    result = new List<string>();
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
        [Route("FirmTypeForDropdown")]
        public ApiResponse<CodeEntity> FirmTypeForDropdown()
        {
            var response = new ApiResponse<CodeEntity>();
            try
            {
                var result = _repository.ExecuteSQL<CodeEntity>("FirmTypeForDropdown").ToList();
                if (result == null)
                {
                    result = new List<CodeEntity>();
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



        [HttpGet]
        [Route("LocationDepartmentDropDown")]
        public ApiResponse<RecordTypeEntity> LocationDepartmentDropDown()
        {
            var response = new ApiResponse<RecordTypeEntity>();
            try
            {
                var result = _repository.ExecuteSQL<RecordTypeEntity>("GetLocationDepartmentDropDown").ToList();
                if (result == null)
                {
                    result = new List<RecordTypeEntity>();
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
        [Route("GetSalutationDropDown")]
        public ApiResponse<RecordTypeEntity> GetSalutationDropDown()
        {
            var response = new ApiResponse<RecordTypeEntity>();
            try
            {
                var result = _repository.ExecuteSQL<RecordTypeEntity>("GetSalutationDropDown").ToList();
                if (result == null)
                {
                    result = new List<RecordTypeEntity>();
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
        [Route("GetAttorneyDropDown")]
        public ApiResponse<RecordTypeEntity> GetAttorneyDropDown()
        {
            var response = new ApiResponse<RecordTypeEntity>();
            try
            {
                var result = _repository.ExecuteSQL<RecordTypeEntity>("GetAttorneyDropDown").ToList();
                if (result == null)
                {
                    result = new List<RecordTypeEntity>();
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
        [Route("GetFileTypeDropdown")]
        public ApiResponse<FileTypeEntity> GetFileTypeDropdown()
        {
            var response = new ApiResponse<FileTypeEntity>();
            try
            {
                var result = _repository.ExecuteSQL<FileTypeEntity>("GetFileTypeDropdown").ToList();
                if (result == null)
                {
                    result = new List<FileTypeEntity>();
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
        [Route("GetFirmByUserId")]
        public ApiResponse<FirmScopeEntity> GetFirmByUserId(string UserID, string CompNo)
        {
            var response = new ApiResponse<FirmScopeEntity>();
            try
            {
                Guid Gid = new Guid(UserID);
                SqlParameter[] param = { new SqlParameter("UserId", (object)Gid ?? (object)DBNull.Value)
                                        ,new SqlParameter("CompanyNo", (object)CompNo ?? (object)DBNull.Value)


                                        };
                var result = _repository.ExecuteSQL<FirmScopeEntity>("GetFirmByUserId", param).ToList();
                if (result == null)
                {
                    result = new List<FirmScopeEntity>();
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
        [Route("GetAssociatedFirm")]
        public ApiResponse<FirmScopeEntity> GetAssociatedFirm(string UserID, string OrderID)
        {
            var response = new ApiResponse<FirmScopeEntity>();
            try
            {
                Guid Gid = new Guid(UserID);
                SqlParameter[] param = { new SqlParameter("UserId", (object)Gid ?? (object)DBNull.Value)
                        ,new SqlParameter("OrderID", (object)OrderID ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<FirmScopeEntity>("GetAssociatedFirm", param).ToList();
                if (result == null)
                {
                    result = new List<FirmScopeEntity>();
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
        [Route("GetFirmDetailByFirmID")]
        public ApiResponse<FirmEntity> GetFirmDetailByFirmID(string FirmID)
        {
            var response = new ApiResponse<FirmEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<FirmEntity>("GetFirmDetailByFirmID", param).ToList();
                if (result == null)
                {
                    result = new List<FirmEntity>();
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
        [Route("GetAttorneyDetailByAttyID")]
        public ApiResponse<AttorneyEntity> GetAttorneyDetailByAttyID(string AttyID)
        {
            var response = new ApiResponse<AttorneyEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)AttyID ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<AttorneyEntity>("GetAttorneyDetailByAttyID", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyEntity>();
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
        [Route("GetQuickNotesDropDown")]
        public ApiResponse<QuickNotesEntity> GetQuickNotesDropDown()
        {
            var response = new ApiResponse<QuickNotesEntity>();
            try
            {
                var result = _repository.ExecuteSQL<QuickNotesEntity>("GetQuickNotesDropDown").ToList();
                if (result == null)
                {
                    result = new List<QuickNotesEntity>();
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
        [Route("GetInternalStatusDropDown")]
        public ApiResponse<InternalStatusesEntity> GetInternalStatusDropDown()
        {
            var response = new ApiResponse<InternalStatusesEntity>();
            try
            {
                var result = _repository.ExecuteSQL<InternalStatusesEntity>("GetInternalStatuses").ToList();
                if (result == null)
                {
                    result = new List<InternalStatusesEntity>();
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
        [Route("GetCanvasRequestMasterDropDown")]
        public ApiResponse<CanvasRequestMasterEntity> GetCanvasRequestMasterDropDown()
        {
            var response = new ApiResponse<CanvasRequestMasterEntity>();
            try
            {
                var result = _repository.ExecuteSQL<CanvasRequestMasterEntity>("GetCanvasRequestMasterDropDown").ToList();
                if (result == null)
                {
                    result = new List<CanvasRequestMasterEntity>();
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
        [Route("GetAccountRepDropdown")]
        public ApiResponse<EmployeeEntity> GetAccountRepDropdown()
        {
            var response = new ApiResponse<EmployeeEntity>();
            try
            {
                var result = _repository.ExecuteSQL<EmployeeEntity>("GetAccountRepDropdown").ToList();
                if (result == null)
                {
                    result = new List<EmployeeEntity>();
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
        [Route("GetInternalStatusesDropdown")]
        public ApiResponse<InternalStatusEntity> GetInternalStatusesDropdown()
        {
            var response = new ApiResponse<InternalStatusEntity>();
            try
            {
                var result = _repository.ExecuteSQL<InternalStatusEntity>("GetInternalStatusesDropdown").ToList();
                if (result == null)
                {
                    result = new List<InternalStatusEntity>();
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

        #region Class Entity

        public class InternalStatusEntity
        {
            public int InternalStatusId { get; set; }
            public string InternalStatus { get; set; }
        }
        public class NoteEntity
        {
            public string Notes { get; set; }
        }
        public class UserDetailEntity
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmpId { get; set; }
        }
        #endregion


        [HttpGet]
        [Route("GetLocationNotes")]
        public ApiResponse<NoteEntity> GetLocationNotes(string LocId)
        {
            var response = new ApiResponse<NoteEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("LocId", (object)LocId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<NoteEntity>("GetLocationNotes", param).ToList();
                if (result == null)
                {
                    result = new List<NoteEntity>();
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
        [Route("GetFirmNotes")]
        public ApiResponse<NoteEntity> GetFirmNotes(string FirmId)
        {
            var response = new ApiResponse<NoteEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<NoteEntity>("GetFirmNotes", param).ToList();
                if (result == null)
                {
                    result = new List<NoteEntity>();
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
        [Route("GetAttorneyNotes")]
        public ApiResponse<NoteEntity> GetAttorneyNotes(string AttyId)
        {
            var response = new ApiResponse<NoteEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("AttyId", (object)AttyId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<NoteEntity>("GetAttorneyNotes", param).ToList();
                if (result == null)
                {
                    result = new List<NoteEntity>();
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
        [Route("GetUserDetails")]
        public ApiResponse<UserDetailEntity> GetUserDetails()
        {
            var response = new ApiResponse<UserDetailEntity>();
            try
            {
                var result = _repository.ExecuteSQL<UserDetailEntity>("GetUserDetailsByEmpId").ToList();
                if (result == null)
                {
                    result = new List<UserDetailEntity>();
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
        [Route("GetScopeForLocation")]
        public ApiResponse<string> GetScopeForLocation(string OrderNo, string RecType)
        {
            var response = new ApiResponse<string>();
            try
            {


                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("RecType", (object)RecType ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<string>("GetScopeForLocation", param).ToList();

                if (result == null)
                {
                    result = new List<string>();
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



    }
}