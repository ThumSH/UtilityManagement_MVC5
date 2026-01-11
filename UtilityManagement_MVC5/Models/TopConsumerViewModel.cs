namespace UtilityManagement_MVC5.Models
{
    public class TopConsumerViewModel
    {
        public string CustomerName { get; set; }
        public string Utility { get; set; }
        public decimal TotalUsage { get; set; }
        public decimal TotalSpent { get; set; }
    }
}