using System;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Services;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Web.Security;
using Viewer.Class;

namespace Viewer
{
    public partial class _Default : System.Web.UI.Page
    {
        public string languageJson = "{}";
        public string searchJson = "{}";
        public string htmlTemplates = string.Empty;
        public string redactionReasons = string.Empty;

        static string root = HttpContext.Current.Server.MapPath(".");
        string languageFileName = root + "/viewer-assets/languages/en-US.json";
        string searchtext = root + "/predefinedsearch.json";
        string redactionReasonFile = root + "/redactionReason.json";
        string templatePath = root + "/viewer-assets/templates";
        public string OrderNo = string.Empty;
        public string PartNo = string.Empty;
        public string FileName = string.Empty;
        public string FileDiskName = string.Empty;

        public string GuidVal = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    if (!this.Page.User.Identity.IsAuthenticated)
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                    else
                    {
                        HttpRequest req = HttpContext.Current.Request;
                        string EncrID = HttpUtility.UrlDecode(Request.QueryString["FileID"].ToString()).Replace(" ", "+");

                        if (!string.IsNullOrEmpty(EncrID))
                        {
                            int fileId = Convert.ToInt32(EncryptDecrypt.Decrypt(EncrID));

                            var result = DbAccess.GetFileOrderData(fileId);
                            if (result != null && result.Rows.Count > 0)
                            {
                                OrderNo = Convert.ToString(result.Rows[0]["OrderNo"]);
                                PartNo = Convert.ToString(result.Rows[0]["PartNo"]);
                                FileDiskName = Convert.ToString(result.Rows[0]["FileDiskName"]);
                                FileName = Convert.ToString(result.Rows[0]["FileName"]);
                            }
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            Guid gs;
                            gs = Guid.NewGuid();
                            GuidVal = gs.ToString();
                            string configPath = Path.Combine(req.PhysicalApplicationPath, languageFileName);
                            if (File.Exists(configPath))
                            {
                                using (Stream jsonDataStream = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    using (TextReader tr = new StreamReader(jsonDataStream))
                                    {
                                        languageJson = tr.ReadToEnd();
                                        languageJson = languageJson.Replace('\r', ' ');
                                        languageJson = languageJson.Replace('\n', ' ');
                                        languageJson = languageJson.Replace('\t', ' ');
                                    }
                                    jsonDataStream.Close();
                                }
                            }

                            configPath = Path.Combine(req.PhysicalApplicationPath, searchtext);
                            if (File.Exists(configPath))
                            {
                                using (Stream jsonDataStream = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    using (TextReader tr = new StreamReader(jsonDataStream))
                                    {
                                        searchJson = tr.ReadToEnd();
                                        searchJson = searchJson.Replace('\r', ' ');
                                        searchJson = searchJson.Replace('\n', ' ');
                                        searchJson = searchJson.Replace('\t', ' ');
                                    }
                                    jsonDataStream.Close();
                                }
                            }
                            getTemplates(Path.Combine(req.PhysicalApplicationPath, templatePath));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string[] GetFiles(string sourceFolder, string filters, System.IO.SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }

        private void getTemplates(string templatePath)
        {
            string templateData = string.Empty;
            Dictionary<string, String> json = new Dictionary<string, String>();

            //Location where template files are stored
            var templateList = GetFiles(templatePath, "*Template.html", System.IO.SearchOption.TopDirectoryOnly);

            for (int i = 0; i < templateList.Length; i++)
            {
                if (File.Exists(templateList[i]))
                {
                    using (Stream jsonDataStream = new FileStream(templateList[i], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (TextReader tr = new StreamReader(jsonDataStream))
                        {
                            templateData = tr.ReadToEnd();
                            templateData = templateData.Replace('\r', ' ');
                            templateData = templateData.Replace('\n', ' ');
                            templateData = templateData.Replace('\t', ' ');
                            if (templateData.Length > 0)
                            {
                                var regex = new Regex("Template.html", RegexOptions.IgnoreCase);
                                String fileName = regex.Replace(templateList[i], "");
                                json.Add(System.IO.Path.GetFileName(fileName), templateData);
                            }
                        }
                        jsonDataStream.Close();
                    }
                }
            }
            //stringify JSON object
            htmlTemplates = toJSON(json);
        }

        private string toJSON(Object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        [WebMethod]
        public static void AddFileToPartNew(string BatchId, int OrderNo, int PartNo, string FileName, string FileNamewithSigned)
        {
            bool isDebug = false;
            Guid UserID;
            if (isDebug)
                UserID = new Guid("3668BE61-8089-42AC-AC30-8334BE2C7410");
            else
                UserID = new Guid(Membership.GetUser().ProviderUserKey.ToString());

            var fileResult = DbAccess.GetFileOrderData(0, OrderNo, PartNo, FileName);

            FileEntity objFile = new FileEntity();
            objFile.CreatedBy = UserID;
            objFile.FileName = FileNamewithSigned;
            objFile.OrderNo = OrderNo;
            objFile.PartNo = PartNo;

            if (fileResult != null)
            {
                objFile.FileTypeId = Convert.ToInt32(fileResult.Rows[0]["FileTypeId"]);
                objFile.IsPublic = Convert.ToBoolean(fileResult.Rows[0]["IsPublic"]);
                objFile.RecordTypeId = Convert.ToInt32(fileResult.Rows[0]["RecordTypeId"]);
            }
            else
            {
                objFile.FileTypeId = 11;
                objFile.IsPublic = true;
                objFile.RecordTypeId = 0;
            }
            objFile.FileDiskName = BatchId;
            objFile.PageNo = 0;
            DbAccess.InsertFile(objFile);
        }
    }

}