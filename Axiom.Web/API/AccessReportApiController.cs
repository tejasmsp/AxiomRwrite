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
using System.Reflection;
using System.Net.Http.Headers;
using System.Web;
using System.IO;
using System.Text;
using System.Data;

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

                    SqlParameter[] param1 = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value),
                                            new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value),
                                            new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value) };

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
                    // GroverBillingEntity

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


            var response = new ApiResponse<InvoiceByDateEntity>();

            SqlParameter[] param = {new SqlParameter("StartDate", (object)StartDate ?? (object)DBNull.Value),
                                            new SqlParameter("EndDate", (object)EndDate ?? (object)DBNull.Value),
                                            new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value) };

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
            SqlParameter[] param = { new SqlParameter("CompanyNo", (object)CompanyID ?? (object)DBNull.Value) };

            var result = _repository.ExecuteSQL<NonInvoicedPartsEntity>("AccessNonInvoicedParts", param).ToList();

            if (result == null)
                result = new List<NonInvoicedPartsEntity>();

            response.Data = result;
            return response;

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

    }
}
