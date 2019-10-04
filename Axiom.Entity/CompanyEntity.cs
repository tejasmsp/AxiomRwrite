using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axiom.Entity
{
    public class CompanyEntity
    {
        public Int16 CompNo { get; set; }
        public string CompID { get; set; }
        public string CompName { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string AreaCode1 { get; set; }
        public string PhoneNo { get; set; }
        public string AreaCode2 { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string TaxID { get; set; }
        public Int16 RemitNo { get; set; }
        public int ChkNo { get; set; }
        public int ARNo { get; set; }
        public int RefundNo { get; set; }
        public int FCNo { get; set; }
        public int DcntNo { get; set; }
        public int DebtNo { get; set; }
        public int PayrollNo { get; set; }
        public Int16 LateDays { get; set; }
        public byte OnInv { get; set; }
        public byte OnStmt { get; set; }
        public byte ShowPage { get; set; }
        public Int16 DueDays { get; set; }
        public byte PreprtInv { get; set; }
        public byte PreprtStmt { get; set; }
        public int IncomeNo { get; set; }
        public string Term { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public string SiteURL { get; set; }
        public string Style { get; set; }
    }

}
