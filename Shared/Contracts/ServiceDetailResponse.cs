namespace Shared.Contracts
{
    public class ServiceDetailResponse
    {
        /// <summary>
        /// Id chi tiết
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// tên chi tiết
        /// </summary>
        public string? Name { get; set; }
        public string? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        /// <summary>
        /// giá
        /// </summary>
        public int? Price { get; set; }
        /// <summary>
        /// giảm giá (%)
        /// </summary>
        public double? Discount { get; set; }

    }

    public class ServiceDetailBatchResponse
    {
        public List<ServiceDetailResponse> Items { get; set; } = new List<ServiceDetailResponse>();
        public List<string> NotFoundIds { get; set; } = new List<string>();
        public int Total { get; set; }
    }
}
