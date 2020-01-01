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
        public int? CompanyNo { get; set; }


        public string LocID { get; set; }
        public string locationName1 { get; set; }
        public string locationName2 { get; set; }

        public string OrderPatientName { get; set; }
        public string OrderingAttorney { get; set; }
        public string BillingAttorneyId { get; set; }
        public string BillingAttorneyFirstName { get; set; }
        public string BillingAttorneyLastName { get; set; }
        public string BillingAttorneyEmail { get; set; }

        public string OrderingAttorneyFirstName { get; set; }
        public string OrderingAttorneyLastName { get; set; }
        public string OrderingAttorneyEmail { get; set; }

        public string ClientAttorneyName { get; set; }
        public string ClientAttorneyEmail { get; set; }

        public string AssistantEmail { get; set; }
        public bool? HasBillingAttorney { get; set; }

    }
}
