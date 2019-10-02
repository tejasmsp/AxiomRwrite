using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFormService.Entity
{
    public class OrderDetail
    {
        public string Caption { get; set; }
        public string CauseNo { get; set; }
        public string PatientName { get; set; }
        public string OrderingAttorney { get; set; }
        public string acctrep { get; set; }
    }
    public class AccntRepDetail
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class FileObject
    {
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public Guid UserId { get; set; }
        public int Pages { get; set; }
        public int RecordType { get; set; }
        public bool IsPublic { get; set; }
        public int FileType { get; set; }
        public string FileName { get; set; }

    }
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
