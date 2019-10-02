using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class DepartmentEntity
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int SortOrder { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? isActive { get; set; }
    }
    public class LocationDepartmentDropdown
    {
        public int Code { get; set; }
        public string Descr { get; set; }        
    }
}
