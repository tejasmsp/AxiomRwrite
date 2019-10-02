using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class MiscellaneousChargesEntity
    {
        public int FeeNo { get; set; }
        public string MemberID { get; set; }
        public string MiscDesc { get; set; }
        public decimal DiscountFee { get; set; }
        public decimal RegularFee { get; set; }
        public string CreatedBy { get; set; }
        public string ProgID { get; set; }

    }
  
}
