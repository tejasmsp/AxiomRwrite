using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class OrderDetail
    {
    }
    public class OrderFirmAttorneyEntity
    {
        public long? OrderFirmAttorneyId { get; set; }
        public string AttyId { get; set; }
        public string FirmID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirmName { get; set; }
        public bool? IsPatientAttorney { get; set; }
        public string Notes { get; set; }
        public string Descr { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string Assistant { get; set; }
        public string AttyEmail { get; set; }
        public string FirmWarning { get; set; }
        public string Represents { get; set; }

        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo { get; set; }
        public string AreaCode2 { get; set; }
        public string FaxNo { get; set; }
        public string StateBarNo { get; set; }
        public Int16? Salutation { get; set; }
        public bool? OppSide { get; set; }
        public Int16? AttorneyFor { get; set; }
        public long? OrderId { get; set; }
        public string EmpId { get; set; }
        public int? UserAccessId { get; set; }
        public bool? IsOrderAttorneyDisabled { get; set; }
    }
    public class WaiverEntity
    {
        public int PartNo { get; set; }
        public string AttyID { get; set; }
        public bool? Waiver { get; set; }
        public bool? Copy { get; set; }
        public string Location { get; set; }
    }

    public class MiscChargesEntity
    {
        public int MiscChrgId { get; set; }
        public string BillToAttorneyName { get; set; }
        public string Descr { get; set; }
        public int? PartNo { get; set; }
        public decimal? Units { get; set; }
        public decimal? RegFee { get; set; }
        public decimal? Amount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PartNoStr { get; set; }
        public string EmpId { get; set; }
        public int OrderId { get; set; }
    }

    public class OrderPartEntity
    {
        public int PartNo { get; set; }
        public int OrderId { get; set; }
        public string LocID { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Dept { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string CityStateZip
        {
            get
            {
                return City + (string.IsNullOrEmpty(State) ? "" : ", ") + State + (string.IsNullOrEmpty(Zip) ? " " : ", ") + Zip;
            }
        }
        public string AreaCode1 { get; set; }
        public string PhoneNo1 { get; set; }
        public string AreaCode2 { get; set; }
        public string PhoneNo2 { get; set; }
        public string AreaCode3 { get; set; }
        public string FaxNo { get; set; }
        public string Comment { get; set; }
        public string Contact { get; set; }
        public string Warning { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string Scope { get; set; }
        public Int16? RecordTypeId { get; set; }
        public string Descr { get; set; }
        public DateTime? CallBack { get; set; }
        public DateTime? CanDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? EntDate { get; set; }
        public DateTime? FirstCall { get; set; }
        public DateTime? HoldDate { get; set; }
        public DateTime? NRDate { get; set; }
        public DateTime? OrdDate { get; set; }
        public DateTime? AuthRecDate { get; set; }
        public string AsgnTo { get; set; }
        public string IStatus { get; set; }
        public int InternalStatusId { get; set; }
        public string RoleName { get; set; }
        public string StatNotes { get; set; }
        public bool? ISChronology { get; set; }
        public string EmpId { get; set; }
        public string PartStatus { get; set; }
        public DateTime? RequestSendDate { get; set; }
    }

    public class OrderCompanyEntity
    {
        public long? OrderId { get; set; }
        public string RecordsOf { get; set; }
        public string SSN { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ClientMatterNo { get; set; }
        public string PatientType { get; set; }
        public string EmpId { get; set; }
        public int? UserAccessId { get; set; }
        public DateTime? BillingDateOfLoss { get; set; }
        public int CompanyNo { get; set; }
    }

    public class ClientInformation
    {
        public long? OrderId { get; set; }

        public string FirmName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string OrderBy { get; set; }
        public string AttorneyWarning { get; set; }
        public string FirmWarning { get; set; }
        public string Represents { get; set; }
        public string Descr { get; set; }
        public string AttorneyPhone { get; set; }
        public string AttorneyFax { get; set; }
        public string FirmPhone { get; set; }
        public string FirmFax { get; set; }

        public string BillingFirmName { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }

        public string BillingAttorneyWarning { get; set; }
        public string BillingFirmWarning { get; set; }

        public int? CompanyNo { get; set; }
        public string CompName { get; set; }
    }

    public class DownloadMultipleFile
    {
        public string FileDiskName { get; set; }
        public string FileName { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string FileTypeID { get; set; }
    }

    public class FileEntity
    {
        public int FileId { get; set; }
        public int? OrderId { get; set; }
        public int? PartNo { get; set; }
        public string FileName { get; set; }
        public int? FileTypeId { get; set; }
        public string FileType { get; set; }
        public int? RecordTypeId { get; set; }
        public int? PageNo { get; set; }
        public bool? IsPublic { get; set; }
        public string FileDiskName { get; set; }
        public DateTime? DtsCreated { get; set; }
        public string EmpId { get; set; }
    }
    public class FileTypeRecordBillFirm
    {
        public string OrderingFirmID { get; set; }
        public string OrderingAttorney { get; set; }
        public string AttorneyName { get; set; }
        public string BillingClaimNo { get; set; }
    }
    public class OrderNoteEntity
    {
        public DateTime? DtsInserted { get; set; }
        public bool? IsPublic { get; set; }
        public string Note { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public int? OrderId { get; set; }
        public int? PartNo { get; set; }
        public string NotesClient { get; set; }
        public string NotesInternal { get; set; }
        public string UserId { get; set; }
    }
    public class InternalStatus
    {
        public string IStatus { get; set; }
        public string AsgnTo { get; set; }
        public string EmployeeName { get; set; }
        public string AcctRep { get; set; }
        public string AccountRepresentative { get; set; }
    }

    public class OrderDetailEntity
    {
        public string AttyID { get; set; }
        public long OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Represents { get; set; }
        public string AttorneyFor { get; set; }
        public bool? BillToOrderingFirm { get; set; }
        public string BillingClaimNo { get; set; }
        public DateTime? BillingDateOfLoss { get; set; }
        public string BillingInsured { get; set; }
        public string PatientName { get; set; }
        public string SSN { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string OPStateName { get; set; }
        public string ZipCode { get; set; }
        public string ClaimMatterNo { get; set; }
        public string PatientType { get; set; }
        public string CauseNo { get; set; }
        public DateTime? TrialDate { get; set; }
        public string OrderCaseState { get; set; }
        public bool? Rush { get; set; }
        public string District { get; set; }
        public string Division { get; set; }
        public string CountyName { get; set; }
        public string Court { get; set; }
        public string Caption1 { get; set; }
        public string VsText1 { get; set; }
        public string Caption2 { get; set; }
        public string VsText2 { get; set; }
        public string Caption3 { get; set; }
        public string VsText3 { get; set; }
        public string FirmID { get; set; }
        public string FirmName { get; set; }
    }
    public class EmailDetails
    {
        public string OrderNo { get; set; }
        public string PartNo { get; set; }
        public string Caption { get; set; }
        public string CauseNumber { get; set; }
        public string PatientName { get; set; }
        public string AccExeName { get; set; }
        public string AccExeEmail { get; set; }
        public string AccExePhone { get; set; }
    }
    public class AccntRepDetails
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class OrderPartLocationFees
    {
        public int ChkID { get; set; }
        public string ChkNo { get; set; }
        public string LocationName { get; set; }
        public string LocAddress { get; set; }
        public string FirmID { get; set; }
        public Int16? Waived { get; set; }
        public DateTime? VoidDate { get; set; }
        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime EntDate { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string ChngBy { get; set; }
        public bool ToBePrint { get; set; }
    }
    public class LocationFeeCheckIIF
    {
        public int ChkID { get; set; }
        public string checkdate { get; set; }
        public int accountnum { get; set; }
        public string FirmName { get; set; }
        public decimal chkamount { get; set; }
        public string ChkNo { get; set; }
        public string Docnum { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public DateTime Date { get; set; }
        public int AcctNo { get; set; }
        public string Secondname { get; set; }
        public decimal checkamount { get; set; }
        public string Memo { get; set; }
        public string LocationName { get; set; }
        public decimal ChkAmt { get; set; }
        public string AmountInWords { get; set; }
        public string OrderNumber { get; set; }
        public string Dept { get; set; }
        public string RecordName { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
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
}
