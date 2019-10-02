using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{

    public partial class ClientEntity
    {
        public string Department { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string EmpId { get; set; }
        public DateTime? OldestDate { get; set; }
        public int OldestDaysOld { get; set; }
        public DateTime? OldestDateRush { get; set; }
        public int OldestDaysOldRush { get; set; }
        public int TwoWeeksRush { get; set; }
        public int OneWeekRush { get; set; }
        public int TwoDaysRush { get; set; }
        public int CurrentRush { get; set; }
        public int TotalRush { get; set; }
        public int TwoWeeks { get; set; }
        public int OneWeek { get; set; }
        public int TwoDays { get; set; }
        public int Current { get; set; }
        public int Total { get; set; }
    }
    public partial class ClientPartListEntity
    {
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public int PartStatusGroupId { get; set; }
        public string RecordsOf { get; set; }
        public string CliMatNo { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string RecentNote { get; set; }
        public string ClassName { get; set; }
        public int OrderDays { get; set; }
    }

    public class ClientOrderListEntity
    {
        public Int64 OrderNo { get; set; }
        public int? PartStatusGroupId { get; set; }
        public int? PartNo { get; set; }
        public string RecordsOf { get; set; }
        public string CliMatNo { get; set; }
        public int? OrderDays { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string ClassName { get; set; }
        public string BillingClaimNo { get; set; }
    }

    public class ClientOrderPartListEntity
    {
        public string Location { get; set; }
        public string Type { get; set; }
        public int PartStatusGroupId { get; set; }
        public int OrderNo { get; set; }
        public int PartNo { get; set; }
        public string Note { get; set; }
        public string ClassName { get; set; }
    }
    public class ClientPartReport
    {
        public Int64 OrderNo { get; set; }
        public int PartNo { get; set; }
        public string Note { get; set; }
        public int PartStatusGroupId { get; set; }
        public int OrderDays { get; set; }
        public string RecordsOf { get; set; }
        public string ClaimMatterNo { get; set; }
        public string BillingClaimNo { get; set; }
        public string OrderPart { get; set; }
        public string Location { get; set; }

    }




}
