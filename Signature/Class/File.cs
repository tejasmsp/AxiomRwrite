using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Viewer.Class
{
    public class FileEntity
    {
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string FileName { get; set; }
        public int? FileTypeId { get; set; }
        public bool? IsPublic { get; set; }
        public int? RecordTypeId { get; set; }
        public string FileDiskName { get; set; }
        public int PageNo { get; set; }
        public Guid CreatedBy { get; set; }
    }
}