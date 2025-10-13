
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
        public string? ServiceDetailName { get; set; } // FK
        public string? ServiceName { get; set; } // tên dịch vụ cha

        // Main Properties
        public int TotalAmount { get; set; } // tổng tiền
        public string? Note { get; set; } // ghi chú
        public int PetQuantity { get; set; } // số lượng thú cưng
        public DateTime CreatedAt { get; set; }
    }
}
