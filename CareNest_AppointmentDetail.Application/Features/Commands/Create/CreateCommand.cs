using CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Create
{
    public class CreateCommand : ICommand<AppointmentDetailResponse>
    {
        // Foreign Keys
        public string? AppointmentId { get; set; } // FK
        public string? ServiceDetailId { get; set; } // FK

        // Main Properties
        public string? Note { get; set; } // ghi chú
        public int PetQuantity { get; set; } // số lượng thú cưng
    }
}
