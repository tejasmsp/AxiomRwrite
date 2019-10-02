using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
   public class RoleEntity
    {
        public int? RoleAccessId { get; set; }

        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }
    }
}
