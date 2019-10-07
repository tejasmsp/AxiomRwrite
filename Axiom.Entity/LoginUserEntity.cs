using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class LoginUserEntity
    {
        public int? UserAccessId { get; set; }

        public string EmpId { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        public string Email { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsLockedOut { get; set; }

        public int? IsRequestedForUnlock { get; set; }

        public int? InvalidLogin { get; set; }

        public int? FailedPasswordAttemptCount { get; set; }

        public int? RequestType { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool? IsAdmin { get; set; }
        public string Msg { get; set; }
        public int CompanyNo { get; set; }
    }

}
