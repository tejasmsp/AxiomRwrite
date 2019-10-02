using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class BillEntity
    {
    }
    public class BillToAttorneyEntity
    {
        public string AttyId { get; set; }
        public string AttorneyName { get; set; }
        public string Warning { get; set; }
    }
    public class BillToAttorneyDetailsEntity
    {
        public Int64 OrderNo { get; set; }
        public string BillingFirmID { get; set; }
        public string BillingAttorneyID { get; set; }
        public string AttorneyName { get; set; }
        public string FirmName { get; set; }
        public string PatientName { get; set; }
        public string LocID { get; set; }
        public string LocationName { get; set; }
    }
    public class SoldToAttorneyDetailsEntity
    {
        public Int64 OrderNo { get; set; }
        public string OrderingAttorney { get; set; }
        public string FirmID { get; set; }
        public string FirmName { get; set; }
        public string AttorneyFirstName { get; set; }
        public string AttorneyLastName { get; set; }
        public string AttyID { get; set; }
        public string PatientName { get; set; }

    }
    public class BillInvoiceListEntity
    {
        public int InvoiceNumber { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string SoldAtty { get; set; }
        public string SoldAttyName { get; set; }
        public string BillAtty { get; set; }
        public string BillAttyName { get; set; }
        public string SoldToFirmID { get; set; }
        public string BillToFirmID { get; set; }
        public string StatusDescription { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? FinChg { get; set; }
    }
    public class EditInvoiceEntity
    {
        public string MemberOf { get; set; }
        public string FirmID { get; set; }
        public string BillAtty { get; set; }
        public string InvHdr { get; set; }
        public string InvoiceHeader { get; set; }
        public string FirmName { get; set; }
        public Int16 Pages { get; set; }
        public int ItemNo { get; set; }
        public int InvoiceNumber { get; set; }
        public int RcvdID { get; set; }
        public byte Original { get; set; }
        public decimal Copies { get; set; }
        public decimal OrigRate { get; set; }
        public decimal CopyRate { get; set; }
        public decimal StdFee1 { get; set; }
        public decimal StdFee2 { get; set; }
        public decimal StdFee3 { get; set; }
        public decimal StdFee4 { get; set; }
        public decimal StdFee5 { get; set; }
        public decimal StdFee6 { get; set; }
        public decimal StdFee7 { get; set; }
        public decimal StdFee8 { get; set; }
        public int MiscChrge { get; set; }
        public string RcvdDate { get; set; }
        public string InvDate { get; set; }
    }

    public class BillingRecordTypeListEntity
    {
        public byte CodeGroup { get; set; }
        public string GroupDescr { get; set; }
        public byte Code { get; set; }
        public string Descr { get; set; }
        public string Descr2 { get; set; }
        public bool isDisable { get; set; }
    }

    public class SoldAttorneyEntity
    {
        public string AttyId { get; set; }
        public string AttyType { get; set; }
    }
    public class GenerateInvoiceMultiple
    {
        public string OrderId { get; set; }
        public string PartNo { get; set; }
    }

    public class GenerateInvoiceEntity
    {
        public int InvNo { get; set; }
        public decimal OrigRate { get; set; }
        public decimal StdFee1 { get; set; }
        public decimal StdFee2 { get; set; }
        public decimal StdFee3 { get; set; }
        public decimal StdFee4 { get; set; }
        public decimal StdFee6 { get; set; }
        public int Copies { get; set; }
        public int ItemNo { get; set; }
        public int RcvdID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountForPatientAtty { get; set; }
        public string SoldAtty { get; set; }
        public string Email { get; set; }
        public string LocID { get; set; }
        public string LocationName { get; set; }
        public string DoctorName { get; set; }
        public string Descr { get; set; }
        public string BillingAttorneyID { get; set; }
        public string OrderingAttorney { get; set; }
        public string Patient { get; set; }
        public string Caption { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirmName { get; set; }
        public string InvoiceId { get; set; }
        public string WaiverAttyFirstName { get; set; }
        public string WaiverAttyLastName { get; set; }
        public int Pages { get; set; }
    }
    public class PrintInvoiceEmailBill
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }

    }
    public class PrintInvoiceEmailBillOrderDetail
    {
        public string BillingClaimNo { get; set; }
        public string PatientName { get; set; }
        public string InvHdr { get; set; }
        public Int16 Pages { get; set; }
    }
    public class SendEmailForInvoice
    {
        public int InvNo { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string BillAtty { get; set; }
        public string SoldAtty { get; set; }
        public decimal InvAmt { get; set; }
        public decimal InvAmtForPatientAttorney { get; set; }
        public string PatientName { get; set; }
        public string Caption { get; set; }
        public string Caption1 { get; set; }

        public string LocID { get; set; }
        public string LocationName { get; set; }
        public Int16 Pages { get; set; }
        public string InvHdr { get; set; }

        public string AttyID { get; set; }
        public string AttorneyName { get; set; }
        public string AttorneyEmail { get; set; }
        public string FirmID { get; set; }
        public string FirmName { get; set; }


        public string OrderingAttorneyName { get; set; }
        public string OrderingFirmName { get; set; }
        public bool IsPatientAttorney { get; set; }
        public bool OppSide { get; set; }
    }
    public class BillingProposalReply
    {
        public string BtnType { get; set; }
        public string AccExecutiveEmail { get; set; }
        public string AccExecutiveName { get; set; }
        public string OrderNo { get; set; }
        public string Location { get; set; }
        public string Pages { get; set; }
        public string Amount { get; set; }
        public string PartNo { get; set; }
        public string AttorneyNm { get; set; }      
        public bool DocPreference { get; set; }
        public string Comment { get; set; }
    }
}
