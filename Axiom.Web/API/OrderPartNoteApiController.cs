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
using System.Web;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderPartNoteApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();
        #endregion
        //InsertOrderDocument
        #region Method
        //[HttpGet]
        //[Route("GetOrderNotes")]
        //public ApiResponse<OrderNoteEntity> GetOrderNotes(int OrderId=0 ,int PartNo=0)
        //{
        //    var response = new ApiResponse<OrderNoteEntity>();
        //    try
        //    {
        //        SqlParameter[] param = {   new SqlParameter("OrderId", (object)OrderId?? (object)DBNull.Value)
        //                                  ,new SqlParameter("PartNo",(object)PartNo?? (object)DBNull.Value)
        //                               };
        //        var result = _repository.ExecuteSQL<OrderNoteEntity>("GetOrderNotes", param).ToList();
        //        if (result == null)
        //        {
        //            result = new List<OrderNoteEntity>();
        //        }
        //        response.Success = true;
        //        response.Data = result;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add(ex.Message);
        //    }
        //    return response;
        //}



        [HttpPost]
        [Route("InsertOrderPartNotes")]
        public BaseApiResponse InsertOrderPartNotes(OrderPartNoteEntity modal)
        {
            string OrderIdsWithPartIdsXML = ConvertToXml<OrderIdsPartIds>.GetXMLString(modal.OrderIdPartIdList);

            var response = new BaseApiResponse();
            try
            {
                Guid UserId = new Guid(modal.UserId);
                SqlParameter[] param = {   new SqlParameter("OrderId", (object)modal.OrderId?? (object)DBNull.Value)
                                          ,new SqlParameter("PartNo",(object)modal.PartNo?? (object)DBNull.Value)
                                          ,new SqlParameter("NotesClient",(object)modal.NotesClient?? (object)DBNull.Value)
                                          ,new SqlParameter("NotesInternal",(object)modal.NotesInternal?? (object)DBNull.Value)
                                          ,new SqlParameter("CallBack",(object)modal.CallBack?? (object)DBNull.Value)
                                          ,new SqlParameter("CanDate",(object)modal.CanDate?? (object)DBNull.Value)
                                          ,new SqlParameter("DueDate",(object)modal.DueDate?? (object)DBNull.Value)
                                          ,new SqlParameter("EntDate",(object)modal.EntDate?? (object)DBNull.Value)
                                          ,new SqlParameter("FirstCall",(object)modal.FirstCall?? (object)DBNull.Value)
                                          ,new SqlParameter("HoldDate",(object)modal.HoldDate?? (object)DBNull.Value)
                                          ,new SqlParameter("NRDate",(object)modal.NRDate?? (object)DBNull.Value)
                                          ,new SqlParameter("OrdDate",(object)modal.OrdDate?? (object)DBNull.Value)
                                          ,new SqlParameter("AuthRecDate",(object)modal.AuthRecDate?? (object)DBNull.Value)
                                          ,new SqlParameter("PartStatusId",(object)modal.PartStatusId?? (object)DBNull.Value)
                                          ,new SqlParameter("AssgnTo",(object)modal.AssgnTo?? (object)DBNull.Value)
                                          ,new SqlParameter("InternalStatusId",(object)modal.InternalStatusId ?? (object)DBNull.Value)
                                          ,new SqlParameter("InternalStatusText",(object)modal.InternalStatusText ?? (object)DBNull.Value)
                                          ,new SqlParameter("UserId",(object)UserId?? (object)DBNull.Value)
                                          ,new SqlParameter("PageFrom",(object)modal.PageFrom?? (object)DBNull.Value)
                                          ,new SqlParameter("OrderIdsWithPartIdsXML",(object)OrderIdsWithPartIdsXML?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertOrderPartNotes", param).FirstOrDefault();
                if (result == 1)
                {
                    if (!string.IsNullOrEmpty(modal.RoleName) && modal.RoleName == "Attorney")
                    {
                        string accExecutiveName = string.Empty;
                        string accExecutiveEmail = string.Empty;
                        

                        SqlParameter[] param1 = { new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value) };
                        var MailDetail = _repository.ExecuteSQL<AccountExecutive>("GetClientAcctExec", param1).FirstOrDefault();
                        if (MailDetail != null)
                        {
                            accExecutiveName = Convert.ToString(MailDetail.Name);
                            accExecutiveEmail = Convert.ToString(MailDetail.Email);
                        }
                        else
                        {
                            accExecutiveName = "Leah Boroski";
                            accExecutiveEmail = "leah.boroski@axiomcopy.com";
                        }

                        string subject = "Client Note Added";
                        System.Text.StringBuilder htmlBody = new System.Text.StringBuilder();
                        htmlBody.Append(@"<!DOCTYPE html><html xmlns='http://www.w3.org/1999/xhtml'><head><title></title><style>table, th, td {border: 1px solid gray;font-family:Verdana;}</style></head>");
                        htmlBody.Append(@"<body style='font-family:Verdana;'>");
                        htmlBody.Append(@"##MESSAGE##");
                        htmlBody.Append(@"<table align='left' style='width:650px;'cellpadding='5' cellspacing='0'>");
                        htmlBody.Append(@"<tr><td align='right' style='width:120px;'><b>Order No :</b></td><td>##ORDERNO##</td></tr>");
                        htmlBody.Append(@"<tr><td align='right'><b>Note :</b></td><td>##NOTE##</td></tr>");
                        htmlBody.Append(@"</table></body></html>");
                        htmlBody.Replace("##MESSAGE##", accExecutiveName + ", <br />Client has added new note.<br /><br />");
                        htmlBody.Replace("##ORDERNO##", modal.OrderId + "-" + modal.PartNo);
                        htmlBody.Replace("##NOTE##", modal.NotesClient);
                        EmailHelper.Email.Send(accExecutiveEmail, htmlBody.ToString(), subject, "", "autharchive@axiomcopy.com,tejaspadia@gmail.com");

                    }
                    response.Success = true;
                    response.InsertedId = result;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }


        [HttpPost]
        [Route("InsertOrderPartNotesByClient")]
        public BaseApiResponse InsertOrderPartNotesByClient(ClientPartNote modal)
        {
      

            var response = new BaseApiResponse();
            try
            {
                Guid UserId = new Guid(modal.UserId);
                SqlParameter[] param = {   new SqlParameter("OrderId", (object)modal.OrderId?? (object)DBNull.Value)
                                          ,new SqlParameter("PartNo",(object)modal.PartNo?? (object)DBNull.Value)
                                          ,new SqlParameter("NotesClient",(object)modal.NotesClient?? (object)DBNull.Value)
                                          ,new SqlParameter("UserId",(object)UserId?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertOrderPartNotesByClient", param).FirstOrDefault();
                if (result == 1)
                {
                    if (!string.IsNullOrEmpty(modal.RoleName) && modal.RoleName == "Attorney")
                    {
                        string accExecutiveName = string.Empty;
                        string accExecutiveEmail = string.Empty;


                        SqlParameter[] param1 = { new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value) };
                        var MailDetail = _repository.ExecuteSQL<AccountExecutive>("GetClientAcctExec", param1).FirstOrDefault();
                        if (MailDetail != null)
                        {
                            accExecutiveName = Convert.ToString(MailDetail.Name);
                            accExecutiveEmail = Convert.ToString(MailDetail.Email);
                        }
                        else
                        {
                            accExecutiveName = "Leah Boroski";
                            accExecutiveEmail = "leah.boroski@axiomcopy.com";
                        }

                        string subject = "Client Note Added";
                        System.Text.StringBuilder htmlBody = new System.Text.StringBuilder();
                        htmlBody.Append(@"<!DOCTYPE html><html xmlns='http://www.w3.org/1999/xhtml'><head><title></title><style>table, th, td {border: 1px solid gray;font-family:Verdana;}</style></head>");
                        htmlBody.Append(@"<body style='font-family:Verdana;'>");
                        htmlBody.Append(@"##MESSAGE##");
                        htmlBody.Append(@"<table align='left' style='width:650px;'cellpadding='5' cellspacing='0'>");
                        htmlBody.Append(@"<tr><td align='right' style='width:120px;'><b>Order No :</b></td><td>##ORDERNO##</td></tr>");
                        htmlBody.Append(@"<tr><td align='right'><b>Note :</b></td><td>##NOTE##</td></tr>");
                        htmlBody.Append(@"</table></body></html>");
                        htmlBody.Replace("##MESSAGE##", accExecutiveName + ", <br />Client has added new note.<br /><br />");
                        htmlBody.Replace("##ORDERNO##", modal.OrderId + "-" + modal.PartNo);
                        htmlBody.Replace("##NOTE##", modal.NotesClient);
                        //  EmailHelper.Email.Send("j.alspaugh@axiomcopy.com", htmlBody.ToString(), subject, "tejaspadia@gmail.com", "");
                        EmailHelper.Email.Send(accExecutiveEmail, htmlBody.ToString(), subject, "", "autharchive@axiomcopy.com,tejaspadia@gmail.com");
                    }
                    response.Success = true;
                    response.InsertedId = result;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route("RemoveCallBackFromNote")]
        public BaseApiResponse RemoveCallBackFromNote(OrderPartNoteEntity modal)
        {
            string OrderIdsWithPartIdsXML = ConvertToXml<OrderIdsPartIds>.GetXMLString(modal.OrderIdPartIdList);

            var response = new BaseApiResponse();
            try
            {
                //Guid UserId = new Guid(modal.UserId);
                SqlParameter[] param = { new SqlParameter("UserId",(object)modal.UserId?? (object)DBNull.Value)                                         
                                          ,new SqlParameter("OrderIdsWithPartIdsXML",(object)OrderIdsWithPartIdsXML?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("RemoveCallBackForSelected", param).FirstOrDefault();
                if (result == 1)
                {                                     
                    response.Success = true;              
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        #endregion
    }
}
