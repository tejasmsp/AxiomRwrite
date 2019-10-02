using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class CourtEntity
    {
        public int CourtID { get; set; }
        public string CourtType { get; set; }
        public string StateID { get; set; }
        public string StateName { get; set; }
        public int? DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int? CountyID { get; set; }
        public string CountyName { get; set; }
        public string CourtName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
