using System;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement_MVC5.Models
{
    public class PaymentViewModel
    {
        [Required]
        public int BillID { get; set; }

        [Required]
        public decimal AmountPaid { get; set; }

        public string PaymentMethod { get; set; }
    }
}