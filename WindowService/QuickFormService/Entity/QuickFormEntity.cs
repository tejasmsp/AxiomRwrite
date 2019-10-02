using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFormService.Entity
{
    public class QuickFormRecordEntity
    {
        public int QuickFormID { get; set; }
        public int OrderNo { get; set; }
        public string PartNo { get; set; }
        public string DocName { get; set; }
        public string DocPath { get; set; }
        public int Status { get; set; }
        public int FileTypeID { get; set; }
        public int RecordTypeID { get; set; }
        public int Pages { get; set; }
        public bool IsPublic { get; set; }
        public string UserId { get; set; }
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string Year4 { get; set; }
        public string Year5 { get; set; }
        public string Year6 { get; set; }
        public string Year7 { get; set; }
        public string Year8 { get; set; }
        public int IsPrint { get; set; }
        public int printStatus { get; set; }
        public int IsFax { get; set; }
        public int faxStatus { get; set; }
        public int IsEmail { get; set; }
        public int EmailStatus { get; set; }
        public string Email { get; set; }
        public string FaxNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsSSN { get; set; }
        public string IsRevised { get; set; }
        public bool isFromClient { get; set; }
        public bool CreateDocument { get; set; }
        public string OrdBy { get; set; }
        public string ClaimNo { get; set; }
        public string BillFirm { get; set; }
        public string AttyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
