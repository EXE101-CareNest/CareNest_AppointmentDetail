namespace CareNest_AppointmentDetail.Infrastructure.Common.ApiEndpoints
{
    public class AppointmentEndPoints
    {
        public static string GetPaging() => "/api/appointment/";
        public static string GetById(string? id) => $"/api/appointment/{id}";
        public static string UpdateTotalAmount(string id) => $"/api/appointment/{id}/total-amount";
    }
}
