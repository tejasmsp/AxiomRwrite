using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class PartsByDateEntity
    {
        public string PartDate { get; set; }
        public int CountOfPartNo { get; set; }
        public int CompanyNo { get; set; }
        public string CompanyName { get; set; }
        // public string Location { get; set; }

    }
    public class InvoiceByDateEntity
    {
        public int InvNo { get; set; }
        public string InvDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        // public decimal InvAmt { get; set; }
        public string SoldAtty { get; set; }
        public string OrderNo { get; set; }
        public string FirmName { get; set; }
        public string AttorneyName { get; set; }
        public string CompName { get; set; }
    }
    public class ChecksByDateEntity
    {
        public string PaymentDate { get; set; }
        public string CheckNo { get; set; }
        public decimal CheckAmount { get; set; }
        public string PaidBy { get; set; }

    }
    public class ChecksByNumberEntity
    {
        public string CheckNo { get; set; }
        public string CheckDate { get; set; }
        public decimal CheckAmount { get; set; }
        public string OrderNo { get; set; }
        private string _voidDate;
        public string VoidDate { get { return _voidDate == "01/01/1900" ? "" : _voidDate; } set { _voidDate = value; } }

    }
    public class OrderBySSNEntity
    {
        public string OrderNo { get; set; }
        public string SSN { get; set; }
        public string Patient { get; set; }
        public string LocID { get; set; }
        public string LocationName { get; set; }

    }
    public class NonInvoicedPartsEntity
    {
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string Location { get; set; }

        private string _CompleteDate;
        public string CompleteDate { get { return _CompleteDate == "01/01/1900" ? "" : _CompleteDate; } set { _CompleteDate = value; } }

        private string _CancelledDate;
        public string CancelledDate { get { return _CancelledDate == "01/01/1900" ? "" : _CancelledDate; } set { _CancelledDate = value; } }

        //public string CompleteDate { get; set; }
        //public string CancelledDate { get; set; }
        // public string NRDate { get; set; }

        //private string _NRDate;
        //public string NRDate { get { return _NRDate == "01/01/1900" ? "" : _NRDate; } set { _NRDate = value; } }

        //public string CheckedByLeah { get; set; }
    }
    public class HanoverBillingEntity
    {
        public string InvoiceDate { get; set; }
        public string OrderNo { get; set; }
        public string BillingClaimNo { get; set; }
        public int InvoiceNo { get; set; }
        public string OrderDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string FirmName { get; set; }
        public string PatientName { get; set; }

    }
    public class HanoverBillingFeesEntity
    {
        public string InvoiceDate { get; set; }
        public string OrderNo { get; set; }
        public string BillingClaimNo { get; set; }
        public int InvoiceNo { get; set; }
        public string OrderDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string Attorney { get; set; }
        public string FirmName { get; set; }
        public decimal RegFee { get; set; }
        public string Description { get; set; }
        public string PatientName { get; set; }
    }
    public class GrangeBillingEntity
    {
        public int InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string OrderNo { get; set; }
        public string BillingClaimNo { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal Balance { get; set; }
        public string PatientName { get; set; }
        public string FirmName { get; set; }
        public string State { get; set; }
        public string OrderingAttorney { get; set; }
        public string OrderBy { get; set; }
        public string OrderDate { get; set; }
    }

    public class GroverBillingEntity
    {
        public string InvoiceDate { get; set; }
        public string OrderNo { get; set; }
        public string Patient { get; set; }
        public string ClaimNo { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal Balance { get; set; }
        public string OrderingAttorney { get; set; }
        public string Location { get; set; }
        public string FirmName { get; set; }
    }
    public class AgedAR
    {
        public int InvoiceNo { get; set; }
        public string OrderNo { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string InvoiceDate { get; set; }
        public string FirmName { get; set; }
        public string Patient { get; set; }
        public string Location { get; set; }
        public int Days { get; set; }
    }
    public class AgedARSummary
    {
        public string FirmName { get; set; }
        public string FirmID { get; set; }
        public int InvoiceNo { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }

        public decimal ThirtyDaysAmount { get; set; }
        public decimal SixtyDaysAmount { get; set; }
        public decimal NintyDaysAmount { get; set; }
        public decimal NintyPlysDaysAmount { get; set; }
        public decimal TotalPending { get; set; }
    }
}
