using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UploadSweepService.Entity;

namespace UploadSweepService
{

    enum RecordGroupType
    {
        NewRecords = 5,
        AuthorizatioNeeded = 2,
        MoreInformation = 4,
        InPrgress = 1
    }


    public partial class UploadSweepService : ServiceBase
    {
        Timer serviceTimer = null;

        private Timer delay;
        private DirectoryInfo di;
        private FileInfo[] fi;
        private string orderPath;
        private string partPath;
        private string path;
        private string destFile;
        private string sourceFile;
        private string names;
        private string scheduleTime;
        private string currentTime;
        private string[] listnames;
        private int orderno;
        private int partno;
        private bool status;
        private DateTime dt;
        private char splitCharacter = char.Parse(ConfigurationManager.AppSettings["splitCharacter"]);
        private bool flag;
        public UploadSweepService()
        {
            Log.ServicLog("======================================================================");
            Log.ServicLog("Initialize");
            Log.ServicLog("Duration Time :  " + ConfigurationManager.AppSettings["Duration"].ToString() + " min");
            InitializeComponent();
            serviceTimer = new Timer();
            serviceTimer.Elapsed += serviceTimer_Elapsed;
            serviceTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["Duration"].ToString()) * 60 * 1000;
            serviceTimer.Enabled = true;
            serviceTimer.Start();

            InitializeComponent();
        }

        private void serviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log.ServicLog(DateTime.Now.ToString() + " TIMER ELAPSED");
            UploadOtherRecords();
            CreateOrder();
        }
        public void UploadOtherRecords()
        {
            int num;
            this.path = string.Empty;

            Log.ServicLog("============ Records To Be Uploaded Processed Started =============");
            this.path = ConfigurationManager.AppSettings["RecordsToBeUploaded"];
            try
            {
                this.di = new DirectoryInfo(this.path);
                if (!this.di.Exists)
                {
                    Log.ServicLog("Source file path " + this.path + " doesn't exists.");
                }
                else
                {
                    this.fi = this.di.GetFiles();
                    FileInfo[] fileInfoArray = this.fi;

                    for (int i = 0; i < fileInfoArray.Length; i++)
                    {
                        try
                        {

                            FileInfo fileInfo = fileInfoArray[i];
                            this.sourceFile = string.Concat(this.path, "\\", fileInfo);
                            this.names = fileInfo.Name;
                            string[] strArrays = this.names.Split(new char[0]);


                            if ((int)strArrays.Length < 1)
                            {
                                this.ErrorFiles(fileInfo);
                                this.flag = true;
                            }
                            else
                            {

                                if (strArrays[0].Contains("."))
                                {
                                    strArrays[0] = strArrays[0].Remove(strArrays[0].IndexOf('.'));
                                }
                                if (strArrays[0].Trim().Length <= 0)
                                {
                                    this.ErrorFiles(fileInfo);
                                    this.flag = true;
                                }
                                if (!this.flag)
                                {
                                    string str = strArrays[0];
                                    char[] chrArray = new char[] { this.splitCharacter };
                                    this.listnames = str.Split(chrArray);
                                    if ((int)this.listnames.Length < 2)
                                    {
                                        this.ErrorFiles(fileInfo);
                                        this.flag = true;
                                    }
                                    else
                                    {
                                        string[] strArrays1 = this.listnames;
                                        for (int j = 0; j < (int)strArrays1.Length; j++)
                                        {
                                            if (strArrays1[j].Trim().Length <= 0)
                                            {
                                                this.ErrorFiles(fileInfo);
                                                this.flag = true;
                                            }
                                        }
                                        if (!this.flag)
                                        {
                                            if (!int.TryParse(this.listnames[0], out num))
                                            {
                                                this.ErrorFiles(fileInfo);
                                                this.flag = true;
                                            }
                                            if (!this.flag)
                                            {
                                                for (int k = 1; k < (int)this.listnames.Length; k++)
                                                {
                                                    if (!int.TryParse(this.listnames[k], out num))
                                                    {
                                                        this.ErrorFiles(fileInfo);
                                                        this.flag = true;
                                                    }
                                                    if (!this.flag)
                                                    {
                                                        // HANOVER AND GRANGE CHANGE
                                                        this.RenameFile(Convert.ToString(fileInfo.FullName), Convert.ToInt32(this.listnames[0]), this.listnames[k]);
                                                        UploadOtherRecords(fileInfo, this.listnames[0], this.listnames[k]);
                                                        //  this.SendMailOnUploadDocument(Convert.ToInt32(this.listnames[0]), Convert.ToInt32(this.listnames[k]));
                                                    }
                                                }
                                                this.ProcessedFiles(fileInfo, this.names);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.ServicLog(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            Log.ServicLog("============ Records To Be Uploaded Processed Finished =============");
        }
        private void ErrorFiles(FileInfo str)
        {
            string item = ConfigurationManager.AppSettings["NotProcessed"];
            string str1 = DateTime.Now.ToString("dd-MM-yyyy");
            item = string.Concat(item, "\\", str1);
            this.di = new DirectoryInfo(item);
            if (this.di.Exists)
            {
                this.destFile = string.Concat(item, "\\", str);
                if (File.Exists(this.destFile))
                {
                    File.Delete(this.destFile);
                    File.Move(this.sourceFile, this.destFile);
                }
                else
                {
                    File.Move(this.sourceFile, this.destFile);
                }
            }
            else
            {
                this.di.Create();
                this.destFile = string.Concat(item, "\\", str);
                if (File.Exists(this.destFile))
                {
                    File.Delete(this.destFile);
                    File.Move(this.sourceFile, this.destFile);
                }
                else
                {
                    File.Move(this.sourceFile, this.destFile);
                }
            }
            Log.ServicLog(string.Concat(str, " file format is not in a specified format"));
        }
        public void RenameFile(string fileInfo, int orderNo, string PartNo)
        {
            try
            {
                if (orderNo > 0)
                {
                    string TStamp = DateTime.Now.ToString("yyyyMMddHHmm");
                    string _storageRoot = string.Empty;
                    string FilePath = string.Empty;
                    string BillFirm = string.Empty;
                    string ClaimNo = string.Empty;
                    string AttyName = string.Empty;
                    string AttyID = string.Empty;
                    DataTable dtresult = new DataTable();
                    dtresult = DbAccess.GetDataList("ServiceSweepGetAttorneyByOrder", orderNo);
                    if (dtresult != null)
                    {
                        BillFirm = Convert.ToString(dtresult.Rows[0]["BillFirm"]);
                        ClaimNo = Convert.ToString(dtresult.Rows[0]["ClaimNo"]);
                        AttyName = Convert.ToString(dtresult.Rows[0]["AttorneyName"]);
                    }
                    if (!string.IsNullOrEmpty(BillFirm))
                    {
                        if (BillFirm == "GRANCO01")
                        {

                            MemoryStream ms = new MemoryStream();
                            _storageRoot = ConfigurationManager.AppSettings["GrangeRoot"].ToString();
                            DirectoryInfo dis = new DirectoryInfo(_storageRoot);
                            if (!dis.Exists)
                            {
                                dis.Create();
                            }
                            try
                            {
                                using (FileStream fsSource = new FileStream(fileInfo, FileMode.Open, FileAccess.Read))
                                {
                                    byte[] bytes = new byte[fsSource.Length];
                                    int numBytesToRead = (int)fsSource.Length;
                                    int numBytesRead = 0;
                                    while (numBytesToRead > 0)
                                    {
                                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);
                                        if (n == 0)
                                            break;
                                        numBytesRead += n;
                                        numBytesToRead -= n;
                                    }
                                    numBytesToRead = bytes.Length;
                                    FilePath = _storageRoot + string.Format("{0}-{1}-{2}-{3}", ClaimNo, TStamp, orderNo, PartNo + ".pdf");

                                    int count = 1;
                                    string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                    string extension = Path.GetExtension(FilePath);
                                    string path = Path.GetDirectoryName(FilePath);
                                    string newFullPath = FilePath;

                                    while (System.IO.File.Exists(newFullPath))
                                    {
                                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                        newFullPath = Path.Combine(path, tempFileName + extension);
                                    }

                                    using (FileStream fileGrangeRoot = new FileStream(newFullPath, FileMode.Create, FileAccess.Write))
                                    {
                                        fileGrangeRoot.Write(bytes, 0, numBytesToRead);
                                        fileGrangeRoot.Close();
                                    }
                                }
                            }
                            catch (Exception ex1)
                            {
                                Log.ServicLog("=============== GRANCO01 ===================");
                                Log.ServicLog(ex1.ToString());
                                Log.ServicLog("===============================================");
                            }
                        }

                        if (BillFirm == "HANOAA01")
                        {
                            System.IO.MemoryStream ms = new System.IO.MemoryStream();
                            _storageRoot = ConfigurationManager.AppSettings["HanoverRoot"].ToString();
                            System.IO.DirectoryInfo dis = new System.IO.DirectoryInfo(_storageRoot);
                            if (!dis.Exists)
                            {
                                dis.Create();
                            }
                            try
                            {
                                using (FileStream fsSource = new FileStream(fileInfo, FileMode.Open, FileAccess.Read))
                                {
                                    byte[] bytes = new byte[fsSource.Length];
                                    int numBytesToRead = (int)fsSource.Length;
                                    int numBytesRead = 0;
                                    while (numBytesToRead > 0)
                                    {
                                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);
                                        if (n == 0)
                                            break;
                                        numBytesRead += n;
                                        numBytesToRead -= n;
                                    }
                                    numBytesToRead = bytes.Length;
                                    FilePath = _storageRoot + string.Format("{0}_{1}_{2}_{3}-{4}", ClaimNo, AttyName, TStamp, orderNo, PartNo + ".pdf");

                                    int count = 1;
                                    string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                    string extension = Path.GetExtension(FilePath);
                                    string path = Path.GetDirectoryName(FilePath);
                                    string newFullPath = FilePath;

                                    while (System.IO.File.Exists(newFullPath))
                                    {
                                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                        newFullPath = Path.Combine(path, tempFileName + extension);
                                    }

                                    using (FileStream fileHanoverRoot = new FileStream(newFullPath, FileMode.Create, FileAccess.Write))
                                    {
                                        fileHanoverRoot.Write(bytes, 0, numBytesToRead);
                                        fileHanoverRoot.Close();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.ServicLog("=============== HANOAA01 ===================");
                                Log.ServicLog(ex.ToString());
                                Log.ServicLog("===============================================");
                            }
                        }


                    }

                }
            }
            catch (Exception ex)
            {

            }

        }
        public void UploadOtherRecords(FileInfo file, string OrderNo, string PartNo)
        {
            string item = ConfigurationManager.AppSettings["DocumentFolder"];
            string str = ConfigurationManager.AppSettings["Processed"];
            this.di = new DirectoryInfo(item);
            if (!this.di.Exists)
            {
                this.di.Create();
            }
            this.orderPath = string.Concat(item, "\\", OrderNo);
            Log.ServicLog(this.orderPath);
            this.di = new DirectoryInfo(this.orderPath);
            if (this.di.Exists)
            {
                this.di = new DirectoryInfo(this.orderPath);
            }
            else
            {
                this.di.Create();
            }
            this.orderno = Convert.ToInt32(OrderNo);
            this.partno = Convert.ToInt32(PartNo);
            this.status = true;
            this.dt = DateTime.Now;
            Guid guid = Guid.NewGuid();
            string str1 = string.Concat(guid.ToString(), file.Extension);
            this.FileToPart(file, PartNo, str1);
            DataTable dt = new DataTable();

            // NEW CHANGE FOR SWEEP PROGRAM FOR ATTACHING UPLOADED FILES WITH RECORDS WHICH DONT HAVE ANY FILE UPLOADED
            int FileTypeID = 18;
            int RecordType = 0;
            int rcvdid = 0;
            int Pages = 0;
            dt = DbAccess.GetLastUploadedRecord("ServiceSweepGetLastUploadedRecord", orderno, partno);
            DataTable dtAssistantContact = new DataTable();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        FileTypeID = 11; // THIS IS FILE TYPE FOR Records
                        RecordType = Convert.ToInt32(dt.Rows[0]["RecType"]);
                        rcvdid = Convert.ToInt32(dt.Rows[0]["RcvdId"]);
                        Pages = Convert.ToInt32(dt.Rows[0]["Pages"]);
                        
                        var result = DbAccess.GetAssistContactEmailList("GetAssistContactEmailList", orderno, partno, FileTypeID, RecordType);
                        List<CompanyDetailForEmailEntity> objCompany = DbAccess.CompanyDetailForEmail("CompanyDetailForEmailByOrderNo", orderno);
                        

                        string template = AppDomain.CurrentDomain.BaseDirectory + "MailTemplate\\BillingRecords.html";
                        System.Text.StringBuilder body = new System.Text.StringBuilder();
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(template))
                        {
                            body.Append(reader.ReadToEnd());
                        }
                        string subject = "Your Records Are Available " + Convert.ToString(orderno) + "-" + Convert.ToString(partno);

                        foreach (AssistContactEmail itemEmail in result.Where(x => x.NewRecordAvailable && !string.IsNullOrEmpty(x.AssistantEmail)))
                        {
                            try
                            {
                                body = body.Replace("{UserName}", "Hello " + itemEmail.AssistantName + ",");
                                body = body.Replace("{LOCATION}", itemEmail.LocationName + " (" + itemEmail.LocID + ")");
                                body = body.Replace("{PATIENT}", itemEmail.PatientName);
                                body = body.Replace("{CLAIMNO}", itemEmail.BillingClaimNo);
                                body = body.Replace("{ORDERNO}", Convert.ToString(orderno) + "-" + Convert.ToString(partno));
                                body = body.Replace("{InvHdr}", itemEmail.InvHdr);
                                body = body.Replace("{Pages}", Convert.ToString(Pages));
                                body = body.Replace("{LINK}", Convert.ToString(objCompany[0].SiteURL) + "/PartDetail?OrderId=" + Convert.ToString(orderno) + "&PartNo=" + Convert.ToString(partno));
                                body = body.Replace("{LogoURL}", objCompany[0].LogoPath);
                                body = body.Replace("{ThankYou}", objCompany[0].ThankYouMessage);
                                body = body.Replace("{CompanyName}", objCompany[0].CompName);
                                

                                EmailHelper.SendMail(itemEmail.AssistantEmail, body.ToString(), subject, "", "autharchive@axiomcopy.com,tejaspadia@gmail.com");
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }

            FileObject fileObject = new FileObject()
            {
                FileName = file.Name,
                FileType = FileTypeID, // dbo.FileTypes = 18 WHICH IS "OTHER"
                IsPublic = true,
                OrderNo = this.orderno,
                PartNo = this.partno,
                RecordType = RecordType,
                Pages = Pages,
                UserId = new Guid("507D72AE-1E1F-4FC0-AEED-3962FF1DCEC8")
            };
            this.AddFilesToPart(fileObject, str1);
            try
            {
                DbAccess.ServiceSweepUpdateRcvdProcess("ServiceSweepUpdateRcvdProcess", rcvdid);
            }
            catch (Exception ex)
            {

            }

            Log.ServicLog(string.Concat(file, " Processed Successfully"));

        }
        private void ProcessedFiles(FileInfo str, string filename)
        {
            string item = ConfigurationManager.AppSettings["Processed"];
            string str1 = DateTime.Now.ToString("dd-MM-yyyy");
            string str2 = string.Concat(item, "\\", str1);
            this.di = new DirectoryInfo(str2);
            if (!this.di.Exists)
            {
                this.di.Create();
                this.destFile = string.Concat(str2, "\\", str);
                this.InsertData(this.orderno, this.partno, filename, this.status, this.dt);
                File.Move(this.sourceFile, this.destFile);
                return;
            }
            this.destFile = string.Concat(str2, "\\", str);
            if (File.Exists(this.destFile))
            {
                //   this.UpdateData(this.orderno);
                File.Delete(this.destFile);
                File.Move(this.sourceFile, this.destFile);
                return;
            }
            //this.InsertData(this.orderno, this.partno, filename, this.status, this.dt);
            File.Move(this.sourceFile, this.destFile);
        }
        private void FileToPart(FileInfo str, string PartNo, string fileName)
        {
            this.partPath = string.Concat(this.orderPath, "\\", PartNo);
            this.di = new DirectoryInfo(this.partPath);
            if (!this.di.Exists)
            {
                this.di.Create();
                this.destFile = string.Concat(this.partPath, "\\", fileName);
                File.Copy(this.sourceFile, this.destFile);
                return;
            }
            this.destFile = string.Concat(this.partPath, "\\", fileName);
            if (!File.Exists(this.destFile))
            {
                File.Copy(this.sourceFile, this.destFile);
                return;
            }
            File.Delete(this.destFile);
            File.Copy(this.sourceFile, this.destFile);
        }
        private void AddFilesToPart(FileObject fileObj, string fileDiskName)
        {
            try
            {
                var obj = new FilesToPartEntity();
                obj.OrderNo = fileObj.OrderNo;
                obj.PartNo = fileObj.PartNo;
                obj.FileName = fileObj.FileName;
                obj.FileTypeId = fileObj.FileType;
                obj.RecordTypeId = fileObj.RecordType;
                obj.UserId = fileObj.UserId;
                obj.IsPublic = fileObj.IsPublic;
                obj.Pages = fileObj.Pages;
                obj.FileDiskName = fileDiskName;
                DbAccess.AddFilesToPart(obj, "ServiceSweepAddFilesToPart");
            }
            catch (Exception exception)
            {
                Log.ServicLog(exception.Message);
            }
        }
        private void InsertData(int orderno, int partno, string documentname, bool status, DateTime createddate)
        {
            //try
            //{
            //    saveStatusDataContext _saveStatusDataContext = new saveStatusDataContext();
            //    UploadStatus uploadStatu = new UploadStatus()
            //    {
            //        Orderno = orderno,
            //        Partno = new int?(partno),
            //        Status = status,
            //        Documentname = documentname,
            //        ProcessedDate = new DateTime?(createddate)
            //    };
            //    _saveStatusDataContext.UploadStatus.InsertOnSubmit(uploadStatu);
            //    _saveStatusDataContext.SubmitChanges();
            //}
            //catch (Exception exception)
            //{
            //    Log.ServicLog(exception.ToString());
            //}
        }


        public void CreateOrder()
        {
            int num;
            int num1;

            Log.ServicLog("Processed files information");
            Log.ServicLog("------------------------");
            this.path = ConfigurationManager.AppSettings["FilesToBeUploaded"];
            try
            {
                this.di = new DirectoryInfo(this.path);
                if (!this.di.Exists)
                {
                    Log.ServicLog("Source file path doesn't exists.");
                }
                else
                {
                    this.fi = this.di.GetFiles();
                    FileInfo[] fileInfoArray = this.fi;
                    for (int i = 0; i < (int)fileInfoArray.Length; i++)
                    {
                        try
                        {

                            FileInfo fileInfo = fileInfoArray[i];
                            this.flag = false;
                            this.sourceFile = string.Concat(this.path, "\\", fileInfo);
                            this.names = fileInfo.Name.Trim();
                            if (this.names.ToUpper().IndexOf("AUTHORIZATION") == -1 && this.names.ToUpper().IndexOf("NRR") == -1 && this.names.ToUpper().IndexOf("FEE") == -1 && this.names.ToUpper().IndexOf("LOCATION") == -1 && this.names.ToUpper().IndexOf("ROS") == -1)
                            {
                                this.ErrorFiles(fileInfo);
                                this.flag = true;
                            }
                            else if (this.names.ToUpper().IndexOf("AUTHORIZATION") != -1)
                            {
                                string[] strArrays = this.names.Split(new char[0]);
                                if ((int)strArrays.Length < 2)
                                {
                                    this.ErrorFiles(fileInfo);
                                    this.flag = true;
                                }
                                else
                                {
                                    if (strArrays[0].Trim().Length <= 0)
                                    {
                                        this.ErrorFiles(fileInfo);
                                        this.flag = true;
                                    }
                                    if (!this.flag)
                                    {
                                        string str = strArrays[0];
                                        char[] chrArray = new char[] { this.splitCharacter };
                                        this.listnames = str.Split(chrArray);
                                        if ((int)this.listnames.Length < 2)
                                        {
                                            this.ErrorFiles(fileInfo);
                                            this.flag = true;
                                        }
                                        else
                                        {
                                            string[] strArrays1 = this.listnames;
                                            for (int j = 0; j < (int)strArrays1.Length; j++)
                                            {
                                                if (strArrays1[j].Trim().Length <= 0)
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                            }
                                            if (!this.flag)
                                            {
                                                if (!int.TryParse(this.listnames[0], out num))
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                                if (!this.flag)
                                                {
                                                    for (int k = 1; k < (int)this.listnames.Length; k++)
                                                    {
                                                        if (!int.TryParse(this.listnames[k], out num))
                                                        {
                                                            this.ErrorFiles(fileInfo);
                                                            this.flag = true;
                                                        }
                                                        if (!this.flag)
                                                        {
                                                            this.UploadFilesToPart(fileInfo, this.listnames[0], this.listnames[k]);
                                                        }
                                                    }
                                                    this.ProcessedFiles(fileInfo, this.names);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (this.names.ToUpper().IndexOf("NRR") != -1)
                            {
                                string[] strArrays2 = this.names.Split(new char[0]);
                                if ((int)strArrays2.Length < 2)
                                {
                                    this.ErrorFiles(fileInfo);
                                    this.flag = true;
                                }
                                else
                                {
                                    if (strArrays2[0].Trim().Length <= 0)
                                    {
                                        this.ErrorFiles(fileInfo);
                                        this.flag = true;
                                    }
                                    if (!this.flag)
                                    {
                                        string str1 = strArrays2[0];
                                        char[] chrArray1 = new char[] { this.splitCharacter };
                                        this.listnames = str1.Split(chrArray1);
                                        if ((int)this.listnames.Length < 1)
                                        {
                                            this.ErrorFiles(fileInfo);
                                            this.flag = true;
                                        }
                                        else
                                        {
                                            string[] strArrays3 = this.listnames;
                                            for (int l = 0; l < (int)strArrays3.Length; l++)
                                            {
                                                if (strArrays3[l].Trim().Length < 0)
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                            }
                                            if (!this.flag)
                                            {
                                                if (!int.TryParse(this.listnames[0], out num1))
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                                if (!this.flag)
                                                {
                                                    this.UploadFilesToOrder(fileInfo, this.listnames[0]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // TNP : UPLOAD FEE DOCUMENTS FROM UploadFeesDocument FOLDER
                            else if (this.names.ToUpper().IndexOf("FEE") != -1)
                            {
                                string[] strArrays = this.names.Split(new char[0]);
                                if ((int)strArrays.Length < 2)
                                {
                                    this.ErrorFiles(fileInfo);
                                    this.flag = true;
                                }
                                else
                                {
                                    if (strArrays[0].Trim().Length <= 0)
                                    {
                                        this.ErrorFiles(fileInfo);
                                        this.flag = true;
                                    }
                                    if (!this.flag)
                                    {
                                        string str = strArrays[0];
                                        char[] chrArray = new char[] { this.splitCharacter };
                                        this.listnames = str.Split(chrArray);
                                        if ((int)this.listnames.Length < 2)
                                        {
                                            this.ErrorFiles(fileInfo);
                                            this.flag = true;
                                        }
                                        else
                                        {
                                            string[] strArrays1 = this.listnames;
                                            for (int j = 0; j < (int)strArrays1.Length; j++)
                                            {
                                                if (strArrays1[j].Trim().Length <= 0)
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                            }
                                            if (!this.flag)
                                            {
                                                if (!int.TryParse(this.listnames[0], out num))
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                                if (!this.flag)
                                                {
                                                    for (int k = 1; k < (int)this.listnames.Length; k++)
                                                    {
                                                        if (!int.TryParse(this.listnames[k], out num))
                                                        {
                                                            this.ErrorFiles(fileInfo);
                                                            this.flag = true;
                                                        }
                                                        if (!this.flag)
                                                        {
                                                            this.UploadFeesDocument(fileInfo, this.listnames[0], this.listnames[k]);
                                                        }
                                                    }
                                                    this.ProcessedFiles(fileInfo, this.names);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // TNP : UPLOAD LOCATION CORRESPONDENCE DOCUMENTS FROM UploadFeesDocument FOLDER
                            else if (this.names.ToUpper().IndexOf("LOCATION") != -1)
                            {
                                string[] strArrays = this.names.Split(new char[0]);
                                if ((int)strArrays.Length < 2)
                                {
                                    this.ErrorFiles(fileInfo);
                                    this.flag = true;
                                }
                                else
                                {
                                    if (strArrays[0].Trim().Length <= 0)
                                    {
                                        this.ErrorFiles(fileInfo);
                                        this.flag = true;
                                    }
                                    if (!this.flag)
                                    {
                                        string str = strArrays[0];
                                        char[] chrArray = new char[] { this.splitCharacter };
                                        this.listnames = str.Split(chrArray);
                                        if ((int)this.listnames.Length < 2)
                                        {
                                            this.ErrorFiles(fileInfo);
                                            this.flag = true;
                                        }
                                        else
                                        {
                                            string[] strArrays1 = this.listnames;
                                            for (int j = 0; j < (int)strArrays1.Length; j++)
                                            {
                                                if (strArrays1[j].Trim().Length <= 0)
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                            }
                                            if (!this.flag)
                                            {
                                                if (!int.TryParse(this.listnames[0], out num))
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                                if (!this.flag)
                                                {
                                                    for (int k = 1; k < (int)this.listnames.Length; k++)
                                                    {
                                                        if (!int.TryParse(this.listnames[k], out num))
                                                        {
                                                            this.ErrorFiles(fileInfo);
                                                            this.flag = true;
                                                        }
                                                        if (!this.flag)
                                                        {
                                                            this.UploadLocationCorrDocument(fileInfo, this.listnames[0], this.listnames[k]);
                                                        }
                                                    }
                                                    this.ProcessedFiles(fileInfo, this.names);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // TNP : UPLOAD RETURN OF SERVICE CORRESPONDENCE DOCUMENTS FROM UploadFeesDocument FOLDER
                            else if (this.names.ToUpper().IndexOf("ROS") != -1)
                            {
                                string[] strArrays = this.names.Split(new char[0]);
                                if ((int)strArrays.Length < 2)
                                {
                                    this.ErrorFiles(fileInfo);
                                    this.flag = true;
                                }
                                else
                                {
                                    if (strArrays[0].Trim().Length <= 0)
                                    {
                                        this.ErrorFiles(fileInfo);
                                        this.flag = true;
                                    }
                                    if (!this.flag)
                                    {
                                        string str = strArrays[0];
                                        char[] chrArray = new char[] { this.splitCharacter };
                                        this.listnames = str.Split(chrArray);
                                        if ((int)this.listnames.Length < 2)
                                        {
                                            this.ErrorFiles(fileInfo);
                                            this.flag = true;
                                        }
                                        else
                                        {
                                            string[] strArrays1 = this.listnames;
                                            for (int j = 0; j < (int)strArrays1.Length; j++)
                                            {
                                                if (strArrays1[j].Trim().Length <= 0)
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                            }
                                            if (!this.flag)
                                            {
                                                if (!int.TryParse(this.listnames[0], out num))
                                                {
                                                    this.ErrorFiles(fileInfo);
                                                    this.flag = true;
                                                }
                                                if (!this.flag)
                                                {
                                                    for (int k = 1; k < (int)this.listnames.Length; k++)
                                                    {
                                                        if (!int.TryParse(this.listnames[k], out num))
                                                        {
                                                            this.ErrorFiles(fileInfo);
                                                            this.flag = true;
                                                        }
                                                        if (!this.flag)
                                                        {
                                                            this.UploadROSDocument(fileInfo, this.listnames[0], this.listnames[k]);
                                                        }
                                                    }
                                                    this.ProcessedFiles(fileInfo, this.names);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.ServicLog(ex);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                Log.ServicLog(exception);
            }
            this.CreateMail();
        }
        public void UploadFilesToPart(FileInfo file, string OrderNo, string PartNo)
        {
            string item = ConfigurationManager.AppSettings["DocumentFolder"];
            string str = ConfigurationManager.AppSettings["Processed"];
            this.di = new DirectoryInfo(item);
            if (!this.di.Exists)
            {
                this.di.Create();
            }
            this.orderPath = string.Concat(item, "\\", OrderNo);
            Log.ServicLog(this.orderPath);
            this.di = new DirectoryInfo(this.orderPath);
            if (this.di.Exists)
            {
                this.di = new DirectoryInfo(this.orderPath);
            }
            else
            {
                this.di.Create();
            }
            this.orderno = Convert.ToInt32(OrderNo);
            this.partno = Convert.ToInt32(PartNo);
            this.status = true;
            this.dt = DateTime.Now;
            Guid guid = Guid.NewGuid();
            string str1 = string.Concat(guid.ToString(), file.Extension);
            this.FileToPart(file, PartNo, str1);
            FileObject fileObject = new FileObject()
            {
                FileName = file.Name,
                FileType = 2,
                IsPublic = true,
                OrderNo = this.orderno,
                PartNo = this.partno,
                RecordType = 0,
                UserId = new Guid("507D72AE-1E1F-4FC0-AEED-3962FF1DCEC8")
            };
            this.AddFilesToPart(fileObject, str1);
            Log.ServicLog(string.Concat(file, " Processed Successfully"));
        }
        public void UploadLocationCorrDocument(FileInfo file, string OrderNo, string PartNo)
        {
            string item = ConfigurationManager.AppSettings["DocumentFolder"];
            string str = ConfigurationManager.AppSettings["Processed"];
            this.di = new DirectoryInfo(item);
            if (!this.di.Exists)
            {
                this.di.Create();
            }
            this.orderPath = string.Concat(item, "\\", OrderNo);
            Log.ServicLog(this.orderPath);
            this.di = new DirectoryInfo(this.orderPath);
            if (this.di.Exists)
            {
                this.di = new DirectoryInfo(this.orderPath);
            }
            else
            {
                this.di.Create();
            }
            this.orderno = Convert.ToInt32(OrderNo);
            this.partno = Convert.ToInt32(PartNo);
            this.status = true;
            this.dt = DateTime.Now;
            Guid guid = Guid.NewGuid();
            string str1 = string.Concat(guid.ToString(), file.Extension);
            this.FileToPart(file, PartNo, str1);
            FileObject fileObject = new FileObject()
            {
                FileName = file.Name,
                FileType = 15,
                IsPublic = false,
                OrderNo = this.orderno,
                PartNo = this.partno,
                RecordType = 0,
                UserId = new Guid("507D72AE-1E1F-4FC0-AEED-3962FF1DCEC8")
            };
            this.AddFilesToPart(fileObject, str1);
            Log.ServicLog(string.Concat(file, " Processed Successfully"));
        }
        public void UploadROSDocument(FileInfo file, string OrderNo, string PartNo)
        {
            string item = ConfigurationManager.AppSettings["DocumentFolder"];
            string str = ConfigurationManager.AppSettings["Processed"];
            this.di = new DirectoryInfo(item);
            if (!this.di.Exists)
            {
                this.di.Create();
            }
            this.orderPath = string.Concat(item, "\\", OrderNo);
            Log.ServicLog(this.orderPath);
            this.di = new DirectoryInfo(this.orderPath);
            if (this.di.Exists)
            {
                this.di = new DirectoryInfo(this.orderPath);
            }
            else
            {
                this.di.Create();
            }
            this.orderno = Convert.ToInt32(OrderNo);
            this.partno = Convert.ToInt32(PartNo);
            this.status = true;
            this.dt = DateTime.Now;
            Guid guid = Guid.NewGuid();
            string str1 = string.Concat(guid.ToString(), file.Extension);
            this.FileToPart(file, PartNo, str1);
            FileObject fileObject = new FileObject()
            {
                FileName = file.Name,
                FileType = 36,
                IsPublic = true,
                OrderNo = this.orderno,
                PartNo = this.partno,
                RecordType = 0,
                UserId = new Guid("507D72AE-1E1F-4FC0-AEED-3962FF1DCEC8")
            };
            this.AddFilesToPart(fileObject, str1);
            Log.ServicLog(string.Concat(file, " Processed Successfully"));
        }
        // UPLOAD FEES FROM FilesToBeUploaded FOLDER
        public void UploadFeesDocument(FileInfo file, string OrderNo, string PartNo)
        {
            string item = ConfigurationManager.AppSettings["DocumentFolder"];
            string str = ConfigurationManager.AppSettings["Processed"];
            this.di = new DirectoryInfo(item);
            if (!this.di.Exists)
            {
                this.di.Create();
            }
            this.orderPath = string.Concat(item, "\\", OrderNo);
            Log.ServicLog(this.orderPath);
            this.di = new DirectoryInfo(this.orderPath);
            if (this.di.Exists)
            {
                this.di = new DirectoryInfo(this.orderPath);
            }
            else
            {
                this.di.Create();
            }
            this.orderno = Convert.ToInt32(OrderNo);
            this.partno = Convert.ToInt32(PartNo);
            this.status = true;
            this.dt = DateTime.Now;
            Guid guid = Guid.NewGuid();
            string str1 = string.Concat(guid.ToString(), file.Extension);
            this.FileToPart(file, PartNo, str1);
            FileObject fileObject = new FileObject()
            {
                FileName = file.Name,
                FileType = 16,
                IsPublic = false,
                OrderNo = this.orderno,
                PartNo = this.partno,
                RecordType = 0,
                UserId = new Guid("507D72AE-1E1F-4FC0-AEED-3962FF1DCEC8")
            };
            this.AddFilesToPart(fileObject, str1);
            Log.ServicLog(string.Concat(file, " Processed Successfully"));
        }
        public void UploadFilesToOrder(FileInfo file, string OrderNo)
        {
            string item = ConfigurationManager.AppSettings["DocumentFolder"];
            string str = ConfigurationManager.AppSettings["Processed"];
            this.di = new DirectoryInfo(item);
            if (!this.di.Exists)
            {
                this.di.Create();
            }
            this.orderPath = string.Concat(item, "\\", OrderNo);
            Log.ServicLog(this.orderPath);
            this.di = new DirectoryInfo(this.orderPath);
            if (this.di.Exists)
            {
                this.di = new DirectoryInfo(this.orderPath);
            }
            else
            {
                this.di.Create();
            }
            this.orderno = Convert.ToInt32(OrderNo);
            this.partno = 0;
            this.status = true;
            this.dt = DateTime.Now;
            Guid guid = Guid.NewGuid();
            string str1 = string.Concat(guid.ToString(), file.Extension);
            this.FileToOrder(file, str1);
            FileObject fileObject = new FileObject()
            {
                FileName = file.Name,
                FileType = 30,
                IsPublic = false,
                OrderNo = this.orderno,
                PartNo = 0,
                RecordType = 0,
                UserId = new Guid("507D72AE-1E1F-4FC0-AEED-3962FF1DCEC8")
            };
            this.AddFilesToPart(fileObject, str1);
            this.ProcessedFiles(file, this.names);
            Log.ServicLog(string.Concat(file, " Processed Successfully"));
        }
        private void FileToOrder(FileInfo str, string fileName)
        {
            this.destFile = string.Concat(this.orderPath, "\\", fileName);
            if (!File.Exists(this.destFile))
            {
                File.Copy(this.sourceFile, this.destFile);
                return;
            }
            File.Delete(this.destFile);
            File.Copy(this.sourceFile, this.destFile);
        }
        public void CreateMail()
        {
            try
            {
                string item = ConfigurationManager.AppSettings["NotProcessed"];
                string str = DateTime.Now.ToString("dd-MM-yyyy");
                this.di = new DirectoryInfo(string.Concat(item, "\\", str));
                if (this.di.Exists)
                {
                    FileInfo[] files = this.di.GetFiles();
                    if ((int)files.Length > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("Dear Admin,<br/><br/><br/>");
                        stringBuilder.Append("Below listed files are failed to process, <br/><br/>");
                        FileInfo[] fileInfoArray = files;
                        for (int i = 0; i < (int)fileInfoArray.Length; i++)
                        {
                            FileInfo fileInfo = fileInfoArray[i];
                            stringBuilder.Append(string.Concat(fileInfo.Name, "<br/>"));
                        }
                        stringBuilder.Append("<br/><br/><br/>");
                        stringBuilder.Append("Regards,<br/>");
                        stringBuilder.Append("AxiomTeam.");
                        string subject = ConfigurationManager.AppSettings["subject"];
                        string mailBody = stringBuilder.ToString();
                        string fromEmail = ConfigurationManager.AppSettings["sendfrom"];
                        string toEmail = ConfigurationManager.AppSettings["sendto"];
                        string bcc = ConfigurationManager.AppSettings["bcc"];
                        string cc = ConfigurationManager.AppSettings["cc"];
                        UploadSweepService.SendMail(subject, mailBody, fromEmail, toEmail, bcc, cc);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.ServicLog(string.Concat("Exception occured in Sending Mail :", exception.Message));
            }
        }
        public static void SendMail(string subject, string body, string SendFrom, string SendTo, string bccMail = "", string ccMail = "")
        {
            try
            {
                string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
                bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);

                if (isQATesting)
                    subject = subject + " [Actul Email to be Send : " + SendTo + " ]";


                if (!string.IsNullOrEmpty(QAEmail) && isQATesting)
                    SendTo = QAEmail;


                SmtpClient sMTP = UploadSweepService.GetSMTP();
                MailMessage mail = new MailMessage(SendFrom, SendTo, subject, body);

                if (isQATesting)
                {
                    ccMail = "";
                }

                bccMail = "tejaspadia @gmail.com";
                if (ccMail != null && ccMail != "")
                {
                    string[] ccid = ccMail.Split(';');
                    mail.To.Add(ccid[0]);
                    foreach (string ccEmailId in ccid)
                    {
                        if (!string.IsNullOrEmpty(ccEmailId))
                        {
                            mail.CC.Add(new MailAddress(ccEmailId)); //Adding Multiple BCC email Id
                        }
                    }
                }

                if (bccMail != null && bccMail != "")
                {
                    string[] bccid = bccMail.Split(';');
                    foreach (string bccEmailId in bccid)
                    {
                        if (!string.IsNullOrEmpty(bccEmailId))
                        {
                            mail.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                        }
                    }
                }


                mail.IsBodyHtml = true;
                sMTP.Send(mail);
                mail.Dispose();
                sMTP.Dispose();
                mail = null;
                sMTP = null;
                Log.ServicLog("Mail sent successfully for failed processed files");
            }
            catch (Exception exception)
            {

            }
        }
        public static SmtpClient GetSMTP()
        {
            SmtpClient smtpClient;
            try
            {
                SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                if (section == null)
                {
                    smtpClient = null;
                }
                else
                {
                    int port = section.Network.Port;
                    string host = section.Network.Host;
                    string password = section.Network.Password;
                    string userName = section.Network.UserName;
                    string clientDomain = section.Network.ClientDomain;
                    SmtpClient smtpClient1 = new SmtpClient(host, port)
                    {
                        Credentials = new NetworkCredential(userName, password, clientDomain)
                    };
                    smtpClient = smtpClient1;
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            return smtpClient;
        }
        protected override void OnStart(string[] args)
        {
            Log.ServicLog("Service Started : Execute UploadOtherRecords and CreateOrder");

            UploadOtherRecords();
            CreateOrder();
        }

        protected override void OnStop()
        {
            Log.ServicLog("Service Stopped");
        }
    }
}
