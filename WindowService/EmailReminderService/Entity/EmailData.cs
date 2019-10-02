using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailReminderService.Entity
{
    public class EmailData { }

    public class OrderPatient
    {
        public string Name { get; set; }
        public string OrderingAttorney { get; set; }
    }

    public class BillAttorney
    {
        public string Name { get; set; }
        public string BillingAttorneyId { get; set; }
        public string OrderingAttorney { get; set; }
    }

    public class Attorney
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
    public class Location
    {
        public string LocID { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
    }
    public class AuthorizationToBeCalledOnEmail
    {
        public int FollowupID { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
    }
    public class PartDetail
    {
        public int PartNo { get; set; }
    }
    public class AdditionalContact
    {
        public string AttyID { get; set; }
        public string AssistantName { get; set; }
        public string AssistantEmail { get; set; }
    }
    public class ClientAcct
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class AttornyAuthorization
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AttyID { get; set; }
    }

}
