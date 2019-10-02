using Axiom.Common;
using Axiom.Data.Repository;
using Axiom.Entity;
using AXIOM.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class LocationApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<LocationEntity> _repository = new GenericRepository<LocationEntity>();
        #endregion

        [HttpPost]
        [Route("GetLocationList")]
        public TableParameter<LocationList> GetLocationList(TableParameter<LocationEntity> tableParameter, int PageIndex, string SearchValue = "", string LocID = "",
                string Name = "",
                string PhoneNo = "",
                string Department = "",
                string Address = "",
                string City = "",
                string State = "", string ReplacedBy = "")
        {

            tableParameter.PageIndex = PageIndex;
            string sortColumn = tableParameter.SortColumn.Desc ? tableParameter.SortColumn.Column + " desc" : tableParameter.SortColumn.Column + " asc";
            sortColumn = sortColumn.Replace("Name","Name1");
            string searchValue = SearchValue == null ? string.Empty : SearchValue.Trim();
            var response = new ApiResponse<LocationList>();
            try
            {
                SqlParameter[] param =
                    {

                  new SqlParameter  {
                     ParameterName = "Keyword",
                     DbType = DbType.String,
                     Value = searchValue
                 },new SqlParameter
                 {
                     ParameterName = "PageIndex",
                     DbType = DbType.Int32,
                     Value = tableParameter.PageIndex
                 }, new SqlParameter
                 {
                     ParameterName = "PageSize",
                     DbType = DbType.Int32,
                     Value = (object)tableParameter != null ? tableParameter.iDisplayLength : 10
                 },
                new SqlParameter
                {
                    ParameterName = "SortBy",
                    DbType = DbType.String,
                    Value =sortColumn
                },
                new SqlParameter{ParameterName = "LocID",DbType = DbType.String,Value = (object)LocID ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "Name",DbType = DbType.String,Value = (object)Name ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "PhoneNo",DbType = DbType.String,Value =  (object)PhoneNo ?? (object)DBNull.Value },
                new SqlParameter{ParameterName = "Department",DbType = DbType.String,Value = (object)Department ?? (object)DBNull.Value },
                new SqlParameter{ParameterName = "Address",DbType = DbType.String,Value = (object)Address ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "City",DbType = DbType.String,Value =  (object)City ?? (object)DBNull.Value },
                new SqlParameter{ParameterName = "State",DbType = DbType.String,Value = (object)State ?? (object)DBNull.Value  },
                new SqlParameter{ ParameterName = "ReplacedBy",DbType = DbType.String,Value = (object)ReplacedBy ?? (object)DBNull.Value }
                };



                var result = _repository.ExecuteSQL<LocationList>("GetLocationList", param).ToList();
                response.Success = true;
                response.Data = result;

                int totalRecords = 0;
                if (response != null && response.Data.Count > 0)
                {
                    totalRecords = response.Data[0].TotalRecords;
                }

                return new TableParameter<LocationList>
                {
                    aaData = (List<LocationList>)response.Data,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

            }
            catch (Exception ex)
            {

                response.Message.Add(ex.Message);
            }

            return new TableParameter<LocationList>();



            //var response = new ApiResponse<LocationEntity>();

            //try
            //{
            //    SqlParameter[] param = { new SqlParameter("LocationId", (object)LocationId ?? (object)DBNull.Value) };
            //    var result = _repository.ExecuteSQL<LocationEntity>("GetLocationList", param).ToList();
            //    if (result == null)
            //    {
            //        result = new List<LocationEntity>();
            //    }

            //    response.Success = true;
            //    response.Data = result;
            //}
            //catch (Exception ex)
            //{
            //    response.Message.Add(ex.Message);
            //}

            //return response;

        }


        [HttpGet]
        [Route("GetLocationListWithSearchFromLocation")]
        public ApiResponse<OrderSearchClientSide> GetLocationListWithSearchFromLocation(string SearchCriteria, int SearchCondition = 1, string SearchText = "")
        {
            var response = new ApiResponse<OrderSearchClientSide>();

            try
            {
                SqlParameter[] param = { new SqlParameter("SearchCriteria", (object)SearchCriteria ?? (object)DBNull.Value),
                                         new SqlParameter("SearchCondition", (object)SearchCondition ?? (object)DBNull.Value),
                                         new SqlParameter("SearchText", (object)SearchText ?? (object)DBNull.Value),
                };
                var result = _repository.ExecuteSQL<OrderSearchClientSide>("GetLocationListWithSearchFromLocation", param).ToList();

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
        [Route("GetLocationById")]
        public ApiResponse<LocationEntity> GetLocationById(string LocationId)
        {
            var response = new ApiResponse<LocationEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("LocationId", (object)LocationId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<LocationEntity>("GetLocationById", param).ToList();
                if (result == null)
                {
                    result = new List<LocationEntity>();
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
        [Route("InsertLocation")]
        public BaseApiResponse InsertLocation(string LoginEmpId, LocationEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                string xmlData = ConvertToXml<LocationEntity>.GetXMLString(new List<LocationEntity>() { model });
                SqlParameter[] param = {
                                         new SqlParameter("UserID", (object)LoginEmpId ?? (object)DBNull.Value),
                                         new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertLocation", param).FirstOrDefault();
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
        [Route("UpdateLocation")]
        public ApiResponse<LocationEntity> UpdateLocation(LocationEntity model, string LoginEmpId)
        {
            var response = new ApiResponse<LocationEntity>();
            try
            {
                model.strChngDate = Common.CommonHelper.GetCustomDateStringForXML(model.ChngDate);
                model.strEntDate = Common.CommonHelper.GetCustomDateStringForXML(model.EntDate);
                model.strLastUsedDate = Common.CommonHelper.GetCustomDateStringForXML(model.LastUsedDate);

                string xmlData = ConvertToXml<LocationEntity>.GetXMLString(new List<LocationEntity>() { model });

                SqlParameter[] param = {  new SqlParameter("LoginEmpId", (object)LoginEmpId ?? (object)DBNull.Value)
                                         ,new SqlParameter("LocID", (object)model.LocID ?? (object)DBNull.Value)
                                         ,new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value)
                                        };

                var result = _repository.ExecuteSQL<LocationEntity>("UpdateLocation", param).ToList();
                if (result == null)
                {
                    result = new List<LocationEntity>();
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
        [Route("DeleteLocation")]
        public BaseApiResponse DeleteLocation(string LocationId)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("LocationID", (object)LocationId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteLocation", param).FirstOrDefault();
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
        [Route("UpdateMergeLocation")]
        public BaseApiResponse UpdateMergeLocation(string locID, string parentLocationId, bool needtoMerge)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("locID", (object)locID ?? (object)DBNull.Value)
                ,new SqlParameter("parentLocationId", (object)parentLocationId ?? (object)DBNull.Value)
                ,new SqlParameter("needtoMerge", (object)needtoMerge ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("UpdateMergeLocation", param).FirstOrDefault();
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

        [HttpGet]
        [Route("GetLocationFormsList")]
        public ApiResponse<LocationForm> GetLocationFormsList(string LocationId, bool IsRequestForm = false)
        {  
            var response = new ApiResponse<LocationForm>();

            try
            {
                SqlParameter[] param = { new SqlParameter("LocID", (object)LocationId ?? (object)DBNull.Value), new SqlParameter("IsRequestForm", (object)IsRequestForm ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<LocationForm>("GetLocationFormsList", param).ToList();
                if (result == null)
                {
                    result = new List<LocationForm>();
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
        [Route("GetFormsAndDirectoryList")]
        public ApiResponse<FileDirectoryInfo> GetFormsAndDirectoryList()
        { 
            var response = new ApiResponse<FileDirectoryInfo>();

            try
            {
                 var fileDirectoryInfoList = DirSearch(ConfigurationManager.AppSettings["DocumentRoot"], null, null);

                response.Success = true;
                response.Data = fileDirectoryInfoList;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }

        [HttpGet]
        [Route("GetDocumentRootPath")]
        public string GetDocumentRootPath()
        {
            return ConfigurationManager.AppSettings["DocumentRoot"];
        }

        [HttpPost]
        [Route("InsertLocationForm")]
        public BaseApiResponse InsertLocationForm(LocationForm model)
        {
            var response = new BaseApiResponse();
            try
            {
                
                SqlParameter[] param = {
                                         new SqlParameter("LocID", (object)model.LocID ?? (object)DBNull.Value),
                                         new SqlParameter("IsRequestForm", (object)model.IsRequestForm ?? (object)DBNull.Value),
                                         new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value),
                                         new SqlParameter("FolderPath", (object)model.FolderPath ?? (object)DBNull.Value),
                                         new SqlParameter("FolderName", (object)model.FolderName ?? (object)DBNull.Value),
                                         new SqlParameter("DocFileName", (object)model.DocFileName ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertLocationForm", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                }
                else if(result == -1)
                {
                    response.Success = false;
                    response.Message.Add("The document already exists for this location.");
                }
                else if(result == -2)
                {
                    response.Success = false;
                    response.Message.Add("The document does not exist in the DB(table: documents).");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;

        }


        [HttpPost]
        [Route("DeleteLocationForm")]
        public BaseApiResponse DeleteLocationForm(int LocformID)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {
                                         new SqlParameter("LocformID", (object)LocformID ?? (object)DBNull.Value) 
                                         
                                       };
                var result = _repository.ExecuteSQL<int>("DeleteLocationForm", param).FirstOrDefault();
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

        private List<FileDirectoryInfo> DirSearch(string sDir, List<FileDirectoryInfo> tree , FileDirectoryInfo parentDirectory)
        {
            //List<clsDirectory> files = new List<clsDirectory>();
            if (parentDirectory == null)
            {
                parentDirectory = new FileDirectoryInfo() { fullpath = sDir, isfolder = true , isExpanded = true};
                //parentDirectory.breadcrumbs = parentDirectory.title;
            }
            if (tree==null || tree.Count() == 0)
            {
                tree = new List<FileDirectoryInfo>();
                tree.Add(parentDirectory);
            } 
            
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    parentDirectory.children.Add(new FileDirectoryInfo() { fullpath= f, isfolder = false, breadcrumbs= parentDirectory.breadcrumbs});
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    var childDirectory = new FileDirectoryInfo() { fullpath = d, isfolder = true };
                    childDirectory.breadcrumbs = string.Format("{0}{1}{2}" ,parentDirectory.breadcrumbs, string.IsNullOrEmpty(parentDirectory.breadcrumbs)? "":">",  childDirectory.title);
                    DirSearch(d, tree, childDirectory);
                    parentDirectory.children.Add(childDirectory);
                    //files.AddRange(DirSearch(d));
                }
            }
            catch (Exception excpt)
            {
                //MessageBox.Show(excpt.Message);
            }

            return tree;
        }


    }

    
}