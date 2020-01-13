using Aspose.Pdf.Facades;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Axiom.Web.Reports
{
    public partial class InvoiceBatchReport : System.Web.UI.Page
    {
        #region Initialization
        public DsInvoice dsCustomers;
        public DataSet cloneSet;
        public DataSet dsProgList;
        public enum ProcessingMode
        {
            Local = 0,
            Remote = 1,
        }

        private string _FirmID
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["FirmID"]) ? Request.QueryString["FirmID"] : "";
            }
        }
        private string _Caption
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["Caption"]) ? Request.QueryString["Caption"] : "";
            }
        }

        private string _ClaimNo
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["ClaimNo"]) ? Request.QueryString["ClaimNo"] : "";
            }
        }
        private string _InvoiceNO
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["InvoiceNO"]) ? Request.QueryString["InvoiceNO"] : "";
            }
        }

        private string _AttyID
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["AttyID"]) ? Request.QueryString["AttyID"] : "";
            }
        }

        private string _SoldAttyName
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["SoldAttyName"]) ? Request.QueryString["SoldAttyName"] : "";
            }
        }

        private string _FromDate
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["FromDate"]) ? Request.QueryString["FromDate"] : "";
            }
        }

        private string _ToDate
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["ToDate"]) ? Request.QueryString["ToDate"] : "";
            }
        }

        private bool? _Invoice
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["Invoice"]) ? Convert.ToBoolean(Request.QueryString["Invoice"]) : false;
            }
        }

        private bool? _Statement
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["Statement"]) ? Convert.ToBoolean(Request.QueryString["Statement"]) : false;
            }
        }

        private bool? _OpenInvoiceOnly
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["OpenInvoiceOnly"]) ? Convert.ToBoolean(Request.QueryString["OpenInvoiceOnly"]) : false;
            }
        }

        private bool? _OnlyFilterByInvoice
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["OnlyFilterByInvoice"]) ? Convert.ToBoolean(Request.QueryString["OnlyFilterByInvoice"]) : false;
            }
        }
        private string _CompanyNo
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["CompanyNo"]) ? Request.QueryString["CompanyNo"] : "";
            }
        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                BindInvoiceBatchReport();               
            }

        }

        protected void BindInvoiceBatchReport()
            {

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            byte[] bSummary;
            byte[] bDetail;

            string FirmID = _FirmID;
            string Caption = _Caption;
            string ClaimNo = _ClaimNo;
            string InvoiceNO = _InvoiceNO;
            string AttyID = _AttyID;
            string SoldAttyName = _SoldAttyName;
            string FromDate = _FromDate;
            string ToDate = _ToDate;
            bool? Invoice = _Invoice;
            bool? Statement = _Statement;
            bool? OpenInvoiceOnly = _OpenInvoiceOnly;
            string CompanyNo = _CompanyNo;

            if (String.IsNullOrEmpty(FromDate))
            {
                FromDate = DateTime.Now.ToString("MM/dd/yyyy");
            }
            if (String.IsNullOrEmpty(ToDate))
            {
                ToDate = DateTime.Now.ToString("MM/dd/yyyy");
            }

            try
            {
                if (_OnlyFilterByInvoice == true)
                {
                    dsCustomers = GetCustomersReport_Invoice(FirmID, Caption, ClaimNo, InvoiceNO, AttyID, SoldAttyName, FromDate, ToDate, Invoice, Statement, OpenInvoiceOnly,OnlyFilterByInvoice: true,CompanyNo: CompanyNo);
                    this.ReportViewer1.PageCountMode = PageCountMode.Actual;
                    ReportViewer1.Width = Unit.Percentage(100);
                    ReportViewer1.ProcessingMode = 0;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/InvoiceNew.rdlc");
                    ReportDataSource datasource = new ReportDataSource("dsInvoice", dsCustomers.Tables["DtInvoice"]);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource);
                    ReportViewer1.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
                    ReportViewer1.LocalReport.Refresh();
                }
                else if(_Invoice.Value && _Statement.Value)
                {
                    DsInvoice dsCustomers = GetCustomersReport_Statement(FirmID, Caption, ClaimNo, InvoiceNO, AttyID, SoldAttyName, FromDate, ToDate, Invoice, Statement, OpenInvoiceOnly, CompanyNo: CompanyNo);

                    ReportViewer1.Width = Unit.Percentage(100);                    
                    //ReportViewer1.Height = Unit.Percentage(100);
                    ReportViewer1.ProcessingMode = 0;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SummaryNew.rdlc");                    
                    ReportDataSource datasource1 = new ReportDataSource("dsInvoice", dsCustomers.Tables["DtInvoice"]);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource1);

                    bSummary = ReportViewer1.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                    this.ReportViewer1.PageCountMode = PageCountMode.Actual;

                    ReportViewer1.ProcessingMode = 0;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/InvoiceNew.rdlc");
                    dsCustomers = GetCustomersReport_Invoice(FirmID, Caption, ClaimNo, InvoiceNO, AttyID, SoldAttyName, FromDate, ToDate, Invoice, Statement, OpenInvoiceOnly, CompanyNo: CompanyNo);
                    ReportDataSource datasource2 = new ReportDataSource("dsInvoice", dsCustomers.Tables["DtInvoice"]);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource2);

                    ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
                    ReportViewer1.LocalReport.Refresh();

                    bDetail = ReportViewer1.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                    MemoryStream msSummary = new MemoryStream(bSummary);
                    MemoryStream msDetail = new MemoryStream(bDetail);
                    MemoryStream[] msMain = new MemoryStream[2];
                    msMain[0] = msSummary;
                    msMain[1] = msDetail;

                    Aspose.Pdf.License license = new Aspose.Pdf.License();
                    license.SetLicense("Aspose.Pdf.lic");

                    MemoryStream pdfStream = new MemoryStream();
                    PdfFileEditor pdfEditor = new PdfFileEditor();
                    pdfEditor.Concatenate(msMain, pdfStream);

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "inline; filename=" + "output.pdf");
                    Response.BinaryWrite(pdfStream.ToArray());
                    pdfStream.Close();
                    Response.End();
                }

               else if (_Invoice.Value)
                {
                     dsCustomers = GetCustomersReport_Invoice(FirmID, Caption, ClaimNo, InvoiceNO, AttyID, SoldAttyName, FromDate, ToDate, Invoice, Statement, OpenInvoiceOnly, CompanyNo: CompanyNo);
                    this.ReportViewer1.PageCountMode = PageCountMode.Actual;
                    ReportViewer1.Width = Unit.Percentage(100);
                    ReportViewer1.ProcessingMode = 0;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/InvoiceNew.rdlc");                                        
                    ReportDataSource datasource = new ReportDataSource("dsInvoice", dsCustomers.Tables["DtInvoice"]);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource);
                    ReportViewer1.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
                    ReportViewer1.LocalReport.Refresh();
                }
                else if (_Statement.Value)
                {
                    DsInvoice dsCustomers = GetCustomersReport_Statement(FirmID, Caption, ClaimNo, InvoiceNO, AttyID, SoldAttyName, FromDate, ToDate, Invoice, Statement, OpenInvoiceOnly, CompanyNo: CompanyNo);
                    ReportViewer1.Width = Unit.Percentage(100);
                    this.ReportViewer1.PageCountMode = PageCountMode.Actual;                    
                    ReportViewer1.ProcessingMode = 0;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/SummaryNew.rdlc");                    
                    ReportDataSource datasource = new ReportDataSource("dsInvoice", dsCustomers.Tables["DtInvoice"]);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource);
                    ReportViewer1.LocalReport.Refresh();
                }

            }
            catch (Exception ex)
            {
                
            }
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            string strInvNo = e.Parameters["InvNo"].Values[0].ToString();
            string strOrderNo = e.Parameters["OrderNo"].Values[0].ToString();
            ReportDataSource rdS = new ReportDataSource("dsCharges", GetSubreportdataWithCharges(strInvNo, strOrderNo));
            e.DataSources.Add(rdS);
        }

        public DataTable GetSubreportdataWithCharges(string strInvNo, string strOrderNo)
        {
            var conString = ConfigurationManager.ConnectionStrings["Axiom"];
            string strConnString = conString.ConnectionString;
            SqlConnection conn = new SqlConnection(strConnString);
            conn.Open();

            try
            {

                SqlCommand sqlCmd = new SqlCommand("GetInvoiceCharges_New", conn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@InvNo", (object)strInvNo ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@OrderNo", (object)strOrderNo ?? (object)DBNull.Value);
                sqlCmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                conn.Close();

                using (DsInvoice dsCustomers = new DsInvoice())
                {
                    da.Fill(dsCustomers.Tables["DtCharges"]);

                    return dsCustomers.Tables["DtCharges"];
                }
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dsCustomers.Tables["DtCharges"];

        }
        public DataTable GetSubreportdata(string strInvNo, string strOrderNo)
        {
            charges(strInvNo, strOrderNo);
            var conString = ConfigurationManager.ConnectionStrings["Axiom"];
            string strConnString = conString.ConnectionString;
            SqlConnection conn = new SqlConnection(strConnString);
            conn.Open();

            try
            {
                
                SqlCommand sqlCmd = new SqlCommand("GetInvoiceSubReportData", conn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@InvNo", (object)strInvNo ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@OrderNo", (object)strOrderNo ?? (object)DBNull.Value);        
                sqlCmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                conn.Close();
                //DataTable DtCharges_Customers = new DataTable("DtCharges");               
                //if (!dsCustomers.Tables.Contains(DtCharges_Customers.TableName))
                //{
                //    dsCustomers.Tables.Add(DtCharges_Customers);
                //}

                //DataTable DtCharges_cloneSet = new DataTable("DtCharges");
                //if (!cloneSet.Tables.Contains(DtCharges_Customers.TableName))
                //{
                //    cloneSet.Tables.Add(DtCharges_cloneSet);
                //}
                using (DsInvoice dsCustomers = new DsInvoice())
                {
                    da.Fill(dsCustomers.Tables["DtCharges"]);
                    cloneSet.Tables["DtCharges"].Merge(dsCustomers.Tables["DtCharges"]);
                    return cloneSet.Tables["DtCharges"];
                }
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return cloneSet.Tables["DtCharges"];
           
        }
        public void charges(string strInvNo, string strOrderNo)
        {
            cloneSet = null;
            var conString = ConfigurationManager.ConnectionStrings["Axiom"];
            string strConnString = conString.ConnectionString;

            SqlConnection conn = new SqlConnection(strConnString);
            conn.Open();
            try
            {
                

                SqlCommand sqlCmd = new SqlCommand("GetInvoiceCharges", conn);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@InvNo", (object)strInvNo ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@OrderNo", (object)strOrderNo ?? (object)DBNull.Value);

                sqlCmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                DataSet customers = new DataSet();
                da.Fill(customers, "Customers");
                conn.Close();
                cloneSet = dsCustomers.Clone();

                for (int i = 0; i < customers.Tables[0].Rows.Count; i++)
                {
                    if (customers.Tables[0].Rows[i]["StdFee1"].ToString() != "0.00")
                    {
                        DataRow dr = cloneSet.Tables["DtCharges"].NewRow();
                        dr["InvNo"] = Convert.ToInt32(strInvNo);
                        dr["Description"] = "BASIC FEE";
                        dr["Charges"] = customers.Tables[0].Rows[i]["StdFee1"].ToString();
                        cloneSet.Tables["DtCharges"].Rows.Add(dr);
                    }
                    if (customers.Tables[0].Rows[i]["StdFee2"].ToString() != "0.00")
                    {
                        DataRow dr = cloneSet.Tables["DtCharges"].NewRow();
                        dr["InvNo"] = Convert.ToInt32(strInvNo);
                        dr["Description"] = "SUBPOENA/REQUEST";
                        dr["Charges"] = customers.Tables[0].Rows[i]["StdFee2"].ToString();
                        cloneSet.Tables["DtCharges"].Rows.Add(dr);
                    }
                    if (customers.Tables[0].Rows[i]["StdFee3"].ToString() != "0.00")
                    {
                        DataRow dr = cloneSet.Tables["DtCharges"].NewRow();
                        dr["InvNo"] = Convert.ToInt32(strInvNo);
                        dr["Description"] = "RETURN ORIGINALS";
                        dr["Charges"] = customers.Tables[0].Rows[i]["StdFee3"].ToString();
                        cloneSet.Tables["DtCharges"].Rows.Add(dr);
                    }
                    if (customers.Tables[0].Rows[i]["StdFee4"].ToString() != "0.00")
                    {
                        DataRow dr = cloneSet.Tables["DtCharges"].NewRow();
                        dr["InvNo"] = Convert.ToInt32(strInvNo);
                        dr["Description"] = "CUSTODIAN FEE";
                        dr["Charges"] = customers.Tables[0].Rows[i]["StdFee4"].ToString();
                        cloneSet.Tables["DtCharges"].Rows.Add(dr);
                    }
                    if (customers.Tables[0].Rows[i]["StdFee5"].ToString() != "0.00")
                    {
                        DataRow dr = cloneSet.Tables["DtCharges"].NewRow();
                        dr["InvNo"] = Convert.ToInt32(strInvNo);
                        dr["Description"] = "PRINTING & BINDING";
                        dr["Charges"] = customers.Tables[0].Rows[i]["StdFee5"].ToString();
                        cloneSet.Tables["DtCharges"].Rows.Add(dr);
                    }
                    if (customers.Tables[0].Rows[i]["StdFee6"].ToString() != "0.00")
                    {
                        DataRow dr = cloneSet.Tables["DtCharges"].NewRow();
                        dr["InvNo"] = Convert.ToInt32(strInvNo);
                        dr["Description"] = "SHIPPING/HANDLING";
                        dr["Charges"] = customers.Tables[0].Rows[i]["StdFee6"].ToString();
                        cloneSet.Tables["DtCharges"].Rows.Add(dr);
                    }
                    if (customers.Tables[0].Rows[i]["StdFee7"].ToString() != "0.00")
                    {
                        DataRow dr = cloneSet.Tables["DtCharges"].NewRow();
                        dr["InvNo"] = Convert.ToInt32(strInvNo);
                        dr["Description"] = "TRIP(S)";
                        dr["Charges"] = customers.Tables[0].Rows[i]["StdFee7"].ToString();
                        cloneSet.Tables["DtCharges"].Rows.Add(dr);
                    }
                    if (customers.Tables[0].Rows[i]["StdFee8"].ToString() != "0.00")
                    {
                        DataRow dr = cloneSet.Tables["DtCharges"].NewRow();
                        dr["InvNo"] = Convert.ToInt32(strInvNo);
                        dr["Description"] = "NOTICE MAILING CHARGE(S)";
                        dr["Charges"] = customers.Tables[0].Rows[i]["StdFee8"].ToString();
                        cloneSet.Tables["DtCharges"].Rows.Add(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                conn.Close();
            }                      
        }
        public DsInvoice GetCustomersReport_Statement(string FirmID, string Caption, string ClaimNo, string InvoiceNO, string AttyID, string SoldAttyName, string FromDate, string ToDate,bool? Invoice,bool? Statement,bool? OpenInvoiceOnly, string CompanyNo = "1")
        {
            var conString = ConfigurationManager.ConnectionStrings["Axiom"];
            string strConnString = conString.ConnectionString;

            SqlConnection conn = new SqlConnection(strConnString);
            conn.Open();

            try
            {               
                SqlCommand sqlCmd = new SqlCommand("GetCustomersReport_Statement", conn);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@FirmID",(object)FirmID?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Caption", (object)Caption ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@ClaimNo", (object)ClaimNo ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@InvoiceNO", (object)InvoiceNO ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@AttyID", (object)AttyID ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@SoldAttyName", (object)SoldAttyName ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@FromDate", (object)FromDate ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@ToDate", (object)ToDate ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Invoice", (object)Invoice ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Statement", (object)Statement ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@OpenInvoiceOnly", (object)OpenInvoiceOnly ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@CompanyNo", CompanyNo);

                sqlCmd.ExecuteNonQuery();               
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                conn.Close();
                using (DsInvoice dsCustomers = new DsInvoice())
                {
                    da.Fill(dsCustomers.Tables["DtInvoice"]);
                    return dsCustomers;
                }               
            }
            catch (Exception ex)
            {
                conn.Close();
            }

            return null;
        }
        public DsInvoice GetCustomersReport_Invoice(string FirmID, string Caption, string ClaimNo, string InvoiceNO, string AttyID, string SoldAttyName, string FromDate, string ToDate, bool? Invoice, bool? Statement, bool? OpenInvoiceOnly, bool? OnlyFilterByInvoice = false, string CompanyNo="1")
        {
            var conString = ConfigurationManager.ConnectionStrings["Axiom"];
            string strConnString = conString.ConnectionString;

            SqlConnection conn = new SqlConnection(strConnString);
            conn.Open();

            try
            {              
                SqlCommand sqlCmd = new SqlCommand("GetCustomersReport_Invoice", conn);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                if (OnlyFilterByInvoice==false)
                {
                    sqlCmd.Parameters.AddWithValue("@FirmID", (object)FirmID ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@Caption", (object)Caption ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@ClaimNo", (object)ClaimNo ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@InvoiceNO", (object)InvoiceNO ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@AttyID", (object)AttyID ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@SoldAttyName", (object)SoldAttyName ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@FromDate", (object)FromDate ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@ToDate", (object)ToDate ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@Invoice", (object)Invoice ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@Statement", (object)Statement ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@OpenInvoiceOnly", (object)OpenInvoiceOnly ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@OnlyFilterByInvoice", false);
                    sqlCmd.Parameters.AddWithValue("@CompanyNo", CompanyNo);
                }
                else
                {
                    sqlCmd.Parameters.AddWithValue("@InvoiceNO", (object)InvoiceNO ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@OnlyFilterByInvoice", (object)OnlyFilterByInvoice ?? (object)DBNull.Value);
                    sqlCmd.Parameters.AddWithValue("@CompanyNo", CompanyNo);
                }
                
                sqlCmd.ExecuteNonQuery();                
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                conn.Close();
                using (DsInvoice dsCustomers = new DsInvoice())
                {
                    da.Fill(dsCustomers.Tables["DtInvoice"]);
                    return dsCustomers;
                }
            }
            catch (Exception ex)
            {
                conn.Close();
            }

            return null;
        }
    }
}