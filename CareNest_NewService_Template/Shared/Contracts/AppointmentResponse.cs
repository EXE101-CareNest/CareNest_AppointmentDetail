namespace Shared.Contracts
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
