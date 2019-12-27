using System;

namespace Axiom.Entity
{
    public partial class FirmEntity
    {
        public string FirmID { get; set; }
        public string FirmName { get; set; }
        public Byte? Rating { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo { get; set; }
        public string AreaCode2 { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string Warning { get; set; }
        public string MiscCode { get; set; }
        public string MemberOf { get; set; }
        public Byte? FirmType { get; set; }
        public Byte? COD { get; set; }
        public Byte? StmtType { get; set; }
        public Byte? Detail { get; set; }
        public decimal? LateChg { get; set; }
        public string ClientOf { get; set; }
        public string SalesRep { get; set; }
        public Byte? ConStmt { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? Discount { get; set; }
        public decimal? FinChg { get; set; }
        public string StaffOf { get; set; }
        public Byte? SendToAdj { get; set; }
        public decimal? CrLimit { get; set; }
        public string Notes { get; set; }
        public string Term { get; set; }
        public decimal? AccRate { get; set; }
        public string Collector { get; set; }
        public Byte? AcctStat { get; set; }
        public Byte? CommType { get; set; }
        public string Password { get; set; }
        public DateTime? CallBack { get; set; }
        public DateTime? Action { get; set; }
        public DateTime? PmtDate { get; set; }
        public decimal PmtAmt { get; set; }
        public DateTime? CODSent { get; set; }
        public DateTime? DemandSent { get; set; }
        public DateTime? ChngDate { get; set; }
        public string ChngBy { get; set; }
        public DateTime? EntDate { get; set; }
        public string EntBy { get; set; }
        public Byte[] TStamp { get; set; }
        public decimal? DocCapForRecords { get; set; }
        public decimal? DocCapForFilms { get; set; }
        public string DocProductionPreference { get; set; }
        public bool? OwnOrganization { get; set; }
        public bool? OwnFacesheet { get; set; }
        public bool? MonthlyBilling { get; set; }
        public string ClientOfFirstName { get; set; }
        public string ClientOfLastName { get; set; }
        public string SalesRepFirstName { get; set; }
        public string SalesRepLastName { get; set; }
        public string MemberOfName { get; set; }
        public bool? LinkRequest { get; set; }
        public string Contact { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string LinkedContacts { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime? LastSalesCall { get; set; }
        public int? UsedRank { get; set; }
        public char? CompanyID { get; set; }
        public string LEDESBillingCode { get; set; }
        public bool isAssociated { get; set; }
        public int? RequestSent { get; set; }
        public int CompanyNo { get; set; }
        public bool? IndividualRequest { get; set; }
    }

    public partial class FirmList
    {
        public int TotalRecords { get; set; }
        public string FirmID { get; set; }
        public string FirmName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public int IsAssociated { get; set; }
        public bool isDefault { get; set; }
    }
    public partial class AssociatedFirmEntity
    {
        public int ID { get; set; }
        public string FirmID { get; set; }
        public string AssociatedFirmID { get; set; }
        public string FirmName { get; set; }
    }

    public class MemberOfIDEntity
    {
        public int FirmBillingRateID { get; set; }
        public string FirmID { get; set; }
        public string MemberOf { get; set; }
        public string CompanyID { get; set; }
        public string CompName { get; set; }
    }
    public class AdditionalContactEntity
    {
        public int ContactID { get; set; }
        public string AttyID { get; set; }
        public string FirmID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactFor { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
    public class FirmForm
    {
        public int FirmformID { get; set; }
        public string FirmID { get; set; }
        public int FormID { get; set; }
        public string FolderPath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsFaceSheet { get; set; }
        public bool IsRequestForm { get; set; }
        //Document tables columns
        public string FolderName { get; set; }
        public string DocFileName { get; set; }
    }
}
