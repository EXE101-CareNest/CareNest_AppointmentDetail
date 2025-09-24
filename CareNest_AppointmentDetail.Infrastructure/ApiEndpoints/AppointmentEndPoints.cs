namespace CareNest_AppointmentDetail.Infrastructure.Common.ApiEndpoints
{
    public class AppointmentEndPoints
    {
        public static string GetPaging() => "api/appointments/";
        public static string GetById(string id) => $"api/appointments/{id}";
    }
}
