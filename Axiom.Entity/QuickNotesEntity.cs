using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public partial class QuickNotesEntity
    {
        public int PartStatusId { get; set; }
        public string PartStatus { get; set; }
        public string Note { get; set; }
        public string PartStatusGroup { get; set; }
        public int PartStatusGroupId { get; set; }
        public bool? IsHidden { get; set; }
        public int? CreatedBy { get; set; }
        public bool? IsPublic { get; set; }
        public bool? InternalNoteEditable { get; set; }
        public bool? PublicNoteEditable { get; set; }
    }
}
