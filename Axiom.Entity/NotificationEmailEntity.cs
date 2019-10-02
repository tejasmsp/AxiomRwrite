using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class NotificationEmailEntity
    {
        public int ID { get; set; }
        public int? OrderNo { get; set; }
        public string AttyID { get; set; }
        public string AttyName { get; set; }
        public string AttyEmail { get; set; }
        public int? AssistantID { get; set; }
        public string AssistantName { get; set; }
        public string AssistantEmail { get; set; }
        public bool? OrderConfirmation { get; set; }
        public bool? FeeApproval { get; set; }
        public bool? AuthNotice { get; set; }
        public bool? NewRecordAvailable { get; set; }
        public DateTime? CratedDate { get; set; }
    }
}
