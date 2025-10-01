namespace CareNest_AppointmentDetail.Infrastructure.ApiEndpoints
{
    public class SerivceDetaileEndpoint
    {
        public static string GetById(string? id) => $"/api/servicedetail/{id}";
    }
}
