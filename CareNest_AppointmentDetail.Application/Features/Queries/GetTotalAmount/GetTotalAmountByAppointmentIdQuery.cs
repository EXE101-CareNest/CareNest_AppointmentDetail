using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;

namespace CareNest_AppointmentDetail.Application.Features.Queries.GetTotalAmount
{
    public class GetTotalAmountByAppointmentIdQuery : IQuery<TotalAmountResponse>
    {
        /// <summary>
        /// Id của cuộc hẹn cần tính tổng tiền
        /// </summary>
        public string AppointmentId { get; set; }

        public GetTotalAmountByAppointmentIdQuery(string appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}