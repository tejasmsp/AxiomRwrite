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
        public HomeApiController homeApiController = new HomeApiController();
        public CompanyApicontroller companyApiController = new CompanyApicontroller();
        #endregion

        #region Methods

        #endregion


        // GET: Login
        public ActionResult Index()
        {
            GetCompanyNoBySiteUrl();
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

        public void GetCompanyNoBySiteUrl()
        {
            string CurrentUrl = Request.Url.AbsoluteUri;
            var response = homeApiController.GetCompanyNoBySiteUrl(CurrentUrl);

            var result = companyApiController.GetCompanyDetailById(response.Data[0]);

            ProjectSession.CompanyUserDetail = new CompanyUserDetail();
            ProjectSession.CompanyUserDetail.CompNo = result.Data[0].CompNo;
            ProjectSession.CompanyUserDetail.CompID = result.Data[0].CompID;
            ProjectSession.CompanyUserDetail.CompName = result.Data[0].CompName;
            ProjectSession.CompanyUserDetail.Street1 = result.Data[0].Street1;
            ProjectSession.CompanyUserDetail.Street2 = result.Data[0].Street2;
            ProjectSession.CompanyUserDetail.City = result.Data[0].City;
            ProjectSession.CompanyUserDetail.State = result.Data[0].State;
            ProjectSession.CompanyUserDetail.Zip = result.Data[0].Zip;
            ProjectSession.CompanyUserDetail.AreaCode1 = result.Data[0].AreaCode1;
            ProjectSession.CompanyUserDetail.PhoneNo = result.Data[0].PhoneNo;
            ProjectSession.CompanyUserDetail.AreaCode2 = result.Data[0].AreaCode2;
            ProjectSession.CompanyUserDetail.FaxNo = result.Data[0].FaxNo;
            ProjectSession.CompanyUserDetail.Email = result.Data[0].Email;
            ProjectSession.CompanyUserDetail.SiteUrl = result.Data[0].SiteURL;
            ProjectSession.CompanyUserDetail.Style = result.Data[0].Style;
            ProjectSession.CompanyUserDetail.ImagePath = "/assets/images/logo-axiom_" + result.Data[0].CompNo + ".png";
        }

        // Post: Login
        [HttpPost]
        public ActionResult Index(LoginUserEntity model)
        {
            model.CompanyNo = ProjectSession.CompanyUserDetail.CompNo;
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
                else if (ProjectSession.LoggedInUserDetail.RoleName.Contains("Attorney"))
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
            string currentUrl = Request.Url.AbsoluteUri;
            var response = homeApiController.GetCompanyNoBySiteUrl(currentUrl);
            model.CompanyNo = response.Data[0];

            try
            {
                CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(model.CompanyNo);

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
                    bodyTemplate = bodyTemplate.Replace("##LogoURL##", objCompany.Logopath);
                    bodyTemplate = bodyTemplate.Replace("##ThankYou##", objCompany.ThankYouMessage);
                    bodyTemplate = bodyTemplate.Replace("##CompanyName##", objCompany.CompName);
                    bodyTemplate = bodyTemplate.Replace("##Link##", objCompany.SiteURL);


                    Email.Send(objCompany.CompNo,MailTo, bodyTemplate, "Login Details", "", "");
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = "Email not register with us.";
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
                    string msg = "Email not register with us.";
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
            string currentUrl = Request.Url.AbsoluteUri;
            var response = homeApiController.GetCompanyNoBySiteUrl(currentUrl);
            model.CompanyNo = response.Data[0];

            CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(model.CompanyNo);

            var Result = loginApi.GetUserDetailByEmail(model);
            if (Result.Data != null && Result.Data.Count > 0)
            {
                LoginUserEntity modelResetPassword = new LoginUserEntity();
                modelResetPassword.UserAccessId = Result.Data[0].UserAccessId;
                modelResetPassword.Password = Security.Encrypt(CreateRandomPassword(8));
                var ResetPassword = loginApi.UpdateNewGereatedPassword(modelResetPassword);
                if (ResetPassword.Success)
                {
                    var MailTo = model.Email;
                    string bodyTemplate = System.IO.File.ReadAllText(Server.MapPath("~/MailTemplate/GetPassword.html"));
                    bodyTemplate = bodyTemplate.Replace("{FirstName}", Result.Data[0].FirstName).Replace("{LastName}", Result.Data[0].LastName);
                    bodyTemplate = bodyTemplate.Replace("{UserName}", Result.Data[0].UserName);
                    bodyTemplate = bodyTemplate.Replace("{Email}", Result.Data[0].Email);
                    //bodyTemplate = bodyTemplate.Replace("{Link}", ConfigurationManager.AppSettings["ResetEmailLink"].ToString());
                    bodyTemplate = bodyTemplate.Replace("{Password}", Security.Decrypt(modelResetPassword.Password));
                    bodyTemplate = bodyTemplate.Replace("{LogoURL}", objCompany.Logopath);
                    bodyTemplate = bodyTemplate.Replace("{ThankYou}", objCompany.ThankYouMessage);
                    bodyTemplate = bodyTemplate.Replace("{CompanyName}", objCompany.CompName);
                    bodyTemplate = bodyTemplate.Replace("{Link}", objCompany.SiteURL);

                    Email.Send(objCompany.CompNo,MailTo, bodyTemplate, "Login Details", "", "");
                    model.Msg = "Password has send successfully in your mail.";
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
                model.Msg = "Email not register with " + objCompany.CompName;
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
