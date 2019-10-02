using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class AttorneyEntity
    {
        public string AttyID { get; set; }
        public string AttorneyName { get; set; }

        public string FirmID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo { get; set; }
        public string AreaCode2 { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string StateBarNo { get; set; }
        public string Warning { get; set; }
        public string CreatedBy { get; set; }
    }
    public class AttorneySearchEntity
    {
        public int TotalRecords { get; set; }
        public string AttyID { get; set; }
        public string FirmID { get; set; }
        public string AttorneyName { get; set; }
        public string FirmName { get; set; }
    }

    public class AttorneyMasterEntity
    {
        public string AttyID { get; set; }
        public string FirmID { get; set; }
        public string FirmName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string FAreaCode1 { get; set; }
        public string FPhoneNo { get; set; }
        public string FAreaCode2 { get; set; }
        public string FFaxNo { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo { get; set; }
        public byte SamePhone { get; set; }
        public string AreaCode2 { get; set; }
        public string FaxNo { get; set; }
        public byte SameFax { get; set; }
        public string StateBarNo { get; set; }
        public string Email { get; set; }
        public byte AttyType { get; set; }
        public string Contact { get; set; }
        public string ContactAsstName { get; set; }
        public decimal DocCapForRecords { get; set; }
        public decimal DocCapForFilms { get; set; }
        public string DocProductionPreference { get; set; }
        public bool OwnOrganization { get; set; }
        public bool OwnFacesheet { get; set; }
        public bool OwnNotice { get; set; }
        public bool OwnNoticeCertified { get; set; }
        public bool LinkRequest { get; set; }
        public bool MonthlyBilling { get; set; }
        public int RequestSent { get; set; }
        public string LinkedContacts { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public bool isDisable { get; set; }

        public string ClientOf { get; set; }
        public string ClientOfFirstName { get; set; }
        public string ClientOfLastName { get; set; }
        public string SalesRep { get; set; }
        public string SalesRepFirstName { get; set; }
        public string SalesRepLastName { get; set; }

        public string Warning { get; set; }
        public string Notes { get; set; }

        public Int16 Rating { get; set; }
        public string FedAdmNo { get; set; }
        public Int16 Salutation { get; set; }
        public Int16 CommType { get; set; }
        public string Password { get; set; }
        public Int16 Dormant { get; set; }
        public Int16 FullAccess { get; set; }
        public int Type { get; set; }

        public List<AttorneyAssistantContact> AttorneyAssistantContactList { get; set; }
    }
    public class AttorneyFormEntity
    {
        public int AttyformID { get; set; }
        public string AttyID { get; set; }
        public int FormID { get; set; }
        public byte FormType { get; set; }
        public string FolderPath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsFacesheet { get; set; }
        public bool IsRequestForm { get; set; }
        //Document tables columns
        public string FolderName { get; set; }
        public string DocFileName { get; set; }
    }

    public class AttorneyAssistantContact
    {
        public int? AdditionalContactsID { get; set; }
        public string AttyID { get; set; }
        public string AssistantName { get; set; }
        public string AssistantEmail { get; set; }
        public bool? isAttorney { get; set; }
    }
}


