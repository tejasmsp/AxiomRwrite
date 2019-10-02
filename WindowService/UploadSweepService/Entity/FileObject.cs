using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadSweepService.Entity
{
    public class FileObject
    {
        public int OrderNo;
        public int PartNo;
        public Guid UserId;
        public int Pages;
        public int RecordType;
        public bool IsPublic;
        public int FileType;
        public string FileName;

    }
}
