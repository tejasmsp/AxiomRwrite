using QuickFormService.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Aspose.Words;
using System.Configuration;

namespace QuickFormService
{
    public class AttachmentInsert
    {

        public static void GenerateDocuments(int QuickFormID, ref List<string> AttachFileName, ref MemoryStream[] msFile)
        {
            try
            {
                string documentRoot = ConfigurationManager.AppSettings["DocumentRoot"];

                var dsMain = DbAccess.GetQuickFormPrintformattachfiles(Convert.ToInt32(QuickFormID), true, false);

                if (dsMain != null && dsMain.Rows.Count > 0)
                {
                    int numrows = dsMain.Rows.Count;
                    string filetype = "pdf";
                    Log.ServicLog("Quick forms generating process started.");

                    int counter = 0;
                    int partCnt = dsMain.Select("PartNo>0").Length;

                    foreach (DataRow drForm in dsMain.Rows)
                    {
                        try
                        {
                            var dtQuery = new DataTable();
                            var dtSubQuery = new DataTable();

                            bool displaySSN = Convert.ToBoolean(Convert.ToString(drForm["IsSSN"]) != "" ? Convert.ToString(drForm["IsSSN"]) : "true");
                            string docName;
                            if (drForm["DocName"].ToString().Split(new char[] { '_' }).Length == 3)
                            {
                                docName = drForm["DocName"].ToString().Split(new char[] { '_' })[2].ToUpper();
                            }
                            else if (drForm["DocName"].ToString().Split(new char[] { '_' }).Length == 4)
                            {
                                docName = drForm["DocName"].ToString().Split(new char[] { '_' })[3].ToUpper();
                            }
                            else
                            {
                                docName = drForm["DocName"].ToString().Split(new char[] { '_' })[1].ToUpper();
                            }
                            string[] folders = Convert.ToString(drForm["DocPath"]).Split('>');
                            QueryType pdt = DbAccess.GetDocumentType(docName, folders[0].ToString());
                            int orderNo = Convert.ToInt32(drForm["OrderNo"]);
                            string partIds = Convert.ToString(drForm["PartNo"]);

                            MemoryStream ms = new MemoryStream();
                            string subquery = string.Empty;
                            string query = Common.GetQueryToExecute(pdt, drForm, folders, orderNo, partIds, displaySSN, partIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length.ToString());

                            if (!string.IsNullOrEmpty(query))
                            {
                                dtQuery = DbAccess.ExecuteSQLQuery(query);
                            }
                            if (folders.Contains("Custodian Letters"))
                            {
                                dtQuery.Columns.Add(new DataColumn("Year1", typeof(string)));
                                dtQuery.Columns.Add(new DataColumn("Year2", typeof(string)));
                                dtQuery.Columns.Add(new DataColumn("Year3", typeof(string)));
                                dtQuery.Columns.Add(new DataColumn("Year4", typeof(string)));
                                dtQuery.Columns.Add(new DataColumn("Year5", typeof(string)));
                                dtQuery.Columns.Add(new DataColumn("Year6", typeof(string)));
                                dtQuery.Columns.Add(new DataColumn("Year7", typeof(string)));
                                dtQuery.Columns.Add(new DataColumn("Year8", typeof(string)));
                                dtQuery.Columns.Add(new DataColumn("Total_Cost", typeof(string)));
                                int noOfYears = 0;
                                try
                                {
                                    var enumerator2 = dtQuery.Rows.GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        DataRow dr = (DataRow)enumerator2.Current;
                                        int i = 1;
                                        try
                                        {
                                            var enumerator3 = dsMain.Columns.GetEnumerator();
                                            while (enumerator3.MoveNext())
                                            {
                                                DataColumn dc = (DataColumn)enumerator3.Current;
                                                if (string.Compare(dc.ColumnName, "Year" + Convert.ToString(i), false) == 0)
                                                {
                                                    dr["Year" + Convert.ToString(i)] = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(drForm["Year" + Convert.ToString(i)]);
                                                    if (!string.IsNullOrEmpty(Convert.ToString(drForm["Year" + Convert.ToString(i)])) && !string.IsNullOrEmpty(Convert.ToString(drForm["Year" + Convert.ToString(i)])))
                                                        noOfYears++;
                                                    i++;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.ServicLog(ex.Message);
                                            Log.ServicLog(ex.StackTrace);
                                        }
                                    }
                                }
                                catch (Exception ex) {
                                    Log.ServicLog(ex.Message);
                                    Log.ServicLog(ex.StackTrace);
                                }
                                int amount = 0;
                                var dtIRsFees = DbAccess.GetIRSFeesByYear();
                                if (dtIRsFees != null && dtIRsFees.Rows.Count > 0)
                                {
                                    amount = noOfYears * Convert.ToInt32(dtIRsFees.Rows[0]["Fees"]);
                                }
                                foreach (DataRow dr in dtQuery.Rows)
                                {
                                    dr["Total_Cost"] = amount;
                                }
                            }
                            if (pdt == QueryType.Confirmation || pdt == QueryType.TargetSheets || pdt == QueryType.StatusProgressReports)
                            {
                                subquery = DbAccess.GetQueryByQueryTypeId((int)pdt, "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    subquery = Common.ReplaceOrderPartNo(subquery, orderNo, partIds);
                                    if (!displaySSN)
                                        subquery = Common.ReplaceSSN(subquery);
                                }
                                StringBuilder partInfo = new StringBuilder();
                                StringBuilder partInfo2 = new StringBuilder();
                                partInfo.Append("_____________________________________________________________________________\n\r");

                                dtSubQuery = DbAccess.ExecuteSQLQuery(subquery);
                                for (int i = 0; i < dtSubQuery.Rows.Count; i++)
                                {
                                    string partInfoText;
                                    if (pdt == QueryType.Confirmation & docName.ToUpper().Equals("ORDER CONFIRMATION.DOC"))
                                        partInfoText = dtSubQuery.Rows[i]["PartInfo1"].ToString().Replace("scopehere", dtSubQuery.Rows[i]["scope"].ToString());
                                    else
                                        partInfoText = dtSubQuery.Rows[i]["PartInfo1"].ToString().Replace("scopehere", "");
                                    partInfo.Append(partInfoText + "\n\r");
                                    partInfo2.Append(dtSubQuery.Rows[i]["LocationHeader"] + "\n");
                                }


                                var dt2 = dtQuery;
                                DataColumn dc2 = new DataColumn("PartInfo", typeof(string));
                                DataColumn dc3 = new DataColumn("PartInfo2", typeof(string));
                                dc2.AllowDBNull = true;
                                dc3.AllowDBNull = true;
                                dt2.Columns.Add(dc2);
                                dt2.Columns.Add(dc3);

                                for (int i = 0; i < dt2.Rows.Count; i++)
                                {
                                    DataRow row = dt2.Rows[i];
                                    row["PartInfo"] = partInfo;
                                    row["PartInfo2"] = partInfo2;
                                }

                            }
                            else if (pdt == QueryType.Waiver)
                            {
                                subquery = DbAccess.GetQueryByQueryTypeId(5, "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    subquery = Common.ReplaceOrderPartNo(subquery, orderNo, partIds);
                                    dtSubQuery = DbAccess.ExecuteSQLQuery(subquery);
                                }
                                DataTable dt3 = dtSubQuery;
                                dt3.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                                dt3.Columns.Add(new DataColumn("Location1", typeof(string)));
                                StringBuilder locationInfo = new StringBuilder();

                                for (int i = 0; i < dtSubQuery.Rows.Count; i++)
                                    locationInfo.Append(Convert.ToString(dtSubQuery.Rows[i]["Location1"]) + "\n");

                                for (int j = 0; j < dt3.Rows.Count; j++)
                                {
                                    DataRow row = dt3.Rows[j];
                                    row["Selected_Part"] = partIds;
                                    row["Location1"] = locationInfo;
                                }
                            }
                            else if (pdt == QueryType.CerticicationNOD)
                            {
                                subquery = DbAccess.GetQueryByQueryTypeId(10, "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    subquery = Common.ReplaceOrderPartNo(subquery, orderNo, partIds);
                                    dtSubQuery = DbAccess.ExecuteSQLQuery(subquery);
                                }
                                StringBuilder attyInfo = new StringBuilder();
                                attyInfo.Append('\n');
                                for (int i = 0; i < dtSubQuery.Rows.Count; i++)
                                    attyInfo.Append(Convert.ToString(dtSubQuery.Rows[i]["Attorneys"]) + "\n");
                                var dt4 = dtQuery;
                                dt4.Columns.Add(new DataColumn("Attorneys", typeof(string)));
                                for (int i = 0; i < dt4.Rows.Count; i++)
                                {
                                    DataRow row = dt4.Rows[i];
                                    row["Attorneys"] = attyInfo;
                                }
                            }
                            else if (pdt == QueryType.AttorneyOfRecords)
                            {
                                string[] strQuery = null;
                                subquery = DbAccess.GetQueryByQueryTypeId(6, "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    strQuery = subquery.Split(new string[] { "--Split--" }, StringSplitOptions.RemoveEmptyEntries);
                                    subquery = Common.ReplaceOrderPartNo(strQuery[0], orderNo, partIds);
                                }
                                dtSubQuery = DbAccess.ExecuteSQLQuery(subquery);
                                StringBuilder attyInfo2 = new StringBuilder();
                                attyInfo2.Append('\n');
                                for (int i = 0; i < dtSubQuery.Rows.Count; i++)
                                    attyInfo2.Append(Convert.ToString(dtSubQuery.Rows[i]["AttyInfo"]));
                                attyInfo2.Append('\n');
                                subquery = Common.ReplaceOrderPartNo(strQuery[1], orderNo, partIds);
                                dtSubQuery = DbAccess.ExecuteSQLQuery(subquery);
                                for (int j = 0; j < dtSubQuery.Rows.Count; j++)
                                    attyInfo2.Append(Convert.ToString("\n" + dtSubQuery.Rows[j]["AttyInfo"]) + "\n");

                                var dt5 = dtQuery;
                                dt5.Columns.Add(new DataColumn("AttyInfo", typeof(string)));
                                for (int k = 0; k < dt5.Rows.Count; k++)
                                {
                                    DataRow row = dt5.Rows[k];
                                    row["AttyInfo"] = attyInfo2;
                                }
                            }
                            else if (pdt == QueryType.AttorneyForms)
                            {
                                string attyId3 = drForm["DocName"].ToString().Split(new char[] { '_' })[2];
                                subquery = DbAccess.GetQueryByQueryTypeId(13, "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    subquery = Common.ReplaceOrderPartNo(subquery, orderNo, partIds).Replace("%%ATTYNO%%", "" + attyId3 + "");
                                }
                                dtSubQuery = DbAccess.ExecuteSQLQuery(subquery);
                                var dt6 = dtQuery;
                                dt6.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                                dt6.Columns.Add(new DataColumn("Location1", typeof(string)));
                                StringBuilder locationInfo2 = new StringBuilder();
                                StringBuilder invoiceInfo = new StringBuilder();
                                for (int i = 0; i < dtSubQuery.Rows.Count; i++)
                                    locationInfo2.Append(Convert.ToString(dtSubQuery.Rows[i]["Location1"]) + "\n");

                                for (int j = 0; j < dt6.Rows.Count; j++)
                                {
                                    DataRow row = dt6.Rows[j];
                                    row["Selected_Part"] = partIds;
                                    row["Location1"] = locationInfo2;
                                }
                            }
                            License license = new License();
                            license.SetLicense("Aspose.Words.lic");
                            string filePath = Path.Combine(documentRoot, drForm["DocPath"].ToString().Trim().Replace(">", "/"), docName.Trim());




                            #region Add Company Wise logo 

                            Document doc;
                            string companyLogoDirectory = ConfigurationManager.AppSettings["CompanyLogoDirectory"];
                            doc = Common.InsertHeaderLogo(filePath, string.Format("{0}logo-axiom_{1}.png", companyLogoDirectory, drForm["CompanyNo"]));

                            //string[] TestOrderNo = ConfigurationManager.AppSettings["TestOrderNo"].Split(',');
                            //if (TestOrderNo.Contains(orderNo.ToString()))
                            //{
                            //    doc = Common.InsertHeaderLogo(filePath, string.Format("{0}logo-axiom_{1}.png", companyLogoDirectory, drForm["CompanyNo"]));
                            //}
                            //else
                            //{
                            //    //OLD Code: //Document doc = new Document(filePath);
                            //    doc = new Document(filePath);
                            //}

                            //OLD Code: //Document doc = new Document(filePath);

                            // Document doc = Common.InsertHeaderLogo(filePath, string.Format("{0}logo-axiom_{1}.png", companyLogoDirectory, drForm["CompanyNo"]));
                            #endregion

                            doc.MailMerge.Execute(dtQuery);
                            doc.Save(ms, SaveFormat.Pdf);
                            string outputFileName = "printforms";
                            outputFileName = Path.GetFileNameWithoutExtension(Convert.ToString(drForm["DocName"]).Replace(" ", "-") + "." + filetype);
                            Guid gid = Guid.NewGuid();
                            if (partIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length > 1 || (pdt == QueryType.AttorneyForms || pdt == QueryType.Confirmation || pdt == QueryType.Waiver))
                            {
                                if (pdt == QueryType.AttorneyForms)
                                {
                                    string attorney = "";
                                    if (drForm["DocName"].ToString().Split(new char[] { '_' }).Length == 4)
                                    {
                                        attorney = drForm["DocName"].ToString().Split(new char[] { '_' })[2].ToUpper();
                                    }
                                    outputFileName = string.Format("{0}-{1}-{2}-{3}{4}", new object[]
                                    {
                                                    orderNo,
                                                    partIds,
                                                    attorney,
                                                    Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-"),
                                                    "." + filetype
                                    }).Replace(",", "-");
                                }
                                else
                                {
                                    outputFileName = string.Format("{0}-{1}-{2}{3}", new object[]
                                    {
                                                    orderNo,
                                                    partIds,
                                                    System.IO.Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-"),
                                                    "." + filetype
                                    }).Replace(",", "-");
                                }
                            }
                            if (!System.IO.Directory.Exists(System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo))))
                            {
                                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo)));
                            }
                            try
                            {
                                using (System.IO.FileStream file = new System.IO.FileStream(System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo), gid.ToString() + "." + filetype), System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                {
                                    ms.WriteTo(file);
                                    file.Close();

                                }
                            }
                            catch (System.Exception ex)
                            {
                                Log.ServicLog(ex.Message);
                                Log.ServicLog(ex.StackTrace.ToString());
                            }
                            finally
                            {

                                FileObject fileObject = new FileObject()
                                {
                                    FileName = outputFileName.Replace("_", "-"),
                                    FileType = Convert.ToInt32(drForm["FileTypeID"]),
                                    IsPublic = Convert.ToBoolean(drForm["IsPublic"]),
                                    OrderNo = orderNo,
                                    PartNo = 0,
                                    Pages = Convert.ToInt32(drForm["Pages"]),
                                    RecordType = Convert.ToInt32(drForm["RecordTypeID"]),
                                    UserId = new System.Guid(drForm["UserID"].ToString()),
                                };
                                string fileDiskName = gid.ToString() + "." + filetype;
                                Common.AddFilesToPart(fileObject, fileDiskName);
                            }
                            if (!System.IO.Directory.Exists(System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo), partIds)))
                            {
                                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo), partIds));
                            }
                            try
                            {
                                using (System.IO.FileStream file2 = new System.IO.FileStream(System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo), partIds, gid.ToString() + "." + filetype), System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                {
                                    ms.WriteTo(file2);
                                    file2.Close();
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Log.ServicLog(ex.Message);
                                Log.ServicLog(ex.StackTrace.ToString());

                            }
                            finally
                            {
                                msFile[counter] = ms;
                                msFile[counter].Position = 0L;
                                AttachFileName.Add(outputFileName);
                                FileObject fileObject = new FileObject()
                                {
                                    FileName = outputFileName.Replace("_", "-"),
                                    FileType = Convert.ToInt32(drForm["FileTypeID"]),
                                    IsPublic = Convert.ToBoolean(drForm["IsPublic"]),
                                    OrderNo = orderNo,
                                    PartNo = Convert.ToInt32(partIds),
                                    Pages = Convert.ToInt32(drForm["Pages"]),
                                    RecordType = Convert.ToInt32(drForm["RecordTypeID"]),
                                    UserId = new System.Guid(drForm["UserID"].ToString()),
                                };
                                string fileDiskName = gid.ToString() + "." + filetype;
                                Common.AddFilesToPart(fileObject, fileDiskName);
                                DbAccess.UpdateDocumentStatus(Convert.ToInt32(drForm["QuickFormID"].ToString()));
                            }
                        }
                        catch (Exception ex)
                        {
                            msFile[counter] = null;
                            AttachFileName.Add(null);
                            Log.ServicLog(ex.Message);
                            Log.ServicLog(ex.StackTrace.ToString());

                        }
                        counter++;
                    }

                    Log.ServicLog("Quickforms generation completed succeesfully.");
                }
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.Message);
                Log.ServicLog(ex.StackTrace);
            }
        }

        public static void GenerateDocumentsAttachment(int QuickFormID, ref List<string> AttachFileName, ref MemoryStream[] msFile, bool isFromClient)
        {
            try
            {
                var dsMain = DbAccess.GetQuickFormPrintformattachfiles(Convert.ToInt32(QuickFormID), false, true);
                if (dsMain != null && dsMain.Rows.Count > 0)
                {
                    int counter = AttachFileName.Count();
                    foreach (DataRow drForm in dsMain.Rows)
                    {
                        try
                        {
                            int orderNo = Convert.ToInt32(drForm["OrderNo"].ToString());
                            //int partIds = Convert.ToInt32(drForm["PartNo"].ToString());
                            string partIds = drForm["PartNo"].ToString();
                            string outputFileName = Convert.ToString(drForm["DocName"]);
                            string FileName = Convert.ToString(drForm["DocPath"]);
                            int UploadType = Convert.ToInt32(drForm["UploadType"].ToString());
                            System.IO.MemoryStream ms = new System.IO.MemoryStream();

                            string _finalPath = "";
                            string uploadRoot = "";

                            if (UploadType == 2)
                            {
                                uploadRoot = System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString());
                                _finalPath = string.Format(@"{0}{1}\{2}", uploadRoot, orderNo, FileName);
                             
                            }
                            else if (UploadType == 3)
                            {
                                uploadRoot = System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRootCase3"].ToString());
                                _finalPath = string.Format(@"{0}{1}", uploadRoot, FileName);
                            }
                           
                        
                            try
                            {
                                System.Guid gid = System.Guid.NewGuid();
                                if (isFromClient == true)
                                {
                                    FileObject fileObject = new FileObject()
                                    {
                                        FileName = Convert.ToString(drForm["DocName"]),
                                        FileType = Convert.ToInt32(drForm["FileTypeID"]),
                                        IsPublic = Convert.ToBoolean(drForm["IsPublic"]),
                                        OrderNo = orderNo,
                                        PartNo = Convert.ToInt32(drForm["PartNo"]),
                                        Pages = Convert.ToInt32(drForm["Pages"]),
                                        RecordType = Convert.ToInt32(drForm["RecordTypeID"]),
                                        UserId = new System.Guid(drForm["UserID"].ToString()),
                                    };
                                    string filetype = fileObject.FileName.Substring(fileObject.FileName.LastIndexOf('.') + 1);
                                    string fileDiskName = gid.ToString() + "." + filetype;
                                    Common.AddFilesToPart(fileObject, fileDiskName);
                                }
                                string filetypeAttach = _finalPath.Substring(_finalPath.LastIndexOf('.') + 1);
                                if (filetypeAttach.ToLower() == "doc" || filetypeAttach.ToLower() == "docx")
                                {
                                    MemoryStream msAttach = new MemoryStream();
                                    Aspose.Words.Document dc = new Document(_finalPath);
                                    dc.Save(msAttach, SaveFormat.Pdf);
                                    msAttach.Position = 0L;
                                    msFile[counter] = msAttach;
                                    AttachFileName.Add(outputFileName);

                                }
                                else
                                {
                                    using (System.IO.FileStream fstream = System.IO.File.OpenRead(_finalPath))
                                    {

                                        MemoryStream mstream = new MemoryStream();
                                        mstream.SetLength(fstream.Length);
                                        fstream.Read(mstream.GetBuffer(), 0, (int)fstream.Length);
                                        mstream.Position = 0L;
                                        msFile[counter] = mstream;
                                        AttachFileName.Add(outputFileName);

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.ServicLog(ex.Message);
                                Log.ServicLog(ex.StackTrace.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.ServicLog(ex.Message);
                            Log.ServicLog(ex.StackTrace.ToString());
                        }
                        counter++;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.Message);
                Log.ServicLog(ex.StackTrace.ToString());
            }
        }
    }
}
