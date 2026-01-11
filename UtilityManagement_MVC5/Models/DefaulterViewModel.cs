using System;

namespace UtilityManagement_MVC5.Models
{
    public class DefaulterViewModel
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string UtilityType { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}