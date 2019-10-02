using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class DistrictEntity
    {
        public int? DistrictID { get; set; }

        public string DistrictName { get; set; }

        public string StateName { get; set; }

        public string StateId { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

    }
}
