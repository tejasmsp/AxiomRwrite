using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class PrintInvoiceDetailEntity
    {
        public string InvHdr { get; set; }
        public int Pages { get; set; }
        public byte Original { get; set; }
        public decimal Copies { get; set; }
        public decimal CopyRate { get; set; }
        public decimal StdFee1 { get; set; }
        public decimal StdFee2 { get; set; }
        public decimal StdFee3 { get; set; }
        public decimal StdFee4 { get; set; }
        public decimal StdFee5 { get; set; }
        public decimal StdFee6 { get; set; }
        public decimal StdFee7 { get; set; }
        public decimal StdFee8 { get; set; }
        public decimal OrigRate { get; set; }
        public string FeeName { get; set; }
        public decimal FeeValue { get; set; }
        public int ItemNo { get; set; }
    }
    public class PrintInvoiceEntity
    {
        public decimal MiscChrg { get; set; }
        public string CompName { get; set; }
        public string TaxID { get; set; }
        public string CompStreet { get; set; }
        public string CompCity { get; set; }
        public string CompState { get; set; }
        public string CompZip { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyFaxNumber { get; set; }
        public Int16 RemitNo { get; set; }
        public string BillPhoneNumber { get; set; }
        public string BillFaxNumber { get; set; }
        public string BillFirmName { get; set; }
        public string BillAttorneyName { get; set; }
        public string BillAddress { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillZip { get; set; }
        public string SoldPhoneNumber { get; set; }
        public string SoldFaxNumber { get; set; }
        public string SoldFirmName { get; set; }
        public string SoldAttorneyName { get; set; }
        public string SoldAddress { get; set; }
        public string SoldCity { get; set; }
        public string SoldState { get; set; }
        public string SoldZip { get; set; }
        public int InvNo { get; set; }
        public string InvDate { get; set; }
        public decimal InvAmt { get; set; }
        public string Message { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string CauseNo { get; set; }
        public string Caption { get; set; }
        public string Name { get; set; }
        public string SSN { get; set; }
        public string DateOfBirth { get; set; }
        public string OrderLocationName { get; set; }
        public string Dept { get; set; }
        public string OrderLocationAddress { get; set; }
        public string OrderLocationCity { get; set; }
        public string OrderLocationState { get; set; }
        public string OrderLocationZip { get; set; }
    }
}
