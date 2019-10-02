using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadSweepService.Entity
{
    public class FilesToPartEntity
    {
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string FileName { get; set; }
        public int FileTypeId { get; set; }
        public int RecordTypeId { get; set; }
        public Guid UserId { get; set; }
        public bool IsPublic { get; set; }
        public int Pages { get; set; }
        public string FileDiskName { get; set; }

    }
}
