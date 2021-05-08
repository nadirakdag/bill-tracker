using System;

namespace Domain
{
    public class Bill
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime PaidDate { get; set; }
    }

    public enum BillStatus
    {
        Unpaid,
        Paid
    }
}