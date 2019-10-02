using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class OrderListEntity
    {
        public int TotalRecords { get; set; }
        public string OrderId { get; set; }
        public string RecordsOf { get; set; }
        public string ClientMatterNo { get; set; }
        public string BillingClaimNo { get; set; }
        public string Caption { get; set; }
        public string CauseNo { get; set; }
        public bool? IsArchive { get; set; }
        public bool? SubmitStatus { get; set; }
        public int CurrentStepID { get; set; }
        public string OrderDateStr { get; set; }
        public string BillingID { get; set; }
        public bool CanAddPart { get; set; }

        public string OrderingFirmID { get; set; }
        public string OrderingFirmName { get; set; }
        public string OrderingAttorney { get; set; }
        public string OrderingAttorneyName { get; set; }

        public string BillingFirmId { get; set; }
        public string BillingFirmName { get; set; }
        public string BillingAttorney { get; set; }
        public string BillingAttorneyName { get; set; }

    }
    public class BasicOrderInformation
    {
        public Int64 OrderNo { get; set; }
        public string PatientName { get; set; }
        public string SSN { get; set; }
        public string DateOfBirth { get; set; }
        public string DateOfDeath { get; set; }
        public string DateOfLoss { get; set; }
        public int CompanyNo { get; set; }
    }
    public class SearchListEntity
    {
        public int TotalRecords { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string LocId { get; set; }
        public string Name1 { get; set; }
        public string RecordType { get; set; }
        public string Patient { get; set; }
        public int? Rush { get; set; }
        public string InternalStatus { get; set; }
        public string StatusClass { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToEmail { get; set; }
        public string PlaintiffAtty { get; set; }
        public string PlaintiffEmail { get; set; }
        public string ActionDate { get; set; }
    }

}
