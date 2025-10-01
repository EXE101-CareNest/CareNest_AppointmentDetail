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
        /// <summary>
        /// giá
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// giảm giá (%)
        /// </summary>
        public double Discount { get; set; }

    }
}
