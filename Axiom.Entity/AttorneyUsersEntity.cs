using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class AttorneyUsersEntity
    {
        public string AttorneyUserId { get; set; }
        public string AttorneyUserName { get; set; }

        public string AttyID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }

        public string FirmId { get; set; }
        public string FirmName { get; set; }
        public string Email { get; set; }
        public string EmployeeType { get; set; }
        public int AttorneyEmployeeTypeId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }

        public int? CreatedBy { get; set; }
        public int CompanyNo { get; set; }

    }
}
