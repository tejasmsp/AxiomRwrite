using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
     
    public partial class HomeEntity
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

}
