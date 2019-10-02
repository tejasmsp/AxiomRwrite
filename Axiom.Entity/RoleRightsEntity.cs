using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class RoleRightsEntity
    {
        public long? RightId { get; set; }

        public string ModuleName { get; set; }

        public string SubmoduleName { get; set; }

        public string FunctionName { get; set; }

        public bool? IsActive { get; set; }

        //FuntionInRoles Table

        public long? RoleRightId { get; set; }

        public int? RoleAccessId { get; set; }


        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }


        //Extra Fields

        public string UserId { get; set; }

        public int? UserAccessId { get; set; }

        public bool? IsSelected { get; set; }

        public bool? IsEdited { get; set; }

        

    }
}
