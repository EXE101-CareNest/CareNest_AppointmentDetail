
namespace CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging
{
    public class AppointmentDetailResponse
    {
        /// <summary>
        /// Id cuộc hẹn 
        /// </summary>
        public string? Id { get; set; }
        // Foreign Keys
        public string? AppointmentId { get; set; } // FK
        public string? ServiceDetailId { get; set; } // FK

        // Main Properties
        public int TotalAmount { get; set; } // tổng tiền
        public string? Note { get; set; } // ghi chú
        public int PetQuantity { get; set; } // số lượng thú cưng
    }
}
