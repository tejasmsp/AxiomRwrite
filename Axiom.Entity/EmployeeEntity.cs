using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class EmployeeEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int? CreatedBy { get; set; }
        public string EmpId { get; set; }
        public  int? DepartmentId { get; set; }
        public string Department { get; set; }
        public string EmployeeName { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsDocumentAdmin { get; set; }
        public bool? IsLockedOut { get; set; }
        public bool? IsApproved { get; set; }
        public string SelectedRoles { get; set; }
        public int CompanyNo { get; set; }
        public bool? IsAssignTo { get; set; }
    }
}
