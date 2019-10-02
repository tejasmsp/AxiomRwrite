using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class BillingRateEntity
    {
        public int ID { get; set; }
        public int RateMainID { get; set; }
        public string RateName { get; set; }
        public decimal DcntRate { get; set; }
        public decimal RegRate { get; set; }
        public int RecordTypeID { get; set; }
        public string RecordTypeName { get; set; }
        public string MemberID { get; set; }
        public string BillType { get; set; }
        public Int16 StartPage { get; set; }
        public Int16 EndPage { get; set; }

    }
    public class BillingRateDetailEntity
    {
        public int ID { get; set; }
        public int RateMainID { get; set; }
        public string RateName { get; set; }
        public decimal DcntRate { get; set; }
        public decimal RegRate { get; set; }
        public int RecordTypeID { get; set; }
        public string RecordTypeName { get; set; }
        public string MemberID { get; set; }
        public string BillType { get;set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

    }
}
