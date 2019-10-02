using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class CustomMessageViewEntity
    {
        public byte CodeGroup { get; set; }
        public string GroupDescr { get; set; }
        public byte Code { get; set; }
        public string Descr { get; set; }
        public string Descr2 { get; set; }
        public bool isDisable { get; set; }
        public string Exp_Code { get; set; }
        public bool isOnRatePage { get; set; }
        public bool isExist { get; set; }
    }
}
