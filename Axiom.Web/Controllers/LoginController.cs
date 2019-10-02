using Axiom.Entity;
using Axiom.Web.API;
using Axiom.Web.EmailHelper;
using AXIOM.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axiom.Web.Controllers
{
    public class LoginController : Controller
    {
        #region Initialization
        LoginApiController loginApi = new LoginApiController();
        #endregion

        #region Methods

        #endregion
       

        // GET: Login
        public ActionResult Index()
        {
            if (ProjectSession.LoggedInUserDetail != null && !String.IsNullOrEmpty(ProjectSession.LoggedInUserDetail.UserId))
            {
                if (ProjectSession.LoggedInUserDetail.RoleName.Contains("Administrator") || ProjectSession.LoggedInUserDetail.RoleName.Contains("DocumentAdmin"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                //var RequestUrl = Request.Url;            
                
               
                LoginUserEntity model = new LoginUserEntity();
                return View(model);
            }
        }

        // Post: Login
        [HttpPost]
        public ActionResult Index(LoginUserEntity model)
        {

            var Result = loginApi.LoginUser(model);
            if (Result.Data != null && !String.IsNullOrEmpty(Result.Data[0].UserId))
            {
                ProjectSession.LoggedInUserDetail = new LoggedInUserDetail();
                ProjectSession.LoggedInUserDetail.UserAccessId = Result.Data[0].UserAccessId;
                ProjectSession.LoggedInUserDetail.Email = Result.Data[0].Email.Trim();
                ProjectSession.LoggedInUserDetail.UserId = Result.Data[0].UserId.Trim();
                ProjectSession.LoggedInUserDetail.UserName = Result.Data[0].UserName.Trim();
                ProjectSession.LoggedInUserDetail.EmpId = Result.Data[0].EmpId.Trim();
                ProjectSession.LoggedInUserDetail.IsAdmin = Result.Data[0].IsAdmin;
                ProjectSession.LoggedInUserDetail.RoleName = loginApi.GetUserType(Result.Data[0].UserId.Trim()).Data;

                if (!String.IsNullOrEmpty(Convert.ToString(TempData["ReturnUrl"])))
                {
                    if (Convert.ToString(TempData["ReturnUrl"]) != Common.CommonHelper.SiteRootPathUrl + "/")
                    {
                        return Redirect(Convert.ToString(TempData["ReturnUrl"]));
                    }
                }
                if (ProjectSession.LoggedInUserDetail.RoleName.Contains("Administrator") || ProjectSession.LoggedInUserDetail.RoleName.Contains("DocumentAdmin"))
                {                    
                    return RedirectToAction("Index", "Home");
                }
                else if(ProjectSession.LoggedInUserDetail.RoleName.Contains("Attorney"))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                model.InvalidLogin = Result.Data[0].InvalidLogin;
            }

            return View(model);

        }

        [HttpGet]
        [Route("Logout")]
        public ActionResult Logout()
        {
            ProjectSession.LoggedInUserDetail = null;            
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public ActionResult ForgetPassword(LoginUserEntity model)
        {
            try
            {
                var Result = loginApi.GetUserDetailByEmail(model);
                if (Result.Data != null && Result.Data.Count > 0)
                {
                    var MailTo = model.Email;
                    //string imagepath = Logopath + "/assets/images/LogoSmall.png";
                    //string path = "<img src='" + imagepath + "' class='navbar-brand' href='javascript:' style='width:180px;float: left;margin-left: 20px;padding-bottom: 11px;padding-top: 10px;'>";           
                    string bodyTemplate = System.IO.File.ReadAllText(Server.MapPath("~/MailTemplate/ForgetPassword.html"));
                    bodyTemplate = bodyTemplate.Replace("##Email##", Result.Data[0].Email);
                    bodyTemplate = bodyTemplate.Replace("##UserName##", Result.Data[0].UserName);
                    bodyTemplate = bodyTemplate.Replace("##Password##", Security.Decrypt(Result.Data[0].Password));
                    Email.Send(MailTo, bodyTemplate, "[Axiom] - Login Details", "", "");
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = "Email not register with Axiom.";
                    return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [Route("RequestToAdmin")]
        public ActionResult RequestToAdmin(LoginUserEntity model)
        {
            try
            {
                var Result = loginApi.GetUserDetailByEmail(model);
                if (Result.Data != null && Result.Data.Count > 0)
                {
                    var MailTo = model.Email;
                    //RequestToAdmin Mail Template Remaining         
                    string bodyTemplate = System.IO.File.ReadAllText(Server.MapPath("~/MailTemplate/ForgetPassword.html"));
                    bodyTemplate = bodyTemplate.Replace("##Email##", Result.Data[0].Email);
                    bodyTemplate = bodyTemplate.Replace("##UserName##", Result.Data[0].UserName);
                    bodyTemplate = bodyTemplate.Replace("##Password##", Security.Decrypt(Result.Data[0].Password));
                    //Email.Send(MailTo, bodyTemplate, "[Axiom]", "", "");
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = "Email not register with Axiom.";
                    return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ResetPassword()
        {
            LoginUserEntity model = new LoginUserEntity();
            return View("~/Views/Login/ResetPassword.cshtml", model);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public ActionResult ResetPassword(LoginUserEntity model)
        {
            var Result = loginApi.GetUserDetailByEmail(model);
            if (Result.Data != null && Result.Data.Count > 0)
            {
                LoginUserEntity modelResetPassword = new LoginUserEntity();
                modelResetPassword.UserAccessId = Result.Data[0].UserAccessId;
                modelResetPassword.Password =Security.Encrypt(CreateRandomPassword(8));
                var ResetPassword = loginApi.UpdateNewGereatedPassword(modelResetPassword);
                if (ResetPassword.Success)
                {
                    var MailTo = model.Email;
                    string bodyTemplate = System.IO.File.ReadAllText(Server.MapPath("~/MailTemplate/GetPassword.html"));
                    bodyTemplate = bodyTemplate.Replace("{FirstName}", Result.Data[0].FirstName).Replace("{LastName}", Result.Data[0].LastName);
                    bodyTemplate = bodyTemplate.Replace("{UserName}", Result.Data[0].UserName);
                    bodyTemplate = bodyTemplate.Replace("{Email}", Result.Data[0].Email);
                    bodyTemplate = bodyTemplate.Replace("{Link}", ConfigurationManager.AppSettings["ResetEmailLink"].ToString());
                    bodyTemplate = bodyTemplate.Replace("{Password}", Security.Decrypt(modelResetPassword.Password));
                    Email.Send(MailTo, bodyTemplate, "Axiom - Login Details", "", "");
                    model.Msg= "Password has send successfully in your mail.";
                    return View("~/Views/Login/Index.cshtml", model);
                }
                else
                {
                    model.Msg = "Please Try Again!.";
                    return View("ResetPassword", model);
                }
            }
            else
            {
                model.Msg = "Email not register with Axiom.";
                return View("ResetPassword", model);
            }
            

        }

        public string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

    }
}
