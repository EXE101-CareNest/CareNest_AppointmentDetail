namespace Shared.Contracts
{
    public class AppointmentResponse
    {
        public string? CustomerId { get; set; }
        public string? ShopId { get; set; }
        public double TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; } // kiểu int giống JSON
        public DateTime StartTime { get; set; }  // nên dùng DateTime để parse chính xác ISO8601
        public string? StaffName { get; set; }
        public string? BankId { get; set; }
        public string? BankTransactionId { get; set; }
        public bool IsPaid { get; set; }

        public string? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }

}
