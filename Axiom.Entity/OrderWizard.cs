using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class OrderWizard
    {

    }

    public class OrderWizardStep1
    {
        public long? OrderId { get; set; }
        public string OrderingFirmID { get; set; }
        public string OrderingAttorney { get; set; }
        public string AttorneyFor { get; set; }
        public string Represents { get; set; }
        public string EmpId { get; set; }
        public int? UserAccessId { get; set; }
        public bool? IsFromClient { get; set; }
        public List<NotificationEmailEntity> NotificationEmail { get; set; }
        public string UserEmail { get; set; }
        public int? SubmitStatus { get; set; }
        public int CompanyNo { get; set; }
        public List<OrderWizardStep6Document> LocDocumentList { get; set; }
    }

    public class OrderWizardStep2
    {
        public long? OrderBillingId { get; set; }
        public long? OrderId { get; set; }
        public bool? BillToOrderingFirm { get; set; }
        public string BillingFirmId { get; set; }
        public string BillingAttorneyId { get; set; }

        public string BillingClaimNo { get; set; }
        public string BillingInsured { get; set; }
        public string EmpId { get; set; }
        public int? UserAccessId { get; set; }
    }
    public class OrderWizardStep3
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
        public string ZipCode { get; set; }
        public string ClaimMatterNo { get; set; }
        public int PatientTypeId { get; set; }
        public string EmpId { get; set; }
        public int? UserAccessId { get; set; }
    }

    public class OrderWizardStep4
    {
        public long? OrderId { get; set; }
        public int? CaseTypeId { get; set; }
        public string Caption1 { get; set; }
        public string VsText1 { get; set; }
        public string Caption2 { get; set; }
        public string VsText2 { get; set; }
        public string Caption3 { get; set; }
        public string VsText3 { get; set; }
        public string CauseNo { get; set; }
        public string ClaimMatterNo { get; set; }
        public DateTime? TrialDate { get; set; }
        public string State { get; set; }
        public string StateName { get; set; }
        public int? IsStateOrFedral { get; set; }
        public string County { get; set; }
        public string Court { get; set; }
        public string District { get; set; }
        public string Division { get; set; }
        public string EmpId { get; set; }
        public int? UserAccessId { get; set; }
        public DateTime? BillingDateOfLoss { get; set; }
    }




    public class OrderWizardStep5
    {
        public string EmpId { get; set; }
        public int? UserAccessId { get; set; }
        public long? OrderFirmAttorneyId { get; set; }
        public long? OrderId { get; set; }
        public string FirmID { get; set; }
        public string AttyID { get; set; }
        public string AttorneyFor { get; set; }
        public bool? IsPatientAttorney { get; set; }
        public bool? OppSide { get; set; }
        public string Represents { get; set; }
        public string Notes { get; set; }

        public string FirmName { get; set; }
        public string AttorneyFirstName { get; set; }
        public string AttorneyLastName { get; set; }
        public string AreaCode2 { get; set; }
        public string FaxNo { get; set; }
        public string StateBarNo { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
    }
    public class OrderSearchClientSide
    {
        public int UsedRank { get; set; }
        public string LocID { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string Dept { get; set; }
        public string City { get; set; }
        public string State { get; set; }

    }
    public class OrderWizardStep6
    {
        public string LocID { get; set; }
        public int? PartNo { get; set; }
        public long OrderLocationId { get; set; }
        public Int64 OrderId { get; set; }
        public bool? IsAuthorization { get; set; }
        public int? RequestMeansId { get; set; }
        public bool? IsRequireAdditionalService { get; set; }
        public DateTime? ScopeStartDate { get; set; }
        public DateTime? ScopeEndDate { get; set; }
        public bool? IsOtherChecked { get; set; }
        public int? RecordTypeId { get; set; }
        public string EmpId { get; set; }
        public int? UserAccessId { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Dept { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo1 { get; set; }
        public string AreaCode2 { get; set; }
        public string PhoneNo2 { get; set; }
        public string AreaCode3 { get; set; }
        public string FaxNo { get; set; }
        public string Scope { get; set; }
        public string Comment { get; set; }
        public string Contact { get; set; }
        public List<OrderWizardStep6Document> DocumentFileList { get; set; }
        public string CreatedBy { get; set; }
        public string Note { get; set; }
        public string AsgnTo { get; set; }
        public string Notes { get; set; }
        public string RecordTypeDesc { get; set; }
        public bool? Rush { get; set; }
        public string RoleName { get; set; }
        public string LoggedInUserEmail { get; set; }
        public string IStatus { get; set; }
        public bool AllowDelete { get; set; }
    }
    public class OrderWizardStep6Document
    {
        public string BatchId { get; set; }
        public string FileName { get; set; }
        public long OrderNo { get; set; }
        public int PartNo { get; set; }
        public string FileDiskName { get; set; }
        public string CreatedBy { get; set; }
        public int? RecordTypeId { get; set; }
        public int FileTypeId { get; set; }
        public int? Pages { get; set; }
        public string LocID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsAuthSub { get; set; }

    }

    public class CanvasRequestEntity
    {
        public int? ID { get; set; }
        public int? OrderID { get; set; }
        public int? PartNo { get; set; }
        public int? PkgType { get; set; }
        public string PkgVal { get; set; }
        public string ZipCode { get; set; }
        public int? FileCount { get; set; }
        public int? UserAccessId { get; set; }
        public string PkgName { get; set; }
        public string PkgDescription { get; set; }
    }

    public class FirmForBilling
    {
        public int ID { get; set; }
        public string FirmID { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
    }
    public class OrderCanvasModel
    {
        public List<OrderWizardStep6Document> CanvasFileList { get; set; }
        public int OrderId { get; set; }
        public string userGuid { get; set; }
        public string PkgType { get; set; }
        public string PkgVal { get; set; }
        public string ZipCode { get; set; }
        public string FileCount { get; set; }
        public string UserAccessId { get; set; }
    }
    public class LocationFilesModel
    {
        public int ID { get; set; }
        public string LocID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BatchId { get; set; }
        public string FileName { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string FileDiskName { get; set; }
        public Guid CreatedBy { get; set; }
        public int? RecordTypeId { get; set; }
        public int FileTypeId { get; set; }
        public int? Pages { get; set; }


    }
    public class ESignOrderAttorney
    {
        public string AttyID { get; set; }
        public string AttyName { get; set; }
        public string Email { get; set; }
    }
}
