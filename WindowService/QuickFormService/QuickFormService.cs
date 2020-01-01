using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Configuration;
using QuickFormService.Entity;
using System.IO;
using Aspose.Words;

namespace QuickFormService
{
    public partial class QuickFormService : ServiceBase
    {
        public static int IsRevised = 0; //For Duplicate doc change
        public static string IsRevisedText = ""; //For Duplicate doc change
        public static string BillFirm = "";
        public static string ClaimNo = "";
        public static int RecordType = 0;
        public static string AttyName = "";
        public static string AttyID = "";
        public static string OrdBy = "";
        public int tempCount = 0;

        Timer serviceTimer = null;


        public QuickFormService()
        {
            Log.ServicLog("======================================================================");
            Log.ServicLog("Initialize");
            Log.ServicLog("Duration Time :  " + ConfigurationManager.AppSettings["Duration"].ToString() + " min");
            InitializeComponent();

            serviceTimer = new Timer();
            serviceTimer.Elapsed += serviceTimer_Elapsed;
            serviceTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["Duration"].ToString()) * 60 * 1000;
            serviceTimer.Enabled = true;
            // serviceTimer.Start();

            InitializeComponent();
        }

        private void serviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log.ServicLog(DateTime.Now.ToString() + " TIMER ELAPSED");
            GenerateDocuments();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Log.ServicLog("Service Started");
                GenerateDocuments();
            }
            catch (Exception ex)
            {
                Log.ServicLog("------------ OnStart Log ------------");
                Log.ServicLog(ex.ToString());
            }
        }

        protected override void OnStop()
        {
            Log.ServicLog("Service Stopped");
        }

        public void GenerateDocuments()
        {
            Log.ServicLog("Generate Documents" + DateTime.Now.ToString());
            Log.ServicLog("---------- SERVICE EXECUTED " + DateTime.Now.ToString() + "---------------");
            string documentRoot = ConfigurationManager.AppSettings["DocumentRoot"];
            string companyLogoDirectory = ConfigurationManager.AppSettings["CompanyLogoDirectory"];
            serviceTimer.Stop();

            List<string> FileNotFound = new List<string>();
            try
            {

                var OrderClientList = DbAccess.GetQuickFormUniqueRecords();

                if (OrderClientList != null && OrderClientList.Count > 0)
                {
                    tempCount = 0;
                    foreach (var order in OrderClientList)
                    {
                        var dtQuery = new DataTable();
                        var dtSubQuery = new DataTable();

                        var dsMain = DbAccess.GetQuickFormOrderDetails(Convert.ToInt32(order));
                        if (dsMain != null && dsMain.Rows.Count > 0)
                        {
                            DataRow[] drTemp = dsMain.Select("isEmail=1 and email is not null and len(email) > 0");
                            DataRow[] dataRowTemp;
                            if (drTemp.Count() > 0)
                            {
                                foreach (DataRow datarow in drTemp)
                                {
                                    string quickformID = string.Empty;
                                    try
                                    {
                                        dataRowTemp = dsMain.Select("email='" + Convert.ToString(datarow["Email"]) + "'");
                                        if (dataRowTemp.Count() > 0)
                                        {
                                            foreach (DataRow dr in dataRowTemp)
                                            {
                                                quickformID += Convert.ToString(dr["QuickFormID"]) + ",";
                                                dsMain.Rows.Remove(dr);
                                            }
                                            CombinedEmail ce = new CombinedEmail();
                                            ce.GenerateDocuments(quickformID, true);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.ServicLog(ex.Message);
                                        Log.ServicLog(ex.StackTrace);
                                    }
                                }
                                if (dsMain.Rows.Count > 0)
                                {
                                    if (drTemp != null)
                                    {
                                        foreach (DataRow dr in drTemp)
                                        {
                                            string quickformID = string.Empty;
                                            DataRow[] drPrintForm;
                                            try
                                            {
                                                quickformID = Convert.ToString(dr["QuickFormID"]);
                                                var dsPrintForm = DbAccess.GetQuickFormPrintformattachfiles(Convert.ToInt32(quickformID), false, false);
                                                if (dsPrintForm != null && dsPrintForm.Rows.Count > 0)
                                                {
                                                    drPrintForm = dsMain.Select("QuickFormID=" + dr["QuickFormID"]);
                                                    foreach (DataRow drow in drPrintForm)
                                                        dsMain.Rows.Remove(drow);

                                                    CombinedEmail ce = new CombinedEmail();
                                                    ce.GenerateDocuments(quickformID, true);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.ServicLog(ex.Message);
                                                Log.ServicLog(ex.StackTrace);
                                            }
                                        }
                                    }

                                }

                            }
                        }
                        if (dsMain != null && dsMain.Rows.Count > 0)
                        {
                            string filetype = "pdf";
                            Log.ServicLog("Quick forms generating process started");
                            try
                            {

                                foreach (DataRow drForm in dsMain.Rows)
                                {
                                    try
                                    {
                                        bool displaySSN = Convert.ToBoolean(Convert.ToString(drForm["IsSSN"]) != "" ? Convert.ToString(drForm["IsSSN"]) : "true");
                                        string docName = string.Empty;
                                        if (drForm["IsRevised"].ToString().ToLower() == "revised" || drForm["IsRevised"].ToString().ToLower() == "duplicate") //For Duplicate doc change
                                        {
                                            IsRevised = 1;
                                            IsRevisedText = drForm["IsRevised"].ToString().Trim();
                                        }
                                        else
                                        {
                                            IsRevised = 0;
                                            IsRevisedText = "";
                                        }
                                        if (Convert.ToInt64(drForm["RecordTypeID"]) != 0)
                                            RecordType = Convert.ToInt16(drForm["RecordTypeID"]);
                                        else
                                            RecordType = 0;

                                        if (drForm["ClaimNo"].ToString().Trim() != "")
                                            ClaimNo = (drForm["ClaimNo"].ToString());
                                        else
                                            ClaimNo = "0";
                                        if (drForm["AttyName"].ToString().Trim() != "")
                                            AttyName = (drForm["AttyName"].ToString());
                                        else
                                            AttyName = "";

                                        if (drForm["BillFirm"].ToString().Trim() != "")
                                            BillFirm = drForm["BillFirm"].ToString();
                                        else
                                            BillFirm = "";
                                        try
                                        {
                                            if (drForm["DocPath"].ToString().Contains("Subpoenas"))
                                            {
                                                string[] dname;
                                                dname = drForm["DocName"].ToString().Split('_');
                                                docName = drForm["DocName"].ToString().Replace(dname[0] + "_" + dname[1] + "_", "");
                                            }
                                            else
                                            {
                                                if (drForm["DocName"].ToString().Split('_').Length == 6)
                                                    docName = drForm["DocName"].ToString().Split('_')[5].ToUpper();
                                                else if (drForm["DocName"].ToString().Split('_').Length == 3)
                                                    docName = drForm["DocName"].ToString().Split('_')[2].ToUpper();
                                                else if (drForm["DocName"].ToString().Split('_').Length == 4)
                                                    docName = drForm["DocName"].ToString().Split('_')[3].ToUpper();
                                                else
                                                    docName = drForm["DocName"].ToString().Split('_')[1].ToUpper();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.ServicLog("---------- DOOCUMENT NAME " + drForm["QuickformID"].ToString() + DateTime.Now.ToString() + "---------------");
                                            Log.ServicLog(ex.Message);
                                            Log.ServicLog(ex.StackTrace.ToString());
                                            Log.ServicLog("---------- DOOCUMENT NAME " + drForm["QuickformID"].ToString() + DateTime.Now.ToString() + "---------------");
                                            continue;
                                        }
                                        string[] folders = Convert.ToString(drForm["DocPath"]).Split('>');
                                        QueryType pdt = DbAccess.GetDocumentType(docName, folders[0].ToString());
                                        int orderNo = Convert.ToInt32(drForm["OrderNo"]);
                                        string partIds = Convert.ToString(drForm["PartNo"]).Replace('-',',').Trim(',');

                                        string subquery = string.Empty;

                                        MemoryStream ms = new MemoryStream();
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
                                            catch (Exception ex)
                                            {
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

                                        Aspose.Words.License license = new License();
                                        license.SetLicense("Aspose.Words.lic");
                                        string filePath = Path.Combine(documentRoot, drForm["DocPath"].ToString().Trim().Replace(">", "/"), docName.Trim());
                                        if (!File.Exists(filePath))
                                        {
                                            string OrderPart = Convert.ToString(drForm["OrderNo"]) + "-" + Convert.ToString(drForm["PartNo"]);
                                            FileNotFound.Add("Order No : <b>" + OrderPart + "</b>   " + filePath);
                                            continue;
                                        }



                                        #region Add Company Wise logo 

                                        Document doc;
                                        Log.ServicLog("3");
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

                                        // doc = Common.InsertHeaderLogo(filePath, string.Format("{0}logo-axiom_{1}.png", companyLogoDirectory, drForm["CompanyNo"]));


                                        #endregion

                                        // Here Barcode Generater Code                                        
                                        var FileTypeId2 = Convert.ToString(drForm["FileTypeID"]);

                                        doc.MailMerge.Execute(dtQuery);


                                        if (IsRevised == 1)
                                            Utility.InsertWatermarkText(doc, IsRevisedText);

                                        doc.Save(ms, SaveFormat.Pdf);
                                        if (RecordType != 0)
                                        {
                                            string TStamp = DateTime.Now.ToString("yyyyMMddHHmm");
                                            string _storageRoot = string.Empty;
                                            string FilePath = string.Empty;
                                            if (!string.IsNullOrEmpty(BillFirm) && BillFirm.ToUpper() == "GRANCO01")
                                            {
                                                _storageRoot = ConfigurationManager.AppSettings["GrangeRoot"].ToString();
                                                DirectoryInfo dis = new DirectoryInfo(_storageRoot);
                                                if (!dis.Exists)
                                                    dis.Create();
                                                FilePath = _storageRoot + string.Format("{0}-{1}-{2}-{3}", ClaimNo, TStamp, orderNo, partIds + ".pdf");

                                                int count = 1;
                                                string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                                string extension = Path.GetExtension(FilePath);
                                                string path = Path.GetDirectoryName(FilePath);
                                                string newFullPath = FilePath;

                                                while (File.Exists(newFullPath))
                                                {
                                                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                                    newFullPath = Path.Combine(path, tempFileName + extension);
                                                }

                                                FileStream file = new FileStream(newFullPath, FileMode.Create, FileAccess.Write);
                                                ms.WriteTo(file);
                                                file.Close();
                                            }
                                            else if (!string.IsNullOrEmpty(BillFirm) && BillFirm.ToUpper() == "HANOAA01")
                                            {
                                                _storageRoot = ConfigurationManager.AppSettings["HanoverRoot"].ToString();

                                                DirectoryInfo dis = new DirectoryInfo(_storageRoot);
                                                if (!dis.Exists)
                                                    dis.Create();

                                                FilePath = _storageRoot + string.Format("{0}_{1}_{2}_{3}-{4}", ClaimNo, AttyName, TStamp, orderNo, partIds + ".pdf");

                                                int count = 1;

                                                string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                                string extension = Path.GetExtension(FilePath);
                                                string path = Path.GetDirectoryName(FilePath);
                                                string newFullPath = FilePath;

                                                while (File.Exists(newFullPath))
                                                {
                                                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                                    newFullPath = Path.Combine(path, tempFileName + extension);
                                                }

                                                FileStream file = new FileStream(newFullPath, FileMode.Create, FileAccess.Write);
                                                ms.WriteTo(file);
                                                file.Close();
                                            }
                                        }
                                        string outputFileName = "printforms";
                                        outputFileName = Path.GetFileNameWithoutExtension(Convert.ToString(drForm["DocName"]).Replace(" ", "-") + "." + filetype);
                                        Guid gid = Guid.NewGuid();
                                        if (partIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length > 1 || (pdt == QueryType.AttorneyForms || pdt == QueryType.Confirmation || pdt == QueryType.Waiver))
                                        {
                                            if (pdt == QueryType.AttorneyForms)
                                            {
                                                string attorney = "";
                                                if (drForm["DocName"].ToString().Split('_').Length == 4)
                                                {
                                                    attorney = drForm["DocName"].ToString().Split('_')[2].ToUpper();
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
                                                    Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-"),
                                                    "." + filetype
                                                }).Replace(",", "-");
                                            }
                                            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo))))
                                            {
                                                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo)));
                                            }
                                            try
                                            {
                                                using (FileStream file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo), gid.ToString() + "." + filetype), System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                                {
                                                    ms.WriteTo(file);
                                                    file.Close();
                                                }
                                            }
                                            catch (System.Exception ex)
                                            {
                                                Log.ServicLog(ex.Message);
                                                Log.ServicLog(ex.StackTrace);
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
                                                DbAccess.UpdateDocumentStatus(Convert.ToInt32(drForm["QuickFormID"].ToString()));
                                            }
                                        }
                                        else
                                        {

                                            if (!Directory.Exists(System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo), partIds)))
                                            {
                                                Directory.CreateDirectory(System.IO.Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo), partIds));
                                            }
                                            try
                                            {
                                                using (FileStream file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(orderNo), partIds, gid.ToString() + "." + filetype), FileMode.Create, FileAccess.Write))
                                                {
                                                    ms.WriteTo(file);
                                                    file.Close();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.ServicLog(ex.Message);
                                                Log.ServicLog(ex.StackTrace);
                                            }
                                            finally
                                            {
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

                                        if (drForm["IsPrint"].ToString() == "1")
                                            Utility.PrintDocument(doc, Convert.ToInt32(drForm["QuickFormID"]));

                                        if (((drForm["IsEmail"].ToString() == "1") && !string.IsNullOrEmpty(Convert.ToString(drForm["Email"])) && drForm["Email"].ToString().Length > 0) ||
                                            (drForm["IsFax"].ToString() == "1") && !string.IsNullOrEmpty(Convert.ToString(drForm["FaxNo"])) && drForm["FaxNo"].ToString().Length > 0)
                                        {
                                            MemoryStream[] msFile = new MemoryStream[10];
                                            List<string> fileNames = new List<string>();

                                            AttachmentInsert.GenerateDocuments(Convert.ToInt32(drForm["QuickFormID"]), ref fileNames, ref msFile);
                                            AttachmentInsert.GenerateDocumentsAttachment(Convert.ToInt32(drForm["QuickFormID"]), ref fileNames, ref msFile, Convert.ToBoolean(drForm["isFromClient"]));

                                            var pdfDocName = Convert.ToString(drForm["DocName"]).Replace("doc", "pdf").Replace("DOC", "pdf");
                                            fileNames.Add(pdfDocName);
                                            if ((Convert.ToString(drForm["IsEmail"]) == "1") && !string.IsNullOrEmpty(Convert.ToString(drForm["Email"])))
                                            {
                                                EmailDetails ed = new EmailDetails();
                                                string acctrep = "";
                                                string OrderAttorney = string.Empty;
                                                var dtOrders = DbAccess.GetOrderDetail(orderNo);
                                                if (dtOrders != null && dtOrders.Rows.Count > 0)
                                                {
                                                    ed.Caption = dtOrders.Rows[0]["Caption"].ToString();
                                                    ed.CauseNumber = dtOrders.Rows[0]["CauseNo"].ToString();
                                                    ed.PatientName = dtOrders.Rows[0]["PatientName"].ToString();
                                                    OrderAttorney = dtOrders.Rows[0]["OrderingAttorney"].ToString();
                                                    acctrep = dtOrders.Rows[0]["AcctRep"].ToString();
                                                }
                                                var dtAccExecutive = DbAccess.GetAccntRepDetail(acctrep);
                                                if (dtAccExecutive != null && dtAccExecutive.Rows.Count > 0)
                                                {
                                                    ed.AccExeName = dtAccExecutive.Rows[0]["Name"].ToString();
                                                    ed.AccExeEmail = dtAccExecutive.Rows[0]["Email"].ToString();
                                                }
                                                else
                                                {
                                                    ed.AccExeName = "Josh Sanford";
                                                    ed.AccExeEmail = "Josh.Sanford@axiomcopy.com";
                                                }
                                                DataTable dtAttorney = new DataTable();
                                                string additionalEmail = string.Empty;
                                                if (pdt == QueryType.Confirmation)
                                                {
                                                    if (!string.IsNullOrEmpty(OrderAttorney))
                                                    {
                                                        dtAttorney = DbAccess.GetNotificationEmails(orderNo, OrderAttorney.Trim());
                                                    }
                                                }
                                                if (dtAttorney.Rows.Count > 0)
                                                {
                                                    foreach (DataRow dr in dtAttorney.Rows)
                                                        additionalEmail += dr["AssistantEmail"].ToString() + ",";
                                                }
                                                additionalEmail = additionalEmail.Trim(',');
                                                Utility.EmailDocument(doc, Convert.ToInt32(drForm["QuickFormID"]), fileNames, drForm["Email"].ToString(), msFile, ed, additionalEmail, IsRevised, IsRevisedText, false);
                                            }
                                            if ((drForm["IsFax"].ToString() == "1") && !string.IsNullOrEmpty(Convert.ToString(drForm["FaxNo"])))
                                                Utility.FaxDocument(doc, Convert.ToInt32(drForm["QuickFormID"]), fileNames, drForm["FaxNo"].ToString(), drForm["Name"].ToString(), msFile);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Log.ServicLog("---------------- QUICKFORMID : " + drForm["QuickFormID"].ToString() + " ---------------------");
                                        Log.ServicLog("Order Number : " + Convert.ToString(order));
                                        Log.ServicLog(ex.Message);
                                        Log.ServicLog(ex.StackTrace.ToString());
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
                Log.ServicLog("------QuickForm Service Completed-------");
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.Message);
                Log.ServicLog(ex.StackTrace.ToString());
                serviceTimer.Start();
            }
            serviceTimer.Start();
        }



        public void ProcessDocument()
        {

            List<string> result = new List<string>();
            result = DbAccess.GetQuickFormUniqueRecords();

            foreach (var item in result)
            {

            }
        }

        public void CombineDocuments(string QuickFormID, bool isMultiple)
        {

        }


    }

}
