namespace CareNest_AppointmentDetail.Application.Features.Queries.GetTotalAmount
{
    public class TotalAmountResponse
    {
        /// <summary>
        /// Id cuộc hẹn
        /// </summary>
        public string AppointmentId { get; set; }

        /// <summary>
        /// Tổng tiền của tất cả appointment detail có cùng appointment id
        /// </summary>
        public int TotalAmount { get; set; }

        /// <summary>
        /// Số lượng appointment detail được tính
        /// </summary>
        public int Count { get; set; }

        public TotalAmountResponse(string appointmentId, int totalAmount, int count)
        {
            AppointmentId = appointmentId;
            TotalAmount = totalAmount;
            Count = count;
        }
    }
}