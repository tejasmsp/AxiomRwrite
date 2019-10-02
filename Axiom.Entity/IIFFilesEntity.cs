using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class IIFFilesEntity
    {
        public int ChkID { get; set; }
        public string CheckDate { get; set; }
        public string AccountNum { get; set; }
        public string FirmName { get; set; }
        public decimal ChkAmount { get; set; }
        public string ChkNo { get; set; }
        public string DocNum { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Date { get; set; }
        public int AcctNo { get; set; }
        public string SecondName { get; set; }
        public decimal CheckAmount { get; set; }

    }
    public class IIFCheckListEntity
    {
        public int TotalRecords { get; set; }
        public int CheckID { get; set; }
        public string FirmId { get; set; }
        public string CheckNo { get; set; }
        public string IssueDate { get; set; }
        public string Memo { get; set; }
        public DateTime EntDate { get; set; }
        public decimal Amount { get; set; }
        public string Location { get; set; }
        public string OrderNo { get; set; }
        public string FirmName { get; set; }
    }
    public class IIFPrintCheckEntity
    {
        public int ChkID { get; set; }
        public string CheckDate { get; set; }
        public int AccountNum { get; set; }
        public string FirmName { get; set; }
        public decimal ChkAmount { get; set; }
        public string ChkNo { get; set; }
        public string DocNum { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Date { get; set; }
        public int AcctNo { get; set; }
        public string SecondName { get; set; }
        public decimal CheckAmount { get; set; }
        public string Memo { get; set; }
        public string LocationName { get; set; }
        public string AmountInWords { get; set; }

        //public string AmountInWords
        //{
        //    get { return CommonHelper.FormatAmount(Convert.ToString(ChkAmount)); }
        //}

        public string OrderNumber { get; set; }
        public string Dept { get; set; }
        public string RecordName { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }

    }
    public class CommonHelper
    {

        public static string FormatAmount(string amount)
        {
            int Count = 74 - amount.Length;
            string a = "*"; string x = "";
            for (int i = 0; i < Count; i++)
            {
                x = String.Concat(x, a);
            }
            var Final = string.Concat(amount, "  ", x);
            return Final;
        }
    }

}
