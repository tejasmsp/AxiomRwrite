using System;
using System.Web;
using System.Web.Security;
using Viewer.Class;

namespace Viewer
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (DOB.Text.Trim() != "")
            {
                lblErrormsg.Text = "";
                CheckDOB(DOB.Text);
            }
        }
        private void CheckDOB(string DOB)
        {
            string Orderno = "0";
            try
            {
                string EncrID = HttpUtility.UrlDecode(Request.QueryString["ReturnUrl"].ToString().Split('?')[1].Substring(7)).Replace(' ', '+');
                if (!string.IsNullOrEmpty(EncrID))
                {
                    var result = DbAccess.GetFileOrderData(Convert.ToInt32(EncryptDecrypt.Decrypt(EncrID)));
                    if (result != null && result.Rows.Count > 0)
                        Orderno = Convert.ToString(result.Rows[0]["OrderNo"]);
                }
                bool isexist = DbAccess.GetOrderByBirthDate(Convert.ToInt32(Orderno), DOB.Trim());
                if (isexist)
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "josh.sanford@axiomcopy.com", DateTime.Now, DateTime.Now.AddMinutes(30), true, "Administrator", FormsAuthentication.FormsCookiePath);
                    string hash = FormsAuthentication.Encrypt(ticket);
                    HttpCookie cookie = new HttpCookie(
                    FormsAuthentication.FormsCookieName, hash);
                    Response.Cookies.Add(cookie);
                    string returnUrl = Request.QueryString["ReturnUrl"];
                    if (returnUrl == null) returnUrl = "Default.aspx?FileID=xmGHd2joKws=";

                    // Don't call the FormsAuthentication.RedirectFromLoginPage since it could
                    // replace the authentication ticket we just added...
                    Response.Redirect(returnUrl);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                    lblErrormsg.Text = "Wrong Date Of Birth..!!";
            }
            catch (Exception ex) { }
        }
    }
}