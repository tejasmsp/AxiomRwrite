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
using NReco.PdfGenerator;
using System.Net.Http.Headers;
using System.Web;
using Aspose.Words;
using Aspose.Words.Tables;
using System.IO;
using Aspose.Pdf.Text;
using System.Drawing;
using Aspose.Words.Fields;


namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class ClientApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<HomeEntity> _repository = new GenericRepository<HomeEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetClientPartStatus")]
        public ApiResponse<ClientPartListEntity> GetClientPartStatus(string UserID)
        {
            var response = new ApiResponse<ClientPartListEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value) };

                var result = _repository.ExecuteSQL<ClientPartListEntity>("GetClientPartStatus", param).ToList();
                if (result == null)
                {
                    result = new List<ClientPartListEntity>();
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
        [Route("GetClientDashboardOrders")]
        public ApiResponse<ClientOrderListEntity> GetClientDashboardOrders(string UserID)
        {
            var response = new ApiResponse<ClientOrderListEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value) };
                //GetClientDashboardOrders
                var result = _repository.ExecuteSQL<ClientOrderListEntity>("GetClientDashboard", param).ToList();
                if (result == null)
                {
                    result = new List<ClientOrderListEntity>();
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
        [Route("GetClientDashboardParts")]
        public ApiResponse<ClientOrderPartListEntity> GetClientDashboardParts(int OrderId, int PartStatusGroupId)
        {
            var response = new ApiResponse<ClientOrderPartListEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value),
                new SqlParameter("PartStatusGroupId", (object)PartStatusGroupId ?? (object)DBNull.Value)};
                //GetClientDashboardParts
                var result = _repository.ExecuteSQL<ClientOrderPartListEntity>("GetClientDashboardParts_New", param).ToList();
                if (result == null)
                {
                    result = new List<ClientOrderPartListEntity>();
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
        [Route("GetClientFileByFileType")]
        public ApiResponse<FileEntity> GetClientFileByFileType(int OrderId, int PartNo, int FileTypeId)
        {
            var response = new ApiResponse<FileEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value),
                new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value),
                new SqlParameter("FileTypeId", (object)FileTypeId ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<FileEntity>("GetClientFileByFileType", param).ToList();
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
        [Route("DownloadClientRecords")]
        public HttpResponseMessage DownloadClientRecords(List<ClientPartListEntity> modal)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            string fileNames = "ClientRecords";

            try
            {
                byte[] bytes;// = new byte[file.Length];

                System.Text.StringBuilder htmlBody = new System.Text.StringBuilder();

                string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "MailTemplate\\ClientRecords\\ClientRecords.html";
                using (System.IO.StreamReader reader = new System.IO.StreamReader((htmlfilePath)))
                {
                    htmlBody.Append(reader.ReadToEnd());
                }
                System.Text.StringBuilder sbTable = new System.Text.StringBuilder();
                foreach (var item in modal)
                {
                    sbTable.Append("<tr class='row100 body'>");
                    sbTable.Append("<td class='cell100 column1'>" + item.OrderNo + "</td>");
                    sbTable.Append("<td class='cell100 column2'>" + item.CliMatNo + "</td>");
                    sbTable.Append("<td class='cell100 column3'>" + item.RecordsOf + "</td>");
                    //sbTable.Append("<td class='cell100 column4'>" + item.Location + "</td>");
                    //sbTable.Append("<td class='cell100 column5'>" + item.Type + "</td>");
                    sbTable.Append("</tr>");
                }
                htmlBody.Replace("##TR##", sbTable.ToString());
                string fname = AppDomain.CurrentDomain.BaseDirectory + "MailTemplate\\ClientRecords\\" + DateTime.Now.ToString("MM-dd-yyyy-hhmm") + Guid.NewGuid() + ".html";
                System.IO.File.WriteAllText(fname, htmlBody.ToString());

                var htmlToPdf = new HtmlToPdfConverter();
                bytes = htmlToPdf.GeneratePdfFromFile(fname, "");

                response.Content = new ByteArrayContent(bytes);

                response.Content.Headers.Clear();
                response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + fileNames + ".pdf");
                response.Content.Headers.Add("Content-Length", bytes.Length.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));

                if (System.IO.File.Exists(fname))
                {
                    System.IO.File.Delete(fname);
                }

                return response;
            }
            catch (Exception ex)
            {
                // response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route("ClientDashboardRemovePartFromList")]
        public BaseApiResponse ClientDashboardRemovePartFromList(int OrderNo, int PartNo)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value),
                new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<int>("ClientDashboardRemovePartFromList", param).FirstOrDefault();
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

        //ClientPartReportAuthorization

        [HttpGet]
        [Route("ClientPartReport")]
        public HttpResponseMessage ClientPartReport(string UserID, string ReportName)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            string fileNames = "ClientRecords";
            string strLiveURL = System.Configuration.ConfigurationManager.AppSettings["LiveSiteURL"].ToString();
            string DocumentPath = System.Configuration.ConfigurationManager.AppSettings["ClientReportDocumentPath"].ToString();
            try
            {
                // TableThirty
                // TableSixty
                // TableNinty
                // TableNintyPlus

                SqlParameter[] param = { new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value)
                };
                List<ClientPartReport> result = new List<ClientPartReport>();
                if (ReportName.ToLower() == "authorization")
                {
                    result = _repository.ExecuteSQL<ClientPartReport>("ClientPartReportAuthorization", param).ToList<ClientPartReport>();
                    fileNames += " - Authorization Needed";
                }
                else if (ReportName.ToLower() == "newrecords")
                {
                    result = _repository.ExecuteSQL<ClientPartReport>("ClientPartReportNewRecords", param).ToList<ClientPartReport>();
                    fileNames += " - New Records";
                }
                else if (ReportName.ToLower() == "inprogress")
                {
                    result = _repository.ExecuteSQL<ClientPartReport>("ClientPartReportInProgress", param).ToList<ClientPartReport>();
                    fileNames += " - In Progress";
                }
                else 
                {
                    result = _repository.ExecuteSQL<ClientPartReport>("ClientPartReportMoreInformation", param).ToList<ClientPartReport>();
                    fileNames += " - More Information";
                }


                if (result.Count > 0)
                {
                    // response.Success = true;
                }

                List<ClientPartReport> resultThirty;
                List<ClientPartReport> resultSixty;
                List<ClientPartReport> resultNinty;
                List<ClientPartReport> resultNintyPlus;


                resultThirty = result.Where(x => x.OrderDays <= 30).OrderByDescending(x=> x.OrderDays).ToList<ClientPartReport>();
                resultSixty = result.Where(x => x.OrderDays > 30 && x.OrderDays <= 60).OrderByDescending(x => x.OrderDays).ToList<ClientPartReport>();
                resultNinty = result.Where(x => x.OrderDays > 60 && x.OrderDays <= 90).OrderByDescending(x => x.OrderDays).ToList<ClientPartReport>();
                resultNintyPlus = result.Where(x => x.OrderDays > 90).OrderBy(x => x.OrderNo).OrderByDescending(x => x.OrderDays).ToList<ClientPartReport>();

                string[] ColName = { "Part No", "Day", "Records Of","Location", "Client Matter", "Claim No", "Note" };
                //Get dummy datasource (data table with random number of rows)

                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense("Aspose.Words.lic");
                //Open or create document and create DocumentBuilder
                Aspose.Words.Document doc = new Aspose.Words.Document(DocumentPath);

                //Document builder will be needed to build table in the document
                Aspose.Words.DocumentBuilder builder = new DocumentBuilder(doc);


                //move documentBuilder cursor to the bookmark inside table

                builder.MoveToBookmark("TableThirty");
                Aspose.Words.Tables.Table myTable = (Aspose.Words.Tables.Table)builder.CurrentNode.GetAncestor(NodeType.Table);
                BuildHeaderRow(ColName, myTable, builder);
                InsertData(resultThirty, myTable, builder);

                // builder.InsertBreak(BreakType.SectionBreakNewPage);
                builder.MoveToBookmark("TableSixty");
                myTable = (Aspose.Words.Tables.Table)builder.CurrentNode.GetAncestor(NodeType.Table);
                BuildHeaderRow(ColName, myTable, builder);
                InsertData(resultSixty, myTable, builder);

                // builder.InsertBreak(BreakType.SectionBreakNewPage);
                builder.MoveToBookmark("TableNinty");
                myTable = (Aspose.Words.Tables.Table)builder.CurrentNode.GetAncestor(NodeType.Table);
                BuildHeaderRow(ColName, myTable, builder);
                InsertData(resultNinty, myTable, builder);

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

            myTable.Rows[0].Cells[0].Remove();
            myTable.Rows[1].Cells[0].Remove();

            myTable.Rows[0].Cells[0].CellFormat.Width = 60; //ORDERNO-PARTNO LINK
            myTable.Rows[0].Cells[1].CellFormat.Width = 30; // DAY
            myTable.Rows[0].Cells[2].CellFormat.Width = 100; // RECORDS OF 
            myTable.Rows[0].Cells[3].CellFormat.Width = 150; // LOCATION
            myTable.Rows[0].Cells[4].CellFormat.Width = 80; // CLIENT MATTER NUMBER
            myTable.Rows[0].Cells[5].CellFormat.Width = 80; // CLAIM NUMBER
            myTable.Rows[0].Cells[6].CellFormat.Width = 270; // NOTE
        }

        public void InsertData(List<ClientPartReport> result, Aspose.Words.Tables.Table myTable, Aspose.Words.DocumentBuilder builder)
        {
            string partLink = string.Empty;
            string strLiveURL = System.Configuration.ConfigurationManager.AppSettings["LiveSiteURL"].ToString();
            // string[] ColName = { "Part No", "Day", "Records Of","Location", "Client Matter", "Claim No", "Note" };
            foreach (ClientPartReport item in result)
            {
                Row clonedRow = (Row)myTable.FirstRow.Clone(true);

                // clonedRow.Cells[0].CellFormat.Width = 100;
                partLink = strLiveURL + "/" + "PartDetail?OrderId=" + item.OrderNo + "&PartNo=" + item.PartNo;

                clonedRow.Cells[0].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[0].FirstParagraph);
                builder.Font.Color = Color.OrangeRed;
                // builder.Font.Underline = Underline.Single;

                builder.InsertHyperlink(item.OrderPart, partLink, false);
                // builder.InsertField(@"HYPERLINK " +  partLink + "\t_blank", item.OrderPart);

                builder.Font.ClearFormatting();

                // clonedRow.Cells[1].CellFormat.Width = 50;
                clonedRow.Cells[1].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[1].FirstParagraph);
                builder.Write(item.OrderDays.ToString());

                clonedRow.Cells[2].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[2].FirstParagraph);
                builder.Write(item.RecordsOf);

                clonedRow.Cells[3].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[3].FirstParagraph);
                builder.Write(item.Location);

                clonedRow.Cells[4].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[4].FirstParagraph);
                builder.Write(item.ClaimMatterNo);

                clonedRow.Cells[5].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[5].FirstParagraph);
                builder.Write(item.BillingClaimNo);

                clonedRow.Cells[6].FirstParagraph.ChildNodes.Clear();
                builder.MoveTo(clonedRow.Cells[6].FirstParagraph);
                builder.Write(item.Note);


                myTable.AppendChild(clonedRow);
            }
            builder.InsertBreak(BreakType.PageBreak);
        }


        #endregion
    }
}