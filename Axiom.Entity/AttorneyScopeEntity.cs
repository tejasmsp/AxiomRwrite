using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class AttorneyScopeEntity
    {
        public int MapID { get; set; }

        public string AttyID { get; set; }

        public string AttorneyName { get; set; }

        public int RecTypeID { get; set; }

        public string RecTypeName { get; set; }

        public string Scope { get; set; }

        public string ScopeTitle { get; set; }
        public int? CreatedBy { get; set; }

    }

    public class DefaultScopeEntity
    {
        public int ScopeID { get; set; }

        public string Descr { get; set; }

        public string ScopeDesc { get; set; }

        public int RecType { get; set; }

        public bool? IsDefault { get; set; }

    }
}
