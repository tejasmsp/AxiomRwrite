using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using AxiomAutomation.Entity;
using System.Text.RegularExpressions;


namespace AxiomAutomation
{
    public partial class AxiomAutomation : ServiceBase
    {
        public static int IsRevised = 0;
        public static string IsRevisedText = "";
        public static string BillFirm = "";
        public static string ClaimNo = "";
        public static int RecordType = 0;
        public static string AttyName = "";
        public static string AttyID = "";
        System.Timers.Timer serviceTimer = null;
        public string time1 = string.Empty;
        public string time2 = string.Empty;
        public string time3 = string.Empty;
        public string time4 = string.Empty;
        Aspose.Words.License license = new Aspose.Words.License();
        string documentRoot = string.Empty;

        public AxiomAutomation()
        {
            license.SetLicense("Aspose.Words.lic");
            documentRoot = ConfigurationManager.AppSettings["DocumentRoot"].ToString();
            Log.ServicLog("======================================================================");
            Log.ServicLog("Initialize");
            Log.ServicLog("Duration Time :  " + ConfigurationManager.AppSettings["Duration"].ToString() + " min");
            InitializeComponent();
            serviceTimer = new System.Timers.Timer();
            serviceTimer.Elapsed += serviceTimer_Elapsed;
            serviceTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["Duration"].ToString()) * 60 * 1000;
            serviceTimer.Enabled = true;

            time1 = ConfigurationManager.AppSettings["Time1"].ToString();
            time2 = ConfigurationManager.AppSettings["Time2"].ToString();
            time3 = ConfigurationManager.AppSettings["Time3"].ToString();
            time4 = ConfigurationManager.AppSettings["Time4"].ToString();

            serviceTimer.Start();

        }

        void serviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log.ServicLog(DateTime.Now.ToString() + " TIMER ELAPSED");
            //AutomationMain();            
            if (DateTime.Now.ToString("HH") == time1 || DateTime.Now.ToString("HH") == time2 || DateTime.Now.ToString("HH") == time3 || DateTime.Now.ToString("HH") == time4)
                AutomationMain();
        }

        protected override void OnStart(string[] args)
        {
            Log.ServicLog(DateTime.Now.ToString() + " SERVICE STARTED");
            AutomationMain();
        }

        protected override void OnStop()
        {
        }


       


        public void AutomationMain()
        {

            CompanyDetailForEmailEntity objCompany = new CompanyDetailForEmailEntity();



            try
            {
                Log.ServicLog(DateTime.Now.ToString() + "======================= Automation Execution Start ===========================");
                var partList = DbAccess.GetPartList();

                if (partList != null && partList.Count > 0)
                {
                    foreach (var pt in partList)
                    {
                        try
                        {
                            int OrderNo = pt.OrderNo;
                            int PartNo = pt.PartNo;

                            objCompany = DbAccess.CompanyDetailForEmail("CompanyDetailForEmailByOrderNo", pt.OrderNo).FirstOrDefault();


                            bool isProcessServer = false;
                            var location = DbAccess.GetPartLocation(pt.LocID);

                            Log.ServicLog(OrderNo + " " + PartNo);

                            var Attorney = DbAccess.GetAttorneyByOrder(OrderNo);
                            if (Attorney != null)
                            {
                                AttyName = Attorney.AttorneyName;
                                AttyID = Attorney.AttyID;
                            }

                            #region Main Code

                            string filetype = "pdf";
                            var docResult = DbAccess.GetDocsForRequest(OrderNo, Convert.ToString(PartNo));
                            if (docResult != null && docResult.Count > 0)
                            {
                                var AutomationProcess = new AutomationProcess(documentRoot, RecordType, BillFirm, ClaimNo, AttyName, AttyID,true);
                                foreach (var docitem in docResult)
                                {
                                    AutomationProcess.DoRequireOperationOnDocuments(docitem, OrderNo, PartNo, filetype, location, partList.Count, objCompany, isProcessServer);
                                }

                            }
                            string AsgnTo;
                            DateTime CallBack = DateTime.Now.AddDays(14);
                            if (!isProcessServer)
                            {
                                AsgnTo = "CBLIST";
                            }
                            else
                            {
                                AsgnTo = "UTILRE";
                            }

                            DbAccess.UpdateOrderPart(OrderNo, PartNo, AsgnTo, CallBack);


                            if (location != null)
                            {

                                if (location.ReqAuthorization == true || location.FeeAmountSendRequest == true)
                                {
                                    DbAccess.UpdateOrderPart(OrderNo, PartNo, "UTILRE", Convert.ToDateTime(pt.CallBack));
                                    string partNotes = string.Empty;
                                    AutomationProcess.CreateNoteString(OrderNo, PartNo, "Assign to In Office Request.", "SYSTEM", ref partNotes, false, false);
                                }

                            }
                            #endregion Main Code

                        }
                        catch (Exception ex)
                        {
                            Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION 933 -------------------");
                            Log.ServicLog(Convert.ToString(ex.Message));
                            Log.ServicLog(Convert.ToString(ex.StackTrace));
                            Log.ServicLog(Convert.ToString(ex.InnerException));
                            Log.ServicLog("--------------------------------------------");
                        }


                    }
                }
                GC.Collect();
                Log.ServicLog(DateTime.Now.ToString() + "============================ Automation Execution Complete ============================ ");
            }
            catch (Exception ex)
            {
                Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION 948  -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
        }
       
    }
}
