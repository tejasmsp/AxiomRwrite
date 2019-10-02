using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class FirmScopeEntity
    {
        public int MapID { get; set; }

        public string FirmID { get; set; }

        public string FirmName { get; set; }

        public int RecTypeID { get; set; }

        public string RecTypeName { get; set; }

        public string Scope { get; set; }

        public string ScopeTitle { get; set; }
        public int? CreatedBy { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string FirmType { get; set; }
    }


}
