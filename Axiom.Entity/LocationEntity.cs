using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{

    public partial class LocationEntity
    {
        public string LocID { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Dept { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo1 { get; set; }
        public string AreaCode2 { get; set; }
        public string PhoneNo2 { get; set; }
        public string AreaCode3 { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Notes { get; set; }
        public string Warning { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Specialty { get; set; }
        public byte ServBy { get; set; }
        public string HandServer { get; set; }
        public byte CommType { get; set; }
        public DateTime? ChngDate { get; set; }
        public string strChngDate { get; set; }
        public string ChngBy { get; set; }
        public DateTime EntDate { get; set; }
        public string strEntDate { get; set; }
        public string EntBy { get; set; }
        public byte[] TStamp { get; set; }
        public string FirmID { get; set; }
        public string FirmName { get; set; }
        public string SendRequest { get; set; }
        public string ReccanRequested { get; set; }
        public Nullable<bool> CopyService { get; set; }
        public Nullable<bool> FeeAmountSendRequest { get; set; }
        public Nullable<bool> ReqAuthorization { get; set; }
        public Nullable<int> AuthorizationDays { get; set; }
        public string LinkedLocation { get; set; }
        public Nullable<decimal> FeeAmount { get; set; }
        public Nullable<bool> LocSpecificAuthorization { get; set; }
        public string BillingLocId { get; set; }
        public string FilmsLocId { get; set; }
        public string BillingLocName { get; set; }
        public string FilmsLocName { get; set; }
        public string ReplacedBy { get; set; }
        public Nullable<bool> LinkRequest { get; set; }
        public Nullable<System.DateTime> LastUsedDate { get; set; }
        public string strLastUsedDate { get; set; }
        public Nullable<int> UsedRank { get; set; }
        public Nullable<bool> EnteredByClient { get; set; }
        public Nullable<bool> IsDisable { get; set; }
        public string Comment { get; set; }
        public Nullable<bool> isCommonLocation { get; set; }
        public Nullable<int> RequestSent { get; set; }
    }

    public partial class LocationList
    {
        public int TotalRecords { get; set; }
        public string LocID { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string ReplacedBy { get; set; }
        public string PhoneNo1 { get; set; }
        public string FaxNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }

    public class LocationForm
    {
        public int LocformID { get; set; }
        public string LocID { get; set; }
        public int FormID { get; set; }
        public string FolderPath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsRequestForm { get; set; }
        //Document tables columns
        public string FolderName { get; set; }
        public string DocFileName { get; set; }
    }

    public class FileDirectoryInfo
    {
        public FileDirectoryInfo()
        {
            children = new List<FileDirectoryInfo>();
        }
        public string title { get { return string.IsNullOrEmpty(fullpath) ? "" : fullpath.Substring(fullpath.LastIndexOf('\\')).Replace("\\", ""); } }
        public string fullpath { get; set; }
        public bool isfolder { get; set; }
        public bool isExpanded { get; set; }
        public string breadcrumbs { get; set; }
        public List<FileDirectoryInfo> children { get; set; }
    }

    public class SendRequestEntity
    {
        public string LocID { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo1 { get; set; }
        public string AreaCode2 { get; set; }
        public string PhoneNo2 { get; set; }
        public string AreaCode3 { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string SendRequest { get; set; }
    }
}
