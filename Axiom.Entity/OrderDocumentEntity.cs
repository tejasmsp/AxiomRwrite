using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Axiom.Entity
{
    public class OrderDocumentEntity
    {
        public long rowNumber { get; set; }
        public long OrderDocumentId { get; set; }
        public long? OrderId { get; set; }
        public int? PartNo { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public int? FileTypeId { get; set; }
        public int? RecordTypeId { get; set; }
        public bool? IsPublic { get; set; }
        public string CreatedBy { get; set; }
        public int? UserAccessId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FileType { get; set; }
        public HttpPostedFileBase UploadedFiles { get; set; }
    }    
}
