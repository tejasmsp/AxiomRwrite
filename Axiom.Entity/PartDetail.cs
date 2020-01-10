using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class PartDetail
    {
        public string LocID { get; set; }
        public bool isBatchUpload { get; set; }
        public int PartNo { get; set; }
        public bool? isCreateAuthSup { get; set; }
        public bool? isAuth { get; set; }
        public bool? isSup { get; set; }
        public byte? RecType { get; set; }
        public int OrderNo { get; set; }
        public DateTime? ChngDate { get; set; }
        public DateTime? CallBack { get; set; }
    }

    public class ProposalFeesEntity
    {
        public int? ProposalFeeID { get; set; }
        public int? OrderNo { get; set; }
        public int? PartNo { get; set; }
        public string Descr { get; set; }
        public int? Pages { get; set; }
        public decimal? Amount { get; set; }
        public decimal TotalFees { get; set; }
        public decimal? DocCap { get; set; }
        public string EntBy { get; set; }
        public byte? FeesStatus { get; set; }
        public int CompanyNo { get; set; }
    }

    public class OrderPartNoteEntity
    {
        public int? OrderId { get; set; }
        public int? PartNo { get; set; }
        public string NotesClient { get; set; }
        public string NotesInternal { get; set; }

        public DateTime? OrdDate { get; set; }
        public DateTime? NRDate { get; set; }
        public DateTime? EntDate { get; set; }
        public DateTime? HoldDate { get; set; }
        public DateTime? FirstCall { get; set; }
        public DateTime? CanDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CallBack { get; set; }
        public DateTime? AuthRecDate { get; set; }
        public DateTime? chkRmvCb { get; set; }
        public int? PartStatusId { get; set; }
        public string AssgnTo { get; set; }
        public string InternalStatusId { get; set; }
        public string InternalStatusText { get; set; }
        public string UserId { get; set; }
        public string PageFrom { get; set; }
        public string RoleName { get; set; }
        public List<OrderIdsPartIds> OrderIdPartIdList { get; set; }
    }

    public class ProposalOrderDetails
    {
        public string BillingAttorneyId { get; set; }
        public string OrderingAttorney { get; set; }
        public string PatientName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal? DocCapForRecords { get; set; }
        public decimal? DocCapForFilms { get; set; }
    }
    public class OrderIdsPartIds
    {
        public int OrderId { get; set; }
        public int PartNo { get; set; }
    }

}
