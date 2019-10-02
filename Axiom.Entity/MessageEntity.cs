using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class MessageEntity
    {
        public int? ID { get; set; }

        public string Name { get; set; }

        public string CustomMessage { get; set; }

        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class UserNotificationEntity
    {
        public int? ID { get; set; }
        public int? UserAccessID { get; set; }
        public int? OrderNo { get; set; }
        public int? PartNo { get; set; }
        public bool? isView { get; set; }
        public string NotificationText { get; set; }
        public string strCreatedDate { get; set; }
    }
}
