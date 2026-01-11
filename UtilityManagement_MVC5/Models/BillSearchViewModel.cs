using System;

namespace UtilityManagement_MVC5.Models
{
    public class BillSearchViewModel
    {
        public int BillID { get; set; }
        public string CustomerName { get; set; }
        public string Utility { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
    }
}