using CareNest_AppointmentDetail.Domain.Commons.Base;

namespace CareNest_AppointmentDetail.Domain.Entitites
{
    public class AppointmentDetail : BaseEntity
    {
        // Foreign Keys
        public string? AppointmentId { get; set; } // FK
        public string? ServiceDetailId { get; set; } // FK

        // Main Properties
        public int TotalAmount { get; set; } // tổng tiền
        public string? Note { get; set; } // ghi chú
        public int PetQuantity { get; set; } // số lượng thú cưng
    }
}
