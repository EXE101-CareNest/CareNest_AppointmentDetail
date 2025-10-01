using CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Domain.Entitites;


namespace CareNest_AppointmentDetail.Application.Features.Commands.Update
{
    public class UpdateCommand : ICommand<AppointmentDetailResponse>
    {
        public string Id { get; set; } = string.Empty;
        // Foreign Keys
        public string? AppointmentId { get; set; } // FK
        public string? ServiceDetailId { get; set; } // FK

        // Main Properties
        public int TotalAmount { get; set; } // tổng tiền
        public string? Note { get; set; } // ghi chú
        public int PetQuantity { get; set; } // số lượng thú cưng
    }
}
