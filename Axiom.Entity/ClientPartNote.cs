using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class ClientPartNote
    {
        public int? OrderId { get; set; }
        public int? PartNo { get; set; }
        public string NotesClient { get; set; }
        public string UserId { get; set; }
  
        public string RoleName { get; set; }
    }
}
