using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Axiom.Entity
{
    public class QuickFormEntity
    {
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
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public List<QuickFormFileEntity> documentFileList { get; set; }

    }
    public class QuickFormFileEntity
    {
        public string FileName { get; set; }
        public string FileDiskName { get; set; }
        public string BatchId { get; set; }
        public int UploadType { get; set; }
        public bool IsSSN { get; set; }
        public int QuickFormID { get; set; }
        public int OrderNo { get; set; }
        public string PartNo { get; set; }
        public string DocName { get; set; }
        public string DocPath { get; set; }
        public int? FileTypeId { get; set; }
        public int? RecordTypeID { get; set; }
        public int? Pages { get; set; }
        public bool? IsPublic { get; set; }
        public string UserID { get; set; }
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string Year4 { get; set; }
        public string Year5 { get; set; }
        public string Year6 { get; set; }
        public string Year7 { get; set; }
        public string Year8 { get; set; }
        public string CreatedBy { get; set; }
    }


    public class QuickFormPartDetailEntity
    {
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string LocID { get; set; }
        public byte? RecType { get; set; }
        public string LocationName { get; set; }
        public string RecordType { get; set; }
        public string BillingClaimNo { get; set; }
        public string BillingFirmId { get; set; }
        public string OrderingAttorney { get; set; }
        public string AttyName { get; set; }
        public string SendRequest { get; set; }
        public string Email { get; set; }
    }

    public class QuickFormDocumentListEntity
    {
        //public int ConfigID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string PartNo { get; set; }
    }

    public class QuickFormGetFileListEntity
    {
        public int FileTypeId { get; set; }
        public string FileType { get; set; }

    }

    public class QuickFormAttorneyListEntity
    {
        public string AttyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AttorneyType { get; set; }
        public string Represent { get; set; }
        public string FaxNo { get; set; }

    }

    public class QuickFormDocumentAttachmentListEntity
    {
        public int FileId { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string FileName { get; set; }
        public int FileTypeId { get; set; }
        public string FileType { get; set; }
        public int RecordTypeId { get; set; }
        public string Descr { get; set; }
        public string FileDiskName { get; set; }
    }
    public class QuickFormUploadDocumentEntity
    {
        public string batchId { get; set; }
        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public long size { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class QuickFormDocument
    {
        public string DocFileName { get; set; }
        public string FolderPath { get; set; }
    }
    public class QuickFormOrderDetail
    {
        public string State { get; set; }
        public string Email { get; set; }
        public string BillingClaimNo { get; set; }
        public string SSN { get; set; }
        public string BillingFirmId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AttyID { get; set; }
        public string CauseNo { get; set; }
        public string Caption { get; set; }
        public string PatientName { get; set; }
        public string AcctRep { get; set; }
        public string OrderingAttorney { get; set; }

    }
    public class QueriesEntity
    {
        public string Query { get; set; }
        public string SubQuery { get; set; }
    }
    public class QuickDocument
    {        
        public int OrderNo { get; set; }
        public string Years { get; set; }
        public int Pages { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public string Fullpath { get; set; }
        public int FileTypeId { get; set; }
        public int? RecordTypeId { get; set; }
        public bool? isPublic { get; set; }
        public bool EnableDocStorage { get; set; }
        public string PartIds { get; set; }
        public string AttysIds { get; set; }
        public bool SSN { get; set; }
        public string IsRevised { get; set; }
        public Guid UserId { get; set; }
        public string EmpId { get; set; }
    }
}
