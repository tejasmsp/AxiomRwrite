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
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Web;
using System.IO;
using System.Text;
using System.Data;
using Aspose.Words;
using Aspose.Words.Tables;
using System.Drawing;


namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class AccessReportApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<DepartmentEntity> _repository = new GenericRepository<DepartmentEntity>();
        #endregion

        public enum AccessReport
        {
            [Description("PartsByDate")]
            PartsByDate,

            [Description("InvoiceByDate")]
            InvoiceByDate,

            [Description("ChecksByDate")]
            ChecksByDate,

            [Description("ChecksByNumber")]
            ChecksByNumber,

            [Description("OrderBySSN")]
            OrderBySSN,

            [Description("NonInvoicedParts")]
            NonInvoicedParts,

            [Description("HanoverBilling")]
            HanoverBilling,

            [Description("HanoverBillingFees")]
            HanoverBillingFees,

            [Description("GrangeBilling")]
            GrangeBilling,

            [Description("GroverBilling")]
            GroverBilling
        }




        #region DatabaseOperations
        [HttpGet]
        [Route("DownloadAccessReport")]
        public HttpResponseMessage DownloadAccessReport(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";
            if (string.IsNullOrEmpty(StartDate))
                StartDate = "01/01/1900";

            if (string.IsNullOrEmpty(EndDate))
                EndDate = DateTime.Now.ToString("MM/dd/yyyy");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            StringBuilder sb = new StringBuilder();

            MemoryStream memoryStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memoryStream);
            DataTable dt = new DataTable();
            int i;




            switch (reportName)
            {
                case "PartsByDate":

                    SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value),
                                            new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value),
                                            new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value) };

                    var result = _repository.ExecuteSQL<PartsByDateEntity>("AccessGetPartByDate", param).ToList();

                    if (result == null)
                    {
                        result = new List<PartsByDateEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result);
                    break;

                case "InvoiceByDate":

                    SqlParameter[] param1 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result1 = _repository.ExecuteSQL<InvoiceByDateEntity>("AccessGetInvoiceByDate", param1).ToList();

                    if (result1 == null)
                    {
                        result1 = new List<InvoiceByDateEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result1);
                    break;

                case "ChecksByDate":

                    SqlParameter[] param2 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value),
                                            new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)};

                    var result2 = _repository.ExecuteSQL<ChecksByDateEntity>("AccessChecksByDate", param2).ToList();

                    if (result2 == null)
                    {
                        result2 = new List<ChecksByDateEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result2);
                    break;

                case "ChecksByNumber":

                    SqlParameter[] param3 = { new SqlParameter("CheckNumber", (object)CheckNumber ?? (object)DBNull.Value) };

                    var result3 = _repository.ExecuteSQL<ChecksByNumberEntity>("AccessChecksByNumber", param3).ToList();

                    if (result3 == null)
                    {
                        result3 = new List<ChecksByNumberEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result3);
                    break;

                case "OrderBySSN":

                    SqlParameter[] param4 = { new SqlParameter("SSNNumber", (object)ssnNumber ?? (object)DBNull.Value)
                                                ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)};

                    var result4 = _repository.ExecuteSQL<OrderBySSNEntity>("AccessOrderBySSN", param4).ToList();

                    if (result4 == null)
                    {
                        result4 = new List<OrderBySSNEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result4);
                    break;

                case "NonInvoicedParts":

                    SqlParameter[] param5 = { new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value) };
                    var result5 = _repository.ExecuteSQL<NonInvoicedPartsEntity>("AccessNonInvoicedParts", param5).ToList();



                    if (result5 == null)
                    {
                        result5 = new List<NonInvoicedPartsEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result5);
                    break;
                case "HanoverBilling":


                    SqlParameter[] param6 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result6 = _repository.ExecuteSQL<HanoverBillingEntity>("AccessHanoverBilling", param6).ToList();

                    if (result6 == null)
                    {
                        result6 = new List<HanoverBillingEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result6);
                    break;

                case "HanoverBillingFees":
                    SqlParameter[] param7 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value),
                                            new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result7 = _repository.ExecuteSQL<HanoverBillingFeesEntity>("AccessHanoverBillingFee", param7).ToList();

                    if (result7 == null)
                    {
                        result7 = new List<HanoverBillingFeesEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result7);
                    break;

                case "GrangeBilling":
                    // AccessGrangeBilling

                    SqlParameter[] param8 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result8 = _repository.ExecuteSQL<GrangeBillingEntity>("AccessGrangeBilling", param8).ToList();

                    if (result8 == null)
                    {
                        result8 = new List<GrangeBillingEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result8);
                    break;

                case "GroverBilling":

                    SqlParameter[] param9 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result9 = _repository.ExecuteSQL<GroverBillingEntity>("AccessHackneyGrover", param9).ToList();

                    if (result9 == null)
                    {
                        result9 = new List<GroverBillingEntity>();
                    }
                    dt = Common.CommonHelper.ToDataTable(result9);
                    break;

            }


            foreach (DataColumn dc in dt.Columns)
            {
                sb.Append(FormatCSV(dc.Caption) + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sw.WriteLine(sb.ToString());
            sb.Clear();

            foreach (DataRow dr in dt.Rows)
            {


                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.ToString().ToLower().Contains("date"))
                        sb.Append(FormatCSV(String.Format("{0:MM/dd/yyyy}", dr[dc.ColumnName].ToString())) + ",");
                    else
                        sb.Append(FormatCSV(dr[dc.ColumnName].ToString()) + ",");

                }
                sb.Remove(sb.Length - 1, 1);
                sw.WriteLine(sb.ToString());
                sb.Clear();
                // sb.AppendLine();
            }
            sw.Flush();



            string filename = string.Empty;
            filename = reportName;

            response.Content = new ByteArrayContent(memoryStream.ToArray());
            response.Content.Headers.Clear();
            response.Content.Headers.TryAddWithoutValidation("Content-Disposition", "attachment; filename=" + filename + ".csv");
            response.Content.Headers.Add("Content-Length", memoryStream.ToArray().Length.ToString());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(".csv"));

            return response;


        }

        #endregion


        [HttpGet]
        [Route("DisplayAccessReportPartsByDate")]
        public ApiResponse<PartsByDateEntity> DisplayAccessReportPartsByDate(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            if (string.IsNullOrEmpty(StartDate))
                StartDate = "01/01/1900";

            if (string.IsNullOrEmpty(EndDate))
                EndDate = DateTime.Now.ToString("MM/dd/yyyy");

            // HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            var response = new ApiResponse<PartsByDateEntity>();




            switch (reportName)
            {
                case "PartsByDate":

                    SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value),
                                            new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value),
                                            new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value) };

                    var result = _repository.ExecuteSQL<PartsByDateEntity>("AccessGetPartByDate", param).ToList();

                    if (result == null)
                    {
                        result = new List<PartsByDateEntity>();
                    }
                    response.Data = result;
                    break;

                case "InvoiceByDate":

                    SqlParameter[] param1 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value),
                                            new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value),
                                            new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value) };

                    var result1 = _repository.ExecuteSQL<InvoiceByDateEntity>("AccessGetInvoiceByDate", param1).ToList();

                    if (result1 == null)
                    {
                        result1 = new List<InvoiceByDateEntity>();
                    }
                    break;

                case "ChecksByDate":

                    SqlParameter[] param2 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value) };

                    var result2 = _repository.ExecuteSQL<ChecksByDateEntity>("AccessChecksByDate", param2).ToList();

                    if (result2 == null)
                    {
                        result2 = new List<ChecksByDateEntity>();
                    }
                    break;

                case "ChecksByNumber":

                    SqlParameter[] param3 = { new SqlParameter("CheckNumber", (object)CheckNumber ?? (object)DBNull.Value) };

                    var result3 = _repository.ExecuteSQL<ChecksByNumberEntity>("AccessChecksByNumber", param3).ToList();

                    if (result3 == null)
                    {
                        result3 = new List<ChecksByNumberEntity>();
                    }
                    break;

                case "OrderBySSN":

                    SqlParameter[] param4 = { new SqlParameter("SSNNumber", (object)ssnNumber ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value) };


                    var result4 = _repository.ExecuteSQL<OrderBySSNEntity>("AccessOrderBySSN", param4).ToList();

                    if (result4 == null)
                    {
                        result4 = new List<OrderBySSNEntity>();
                    }
                    break;

                case "NonInvoicedParts":

                    SqlParameter[] param5 = { new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value) };

                    var result5 = _repository.ExecuteSQL<NonInvoicedPartsEntity>("AccessNonInvoicedParts", param5).ToList();

                    if (result5 == null)
                    {
                        result5 = new List<NonInvoicedPartsEntity>();
                    }
                    break;
                case "HanoverBilling":


                    SqlParameter[] param6 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result6 = _repository.ExecuteSQL<HanoverBillingEntity>("AccessHanoverBilling", param6).ToList();

                    if (result6 == null)
                    {
                        result6 = new List<HanoverBillingEntity>();
                    }
                    break;

                case "HanoverBillingFees":
                    SqlParameter[] param7 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result7 = _repository.ExecuteSQL<HanoverBillingFeesEntity>("AccessHanoverBillingFee", param7).ToList();

                    if (result7 == null)
                    {
                        result7 = new List<HanoverBillingFeesEntity>();
                    }
                    break;

                case "GrangeBilling":
                    // AccessGrangeBilling

                    SqlParameter[] param8 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result8 = _repository.ExecuteSQL<GrangeBillingEntity>("AccessGrangeBilling", param8).ToList();

                    if (result8 == null)
                    {
                        result8 = new List<GrangeBillingEntity>();
                    }
                    break;

                case "GroverBilling":
                    // GroverBillingEntity

                    SqlParameter[] param9 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                    var result9 = _repository.ExecuteSQL<GroverBillingEntity>("AccessHackneyGrover", param9).ToList();

                    if (result9 == null)
                    {
                        result9 = new List<GroverBillingEntity>();
                    }
                    break;
            }

            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportInvoiceByDate")]
        public ApiResponse<InvoiceByDateEntity> DisplayAccessReportInvoiceByDate(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            if (string.IsNullOrEmpty(StartDate))
                StartDate = "01/01/1900";

            if (string.IsNullOrEmpty(EndDate))
                EndDate = DateTime.Now.ToString("MM/dd/yyyy");

            var response = new ApiResponse<InvoiceByDateEntity>();

            SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

            var result = _repository.ExecuteSQL<InvoiceByDateEntity>("AccessGetInvoiceByDate", param).ToList();

            if (result == null)
                result = new List<InvoiceByDateEntity>();

            response.Data = result;
            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportChecksByDate")]
        public ApiResponse<ChecksByDateEntity> DisplayAccessReportChecksByDate(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            if (string.IsNullOrEmpty(StartDate))
                StartDate = "01/01/1900";

            if (string.IsNullOrEmpty(EndDate))
                EndDate = DateTime.Now.ToString("MM/dd/yyyy");

            var response = new ApiResponse<ChecksByDateEntity>();

            SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)};

            var result = _repository.ExecuteSQL<ChecksByDateEntity>("AccessChecksByDate", param).ToList();

            if (result == null)
                result = new List<ChecksByDateEntity>();

            response.Data = result;
            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportChecksByNumber")]
        public ApiResponse<ChecksByNumberEntity> DisplayAccessReportChecksByNumber(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            if (string.IsNullOrEmpty(StartDate))
                StartDate = "01/01/1900";

            if (string.IsNullOrEmpty(EndDate))
                EndDate = DateTime.Now.ToString("MM/dd/yyyy");

            var response = new ApiResponse<ChecksByNumberEntity>();

            SqlParameter[] param = { new SqlParameter("CheckNumber", (object)CheckNumber ?? (object)DBNull.Value) };

            var result = _repository.ExecuteSQL<ChecksByNumberEntity>("AccessChecksByNumber", param).ToList();

            if (result == null)
                result = new List<ChecksByNumberEntity>();

            response.Data = result;
            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportOrderBySSN")]
        public ApiResponse<OrderBySSNEntity> DisplayAccessReportOrderBySSN(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            var response = new ApiResponse<OrderBySSNEntity>();

            SqlParameter[] param = { new SqlParameter("SSNNumber", (object)ssnNumber ?? (object)DBNull.Value)
                    ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
            };

            var result = _repository.ExecuteSQL<OrderBySSNEntity>("AccessOrderBySSN", param).ToList();

            if (result == null)
                result = new List<OrderBySSNEntity>();

            response.Data = result;
            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportNonInvoicedParts")]
        public ApiResponse<NonInvoicedPartsEntity> DisplayAccessReportNonInvoicedParts(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";
            var response = new ApiResponse<NonInvoicedPartsEntity>();
            try
            {

                SqlParameter[] param = { new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value) };

                var result = _repository.ExecuteSQL<NonInvoicedPartsEntity>("AccessNonInvoicedParts", param).ToList();

                if (result == null)
                    result = new List<NonInvoicedPartsEntity>();

                response.Data = result;
                return response;
            }
            catch (Exception ex)
            {
                return response;
            }


        }

        [HttpGet]
        [Route("DisplayAccessReportHanoverBilling")]
        public ApiResponse<HanoverBillingEntity> DisplayAccessReportHanoverBilling(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            var response = new ApiResponse<HanoverBillingEntity>();

            SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};


            var result = _repository.ExecuteSQL<HanoverBillingEntity>("AccessHanoverBilling", param).ToList();

            if (result == null)
                result = new List<HanoverBillingEntity>();


            response.Data = result;
            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportHanoverBillingFees")]
        public ApiResponse<HanoverBillingFeesEntity> DisplayAccessReportHanoverBillingFees(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            var response = new ApiResponse<HanoverBillingFeesEntity>();

            SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                    ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                    ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                    ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

            var result = _repository.ExecuteSQL<HanoverBillingFeesEntity>("AccessHanoverBillingFee", param).ToList();

            if (result == null || result.Count <= 0)
                result = new List<HanoverBillingFeesEntity>();


            response.Data = result;
            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportGrangeBilling")]
        public ApiResponse<GrangeBillingEntity> DisplayAccessReportGrangeBilling(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            var response = new ApiResponse<GrangeBillingEntity>();

            SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

            var result = _repository.ExecuteSQL<GrangeBillingEntity>("AccessGrangeBilling", param).ToList();

            if (result == null)
                result = new List<GrangeBillingEntity>();

            response.Data = result;
            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportGroverBilling")]
        public ApiResponse<GroverBillingEntity> DisplayAccessReportGroverBilling(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            var response = new ApiResponse<GroverBillingEntity>();


            SqlParameter[] param9 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

            var result = _repository.ExecuteSQL<GroverBillingEntity>("AccessHackneyGrover", param9).ToList();

            if (result == null)
                result = new List<GroverBillingEntity>();

            response.Data = result;
            return response;
        }

        [HttpGet]
        [Route("DisplayAccessReportAgedAR")]
        public ApiResponse<AgedAR> DisplayAccessReportAgedAR(string reportName, string StartDate, string EndDate, string CompanyID, string CheckNumber, string ssnNumber, string FirmID)
        {
            if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                FirmID = string.Empty;

            if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                CompanyID = "0";

            if (string.IsNullOrEmpty(StartDate))
                StartDate = "01/01/1900";

            if (string.IsNullOrEmpty(EndDate))
                EndDate = DateTime.Now.ToString("MM/dd/yyyy");

            var response = new ApiResponse<AgedAR>();


            SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

            var result = _repository.ExecuteSQL<AgedAR>("AccessAgedAR", param).ToList();

            if (result == null)
                result = new List<AgedAR>();

            response.Data = result;
            return response;
        }

        public static string FormatCSV(string input)
        {
            try
            {
                if (input == null)
                    return string.Empty;

                bool containsQuote = false;
                bool containsComma = false;
                int len = input.Length;
                for (int i = 0; i < len && (containsComma == false || containsQuote == false); i++)
                {
                    char ch = input[i];
                    if (ch == '"')
                        containsQuote = true;
                    else if (ch == ',')
                        containsComma = true;
                }

                if (containsQuote && containsComma)
                    input = input.Replace("\"", "\"\"");

                if (containsComma)
                    return "\"" + input + "\"";
                else
                    return input;
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadAgedAR")]
        public HttpResponseMessage DownloadAgedAR(string StartDate, string EndDate, string CompanyID, string FirmID)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            string fileNames = "ClientRecords";
            string strLiveURL = System.Configuration.ConfigurationManager.AppSettings["LiveSiteURL"].ToString();
            string DocumentPath = System.Configuration.ConfigurationManager.AppSettings["AgedARReportDocumentPath"].ToString();

            if (string.IsNullOrEmpty(StartDate))
                StartDate = "01/01/1900";

            if (string.IsNullOrEmpty(EndDate))
                EndDate = DateTime.Now.ToString("MM/dd/yyyy");

            try
            {
                if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                    FirmID = string.Empty;

                if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                    CompanyID = "0";

                SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<AgedAR>("AccessAgedAR", param).ToList();

                // dt = Common.CommonHelper.ToDataTable(result10);

                fileNames = "AgedAR-Report_" + DateTime.Now.ToString("MMddyyyyhhmm");
                if (!string.IsNullOrEmpty(FirmID))
                {
                    fileNames += "_" + FirmID;
                }


                List<AgedAR> resultThirty;
                List<AgedAR> resultSixty;
                List<AgedAR> resultNinty;
                List<AgedAR> resultNintyPlus;

                decimal thirtyTotalInv = 0;
                decimal thirtyTotalBalance = 0;

                decimal sixtyTotalInv = 0;
                decimal sixtyTotalBalance = 0;

                decimal nintyTotalInv = 0;
                decimal nintyTotalBalance = 0;

                decimal nintyPlusTotalInv = 0;
                decimal nintyPlusTotalBalance = 0;



                resultThirty = result.Where(x => x.Days <= 30).OrderByDescending(x => x.Days).ToList<AgedAR>();
                resultSixty = result.Where(x => x.Days > 30 && x.Days <= 60).OrderByDescending(x => x.Days).ToList<AgedAR>();
                resultNinty = result.Where(x => x.Days > 60 && x.Days <= 90).OrderByDescending(x => x.Days).ToList<AgedAR>();
                resultNintyPlus = result.Where(x => x.Days > 90).OrderBy(x => x.OrderNo).OrderByDescending(x => x.Days).ToList<AgedAR>();

                thirtyTotalInv = resultThirty.Where(x => x.Days <= 30).Sum(x => x.InvoiceAmount);
                thirtyTotalBalance = resultThirty.Where(x => x.Days <= 30).Sum(x => x.Balance);

                sixtyTotalInv = resultSixty.Where(x => x.Days > 30 && x.Days <= 60).Sum(x => x.InvoiceAmount);
                sixtyTotalBalance = resultSixty.Where(x => x.Days > 30 && x.Days <= 60).Sum(x => x.Balance);

                nintyTotalInv = resultNinty.Where(x => x.Days > 60 && x.Days <= 90).Sum(x => x.InvoiceAmount);
                nintyTotalBalance = resultNinty.Where(x => x.Days > 60 && x.Days <= 90).Sum(x => x.Balance);

                nintyPlusTotalInv = resultNintyPlus.Where(x => x.Days > 90).Sum(x => x.InvoiceAmount);
                nintyPlusTotalBalance = resultNintyPlus.Where(x => x.Days > 90).Sum(x => x.Balance);

                string[] ColName = { "Invoice No", "OrderNo", "Invoice Amount", "Paid Amount", "Balance", "Invoice Date", "Firm", "Location", "Days" };
                //Get dummy datasource (data table with random number of rows)

                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense("Aspose.Words.lic");
                //Open or create document and create DocumentBuilder
                Aspose.Words.Document doc = new Aspose.Words.Document(DocumentPath);

                //Document builder will be needed to build table in the document
                Aspose.Words.DocumentBuilder builder = new DocumentBuilder(doc);


                //move documentBuilder cursor to the bookmark inside table

                if (resultThirty.Count > 0)
                {
                    builder.MoveToBookmark("Count30");
                    builder.Writeln("(" + resultThirty.Count.ToString() + ")");
                    builder.Writeln("Invoice Amount :" + thirtyTotalInv.ToString("C"));
                    builder.Write("Balance Amount :" + thirtyTotalBalance.ToString("C"));
                }

                builder.MoveToBookmark("TableThirty");
                Aspose.Words.Tables.Table myTable = (Aspose.Words.Tables.Table)builder.CurrentNode.GetAncestor(NodeType.Table);
                BuildHeaderRow(ColName, myTable, builder);
                InsertData(resultThirty, myTable, builder);

                if (resultSixty.Count > 0)
                {
                    builder.MoveToBookmark("Count60");
                    builder.Writeln("(" + resultSixty.Count.ToString() + ")");
                    builder.Writeln("Invoice Amount :" + sixtyTotalInv.ToString("C"));
                    builder.Write("Balance Amount :" + sixtyTotalBalance.ToString("C"));
                }
                // builder.InsertBreak(BreakType.SectionBreakNewPage);
                builder.MoveToBookmark("TableSixty");
                myTable = (Aspose.Words.Tables.Table)builder.CurrentNode.GetAncestor(NodeType.Table);
                BuildHeaderRow(ColName, myTable, builder);
                InsertData(resultSixty, myTable, builder);

                if (resultNinty.Count > 0)
                {
                    builder.MoveToBookmark("Count90");
                    builder.Writeln("(" + resultNinty.Count.ToString() + ")");
                    builder.Writeln("Invoice Amount :" + nintyTotalInv.ToString("C"));
                    builder.Write("Balance Amount :" + nintyTotalBalance.ToString("C"));
                }
                // builder.InsertBreak(BreakType.SectionBreakNewPage);
                builder.MoveToBookmark("TableNinty");
                myTable = (Aspose.Words.Tables.Table)builder.CurrentNode.GetAncestor(NodeType.Table);
                BuildHeaderRow(ColName, myTable, builder);
                InsertData(resultNinty, myTable, builder);

                if (resultNintyPlus.Count > 0)
                {
                    builder.MoveToBookmark("Count90Plus");
                    builder.Writeln("(" + resultNintyPlus.Count.ToString() + ")");
                    builder.Writeln("Invoice Amount :" + nintyPlusTotalInv.ToString("C"));
                    builder.Write("Balance Amount :" + nintyPlusTotalBalance.ToString("C"));
                }
                builder.MoveToBookmark("TableNintyPlus");
                myTable = (Aspose.Words.Tables.Table)builder.CurrentNode.GetAncestor(NodeType.Table);
                BuildHeaderRow(ColName, myTable, builder);
                InsertData(resultNintyPlus, myTable, builder);


                MemoryStream ms = new MemoryStream();
                ms.Flush();
                doc.Save(ms, Aspose.Words.SaveFormat.Pdf);


                response.Content = new ByteArrayContent(ms.ToArray());
                response.Content.Headers.Clear();
                response.Content.Headers.TryAddWithoutValidation("Content-Disposition", "attachment; filename=" + fileNames + ".pdf");
                response.Content.Headers.Add("Content-Length", ms.ToArray().Length.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));



                return response;
            }
            catch (Exception ex)
            {
                // response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("DownloadAgedARSummary")]
        public HttpResponseMessage DownloadAgedARSummary(string StartDate, string EndDate, string CompanyID, string FirmID)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            string fileNames = "ClientRecords";
            string strLiveURL = System.Configuration.ConfigurationManager.AppSettings["LiveSiteURL"].ToString();
            string DocumentPath = System.Configuration.ConfigurationManager.AppSettings["AgedARSummaryReportDocumentPath"].ToString();

            if (string.IsNullOrEmpty(StartDate))
                StartDate = "01/01/1900";

            if (string.IsNullOrEmpty(EndDate))
                EndDate = DateTime.Now.ToString("MM/dd/yyyy");

            try
            {
                if (FirmID.ToLower() == "undefined" || FirmID.ToLower() == "null")
                    FirmID = string.Empty;

                if (CompanyID.ToLower() == "undefined" || CompanyID.ToLower() == "null")
                    CompanyID = "0";

                SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value)
                                            ,new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value)
                                            ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)};

                List<AgedARSummary> result = _repository.ExecuteSQL<AgedARSummary>("AccessAgedARSummary", param).ToList();

                // dt = Common.CommonHelper.ToDataTable(result10);

                fileNames = "AgedARSummary-Report_" + DateTime.Now.ToString("MMddyyyyhhmm");
                if (!string.IsNullOrEmpty(FirmID))
                {
                    fileNames += "_" + FirmID;
                }


                decimal totalInvoiceAmount = 0;
                decimal totalPaidAmount = 0;
                decimal totalThirtyAmount = 0;
                decimal totalSixtyAmount = 0;
                decimal totalNintyAmount = 0;
                decimal totalNintyPlusAmount = 0;
                decimal totalPendingAmount = 0;



                string[] ColName = { "Firm Name", "FirmID", "Invoice Amount", "Paid Amount", "Thirty", "Sixty", "Ninty", "Ninty Plus", "Total Pending" };
                //Get dummy datasource (data table with random number of rows)

                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense("Aspose.Words.lic");
                //Open or create document and create DocumentBuilder
                Aspose.Words.Document doc = new Aspose.Words.Document(DocumentPath);

                //Document builder will be needed to build table in the document
                Aspose.Words.DocumentBuilder builder = new DocumentBuilder(doc);

                totalInvoiceAmount = result.Sum(x => x.InvoiceAmount);
                totalPaidAmount = result.Sum(x => x.PaidAmount);
                totalThirtyAmount = result.Sum(x => x.ThirtyDaysAmount);
                totalSixtyAmount = result.Sum(x => x.SixtyDaysAmount);
                totalNintyAmount = result.Sum(x => x.NintyDaysAmount);
                totalNintyPlusAmount = result.Sum(x => x.NintyPlysDaysAmount);
                totalPendingAmount = result.Sum(x => x.TotalPending);


                //move documentBuilder cursor to the bookmark inside table

                if (result.Count > 0)
                {
                    builder.MoveToBookmark("Count30");
                    builder.Writeln(result.Count.ToString());
                    builder.Writeln("Invoice Amount :" + totalInvoiceAmount.ToString("C"));
                    builder.Writeln("Paid Amount :" + totalPaidAmount.ToString("C"));

                    builder.Writeln("Thirty Days Amount :" + totalThirtyAmount.ToString("C"));
                    builder.Writeln("Sixty Days Amount :" + totalSixtyAmount.ToString("C"));
                    builder.Writeln("Ninty Days Amount :" + totalNintyAmount.ToString("C"));
                    builder.Writeln("Ninty Plus Days Amount :" + totalNintyPlusAmount.ToString("C"));
                    builder.Writeln("Total Pending Amount :" + totalPendingAmount.ToString("C"));
                }
                builder.MoveToBookmark("Date");
                builder.Write(StartDate + " to " + EndDate);

                builder.MoveToBookmark("TableThirty");
                Aspose.Words.Tables.Table myTable = (Aspose.Words.Tables.Table)builder.CurrentNode.GetAncestor(NodeType.Table);

                foreach (string col in ColName)
                {
                    //Clone first cell of first row to build header of the table
                    Cell hCell = (Cell)myTable.FirstRow.FirstCell.Clone(true);

                    //Insert cell into the first row
                    myTable.FirstRow.AppendChild(hCell);

                    // myTable.LastRow.AppendChild(hCell);

                    //Move document builder cursor to the cell
                    builder.MoveTo(hCell.FirstParagraph);

                    //Insert text
                    builder.Write(col);
                }

                myTable.Rows[0].Cells[0].Remove();
                myTable.Rows[1].Cells[0].Remove();

                myTable.Rows[0].Cells[0].CellFormat.Width = 180; //Firm Name
                myTable.Rows[0].Cells[1].CellFormat.Width = 70; // FirmID
                myTable.Rows[0].Cells[2].CellFormat.Width = 74; // Invoice Amount
                myTable.Rows[0].Cells[3].CellFormat.Width = 74; // Paid Amount
                myTable.Rows[0].Cells[4].CellFormat.Width = 74; // Thirty
                myTable.Rows[0].Cells[5].CellFormat.Width = 74; // Sixty
                myTable.Rows[0].Cells[6].CellFormat.Width = 70; // Ninty
                myTable.Rows[0].Cells[7].CellFormat.Width = 70; // Ninty Plus
                myTable.Rows[0].Cells[8].CellFormat.Width = 74; // Total Pending

                foreach (AgedARSummary item in result)
                {
                    Row clonedRow = (Row)myTable.FirstRow.Clone(true);

                    clonedRow.Cells[0].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[0].FirstParagraph);
                    builder.Write(item.FirmName.ToString());

                    clonedRow.Cells[1].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[1].FirstParagraph);
                    builder.Write(item.FirmID.ToString());

                    clonedRow.Cells[2].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[2].FirstParagraph);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    builder.Write(item.InvoiceAmount.ToString("C"));

                    clonedRow.Cells[3].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[3].FirstParagraph);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    builder.Write(item.PaidAmount.ToString("C"));

                    clonedRow.Cells[4].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[4].FirstParagraph);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    builder.Write(item.ThirtyDaysAmount.ToString("C"));

                    clonedRow.Cells[5].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[5].FirstParagraph);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    builder.Write(item.SixtyDaysAmount.ToString("C"));

                    clonedRow.Cells[6].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[6].FirstParagraph);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    builder.Write(item.NintyDaysAmount.ToString("C"));

                    clonedRow.Cells[7].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[7].FirstParagraph);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    builder.Write(item.NintyPlysDaysAmount.ToString("C"));

                    clonedRow.Cells[8].FirstParagraph.ChildNodes.Clear();
                    builder.MoveTo(clonedRow.Cells[8].FirstParagraph);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    builder.Write(item.TotalPending.ToString("C"));


                    myTable.AppendChild(clonedRow);
                }
                builder.InsertBreak(BreakType.PageBreak);

                MemoryStream ms = new MemoryStream();
                ms.Flush();
                doc.Save(ms, Aspose.Words.SaveFormat.Pdf);

                response.Content = new ByteArrayContent(ms.ToArray());
                response.Content.Headers.Clear();
                response.Content.Headers.TryAddWithoutValidation("Content-Disposition", "attachment; filename=" + fileNames + ".pdf");
                response.Content.Headers.Add("Content-Length", ms.ToArray().Length.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));

                return response;
            }
            catch (Exception ex)
            {
                // response.Message.Add(ex.Message);
            }
            return response;
        }

        public void BuildHeaderRow(string[] ColName, Aspose.Words.Tables.Table myTable, Aspose.Words.DocumentBuilder builder)
        {
            foreach (string col in ColName)
            {
                //Clone first cell of first row to build header of the table
                Cell hCell = (Cell)myTable.FirstRow.FirstCell.Clone(true);

                //Insert cell into the first row
                myTable.FirstRow.AppendChild(hCell);

                // myTable.LastRow.AppendChild(hCell);

                //Move document builder cursor to the cell
                builder.MoveTo(hCell.FirstParagraph);

                //Insert text
                builder.Write(col);
            }
            //string[] ColName = { "Invoice No", "OrderNo", "Invoice Amount", "Paid Amount", "Balance", "Invoice Date", "Firm","Patient","Location","Days" };
            myTable.Rows[0].Cells[0].Remove();
            myTable.Rows[1].Cells[0].Remove();

            myTable.Rows[0].Cells[0].CellFormat.Width = 50; //Invoice No
            myTable.Rows[0].Cells[1].CellFormat.Width = 60; // OrderNo
            myTable.Rows[0].Cells[2].CellFormat.Width = 80; // Invoice Amount
            myTable.Rows[0].Cells[3].CellFormat.Width = 80; // Paid Amount
            myTable.Rows[0].Cells[4].CellFormat.Width = 50; // Balance
            myTable.Rows[0].Cells[5].CellFormat.Width = 70; // Invoice Date
            myTable.Rows[0].Cells[6].CellFormat.Width = 150; // Firm            
            myTable.Rows[0].Cells[7].CellFormat.Width = 150; // Location
            myTable.Rows[0].Cells[8].CellFormat.Width = 35; // Days

        }

        public void InsertData(List<AgedAR> result, Aspose.Words.Tables.Table myTable, Aspose.Words.DocumentBuilder builder)
        {
            string partLink = string.Empty;
            string invoiceLink = string.Empty;
            string strLiveURL = System.Configuration.ConfigurationManager.AppSettings["LiveSiteURL"].ToString();
            // string[] ColName = { "Part No", "Day", "Records Of","Location", "Client Matter", "Claim No", "Note" };
            foreach (AgedAR item in result)
            {
                Row clonedRow = (Row)myTable.FirstRow.Clone(true);
                // https://www.axiomcopyonline.com/PrintInvoice?InvoiceID=622265&OrderId=61795&PartNo=15
                // clonedRow.Cells[0].CellFormat.Width = 100;
                partLink = strLiveURL + "/" + "PartDetail?OrderId=" + item.OrderNo.Split('-')[0] + "&PartNo=" + item.OrderNo.Split('-')[1];
                invoiceLink = strLiveURL + "/" + "PrintInvoice?InvoiceID=" + item.InvoiceNo + "&OrderId=" + item.OrderNo.Split('-')[0] + "&PartNo=" + item.OrderNo.Split('-')[1];

                clonedRow.Cells[0].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[0].FirstParagraph);
                builder.Font.Color = Color.OrangeRed;
                // builder.Font.Underline = Underline.Single;

                builder.InsertHyperlink(item.InvoiceNo.ToString(), invoiceLink, false);
                // builder.InsertField(@"HYPERLINK " +  partLink + "\t_blank", item.OrderPart);

                builder.Font.ClearFormatting();

                // clonedRow.Cells[1].CellFormat.Width = 50;
                clonedRow.Cells[1].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[1].FirstParagraph);
                builder.Font.Color = Color.OrangeRed;
                builder.InsertHyperlink(item.OrderNo.ToString(), partLink, false);
                builder.Font.ClearFormatting();


                clonedRow.Cells[2].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[2].FirstParagraph);
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                builder.Write(item.InvoiceAmount.ToString());

                clonedRow.Cells[3].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[3].FirstParagraph);
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                builder.Write(item.PaidAmount.ToString());

                clonedRow.Cells[4].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[4].FirstParagraph);
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                builder.Write(item.Balance.ToString());

                clonedRow.Cells[5].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[5].FirstParagraph);
                builder.Write(item.InvoiceDate);

                clonedRow.Cells[6].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[6].FirstParagraph);
                builder.Write(item.FirmName);

                //clonedRow.Cells[7].FirstParagraph.ChildNodes.Clear();
                //builder.MoveTo(clonedRow.Cells[7].FirstParagraph);
                //builder.Write(item.Patient);

                clonedRow.Cells[7].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[7].FirstParagraph);
                builder.Write(item.Location);

                clonedRow.Cells[8].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[8].FirstParagraph);
                builder.Write(item.Days.ToString());


                myTable.AppendChild(clonedRow);
            }
            builder.InsertBreak(BreakType.PageBreak);
        }

    }
}
