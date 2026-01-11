namespace UtilityManagement_MVC5.Models
{
    public class RevenueReportViewModel
    {
        public string UtilityType { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBills { get; set; }
    }
}