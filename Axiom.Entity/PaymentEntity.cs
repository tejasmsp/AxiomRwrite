using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class PaymentEntity
    {
        public int PmtNo { get; set; }
        public string PaidBy { get; set; }
        public string ChkNo { get; set; }
        public DateTime PmtDate { get; set; }
        public DateTime PostDate { get; set; }
        public decimal ChkAmt { get; set; }
        public byte Type { get; set; }
        public int AcctNo { get; set; }
        public int InvNo { get; set; }
        public int OrderNo { get; set; }
        public int CompNo { get; set; }
        public DateTime ChngDate { get; set; }
        public string ChngBy { get; set; }
        public DateTime EntDate { get; set; }
        public string EntBy { get; set; }
    }
}
