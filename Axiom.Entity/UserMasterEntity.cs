using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Axiom.Entity
{
    public class UserMasterEntity
    {
        public int UserAccessId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public int IsRequestedForUnlock { get; set; }
        public int? AttorneyEmployeeTypeId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte[] Photo { get; set; }
        public HttpPostedFileBase UploadedFiles { get; set; }
    }
}
