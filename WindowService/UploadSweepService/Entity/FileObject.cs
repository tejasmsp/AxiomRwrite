using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadSweepService.Entity
{
    public class FileObject
    {
        public int OrderNo;
        public int PartNo;
        public Guid UserId;
        public int Pages;
        public int RecordType;
        public bool IsPublic;
        public int FileType;
        public string FileName;

    }
    public class AssistContactEmail
    {
        //public Int64 ID { get; set; }
        public string AttyID { get; set; }
        //public string AssistantID { get; set; }
        public string AssistantName { get; set; }
        public string AssistantEmail { get; set; }
        public string LocID { get; set; }
        public string LocationName { get; set; }
        public string PatientName { get; set; }
        public string BillingClaimNo { get; set; }
        public string InvHdr { get; set; }
        public bool NewRecordAvailable { get; set; }
        public bool OrderConfirmation { get; set; }
        public bool AuthNotice { get; set; }
        public bool FeeApproval { get; set; }
    }
    public class CompanyDetailForEmailEntity
    {
        public Int16 CompNo { get; set; }
        public string CompID { get; set; }
        public string CompName { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string ThankYouMessage { get; set; }
        public string LogoPath { get; set; }
        public string SiteURL { get; set; }

    }
}
