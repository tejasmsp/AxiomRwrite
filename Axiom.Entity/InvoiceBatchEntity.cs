using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
   public class InvoiceBatchEntity
    {
        public string FirmID { get; set; }
        public string Caption { get; set; }
        public string ClaimNo { get; set; }
        public string InvoiceNO { get; set; }
        public string AttyID { get; set; }
        public string SoldAttyID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? Invoice { get; set; }
        public bool? Statement { get; set; }
        public bool? OpenInvoiceOnly { get; set; }
    }
}
