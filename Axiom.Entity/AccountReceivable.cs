using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class AccountReceivable
    {      
        public int ArID { get; set; }
        public string CheckType { get; set; }
        public string FirmID { get; set; }
        public string CheckNumber { get; set; }
        public decimal CheckAmount { get; set; }
        public string Note { get; set; }
        public int UserAccessId { get; set; }
        public string FirmName { get; set; }
        public string InvoicesWithPayment { get; set; }
        public decimal? RemainingAmount { get; set; }
    }
    public class AccountReceivableInvoice
    {
        public int CheckInvoiceId { get; set; }
        public int ArID { get; set; }
        public int InvNo { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal? InvoicePayableAmount { get; set; }
        public decimal? TotalPaymentDoneForCheck { get; set; }
        public decimal? InvAmt { get; set; }       
        public string Note { get; set; }
        public int CreatedBy { get; set; }
        public int? Status { get; set; }
        public int? AdjustmentType { get; set; }
        public string CheckType { get; set; }
    }
    public class InvoiceListEntity
    {
        public int InvNo { get; set; }
        public int OrderNo { get; set; }            
        public decimal? InvoiceAmount { get; set; }
        public decimal? InvoicePayableAmount { get; set; }
        public decimal? InvAmt { get; set; }
        public string SoldAtty { get; set; }
        public string SoldAttyName { get; set; }
        public string BillAtty { get; set; }
        public string BillAttyName { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string StatusDesciption { get; set; }
        public decimal? PaidAmount { get; set; }
        public int TotalRecords { get; set; }
        public decimal? FinChg { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
    }
    public class VoidInvoiceEntity
    {
        public int InvNo { get; set; }
        public int OrderNo { get; set; }
        public decimal InvAmt{ get; set; }
        public string SoldAtty { get; set; }
        public string SoldAttyName { get; set; }
        public string BillAtty { get; set; }
        public string BillAttyName { get; set; }
        public int? Status { get; set; }

    }
    public class ChangeLogCheckEntity
    {
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int CreatedBy { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }

    }
    public class ChangeLogInvoiceEntity
    { 
        public decimal? InvoiceTotalBeforePayment { get; set; }
        public decimal? Payment { get; set; }
        public int InvoiceId { get; set; }
        public decimal? InvoiceRemaingPayment { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public string CheckNumber { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
