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
        public string EmployeeName { get; set; } 
    }
    public class LogFilterEntity
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public string EmployeeName { get; set; }
        public string UserId { get; set; }
        public string EmpId { get; set; } 
        public int LogCount { get; set; }
        public bool IsSelected { get; set; }
    }
}
