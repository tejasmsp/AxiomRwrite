using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFormService.Entity
{
    public class EmailDetails
    {
        public string OrderNo { get; set; }
        public string PartNo { get; set; }
        public string Caption { get; set; }
        public string CauseNumber { get; set; }
        public string PatientName { get; set; }
        public string AccExeName { get; set; }
        public string AccExeEmail { get; set; }
        public string AccExePhone { get; set; }
    }
}
