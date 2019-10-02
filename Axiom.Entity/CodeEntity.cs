using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class CodeEntity
    {
        public Int16 CodeGroup { get; set; }
        public string GroupDescr { get; set; }
        public Int16 Code { get; set; }
        public string Descr { get; set; }
        public string Descr2 { get; set; }
        public bool? isDisable { get; set; }
        public string ExpCode { get; set; }
        public bool? isOnRatePage { get; set; }
    }
}
