using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailReminderService.Entity
{
    public class ProposalFees
    {
        public int ProposalFeeID { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string Descr { get; set; }
        public int Pages { get; set; }
        public decimal Amount { get; set; }
    }
}
