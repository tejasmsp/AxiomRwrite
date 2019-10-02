using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Axiom.Web.Models
{
    public class ProposalFeesApprovalModel
    {
        public string accExecutiveEmail { get; set; }
        public string accExecutiveName { get; set; }
        public string orderno { get; set; }
        public string location { get; set; }
        public string pages { get; set; }
        public string amount { get; set; }
        public string proposalFeesID { get; set; }
        public string part { get; set; }
        public string operation { get; set; }
        public string comment { get; set; }
        public string Newpages { get; set; }
        public string Newamount { get; set; }
        public bool isUpdated { get; set; }
    }
}