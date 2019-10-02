using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class Settings
    {

    }
    public class LogDetailEntity
    {
        public string ActionDate { get; set; }
        public string NoteDate { get; set; }
        public string InternalStatus { get; set; }
        public string StatusClass { get; set; }
        public string Note { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string RecordOf { get; set; }
        public string Location { get; set; }
        public string AssignedTo { get; set; }
    }
}
