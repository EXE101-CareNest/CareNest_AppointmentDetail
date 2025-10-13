
using CareNest_AppointmentDetail.Application.Common;
using CareNest_AppointmentDetail.Application.Features.Commands.Create;
using CareNest_AppointmentDetail.Application.Features.Commands.Delete;
using CareNest_AppointmentDetail.Application.Features.Commands.Update;
using CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging;
using CareNest_AppointmentDetail.Application.Features.Queries.GetById;
using CareNest_AppointmentDetail.Application.Features.Queries.GetTotalAmount;
using CareNest_AppointmentDetail.Application.Features.Queries.Dashboard;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS;
using CareNest_AppointmentDetail.Domain.Commons.Constant;
using CareNest_AppointmentDetail.Extensions;
using Microsoft.AspNetCore.Mvc;


namespace CareNest_AppointmentDetail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentDetailController : ControllerBase
    {
        private readonly IUseCaseDispatcher _dispatcher;

        public AppointmentDetailController(IUseCaseDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Hiển thị toàn bộ danh sách chi tiết cuộc hẹn hiện có trong hệ thống với phân trang và sắp xếp
        /// </summary>
        /// <param name="pageIndex">trang hiện tại</param>
        /// <param name="pageSize">Số lượng phần tử trong trang</param>
        /// <param name="sortColumn">cột muốn sort: name, updateat,ownerid</param>
        /// <param name="sortDirection">cách sort asc or desc</param>
        /// <returns>Danh sách chi tiết cuộc hẹn</returns>
        [HttpGet]
        public async Task<IActionResult> GetPaging(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortColumn = null,
            [FromQuery] string? sortDirection = "asc",
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetAllPagingQuery()
            {
                Index = pageIndex,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SearchTerm = searchTerm,
                SortDirection = sortDirection
            };
            var result = await _dispatcher.DispatchQueryAsync<GetAllPagingQuery, PageResult<AppointmentDetailResponse>>(query);
            return this.OkResponse(result, MessageConstant.SuccessGet);
        }

        /// <summary>
        /// Dashboard: Top service/service detail theo số lượt đặt, filter theo CreatedAt
        /// </summary>
        /// <param name="fromDate">lọc từ ngày (UTC hoặc local theo hệ thống)</param>
        /// <param name="toDate">lọc đến ngày</param>
        /// <param name="top">số lượng top cần trả (mặc định 10)</param>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null, [FromQuery] int top = 10, [FromQuery] string? appointmentId = null)
        {
            var query = new GetDashboardStatsQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Top = top <= 0 ? 10 : top,
                AppointmentId = appointmentId
            };

            var result = await _dispatcher.DispatchQueryAsync<GetDashboardStatsQuery, GetDashboardStatsResponse>(query);
            return this.OkResponse(result, "Lấy dashboard thành công");
        }

        /// <summary>
        /// Hiển thị chi tiết chi tiết cuộc hẹn theo id
        /// </summary>
        /// <param name="id">Id chi tiết cuộc hẹn</param>
        /// <returns>chi tiết chi tiết cuộc hẹn</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new GetByIdQuery() { Id = id };
            AppointmentDetailResponse result = await _dispatcher.DispatchQueryAsync<GetByIdQuery, AppointmentDetailResponse>(query);
            return this.OkResponse(result, MessageConstant.SuccessGet);
        }

        /// <summary>
        /// tạo mới chi tiết cuộc hẹn
        /// </summary>
        /// <param name="command">thông tin chi tiết cuộc hẹn</param>
        /// <returns>thông tin chi tiết cuộc hẹn mới tạo xog</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommand command)
        {
            AppointmentDetailResponse result = await _dispatcher.DispatchAsync<CreateCommand, AppointmentDetailResponse>(command);

            return this.OkResponse(result, MessageConstant.SuccessCreate);
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết cuộc hẹn
        /// </summary>
        /// <param name="id">Id chi tiết cuộc hẹn</param>
        /// <param name="request">các thông tin cần sửa</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRequest request)
        {

            var command = new UpdateCommand()
            {
                Id = id,
                Note = request.Note,
                AppointmentId = request.AppointmentId,
                PetQuantity = request.PetQuantity,
                ServiceDetailId = request.ServiceDetailId
            };
            AppointmentDetailResponse result = await _dispatcher.DispatchAsync<UpdateCommand, AppointmentDetailResponse>(command);

            return this.OkResponse(result, MessageConstant.SuccessUpdate);
        }

        /// <summary>
        /// xoá chi tiết cuộc hẹn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _dispatcher.DispatchAsync(new DeleteCommand { Id = id });
            return this.OkResponse(MessageConstant.SuccessDelete);
        }

        /// <summary>
        /// Tính tổng tiền của tất cả appointment detail có cùng appointment id
        /// </summary>
        /// <param name="appointmentId">Id của cuộc hẹn cần tính tổng tiền</param>
        /// <returns>Tổng tiền và số lượng appointment detail</returns>
        [HttpGet("total-amount/{appointmentId}")]
        public async Task<IActionResult> GetTotalAmountByAppointmentId(string appointmentId)
        {
            var query = new GetTotalAmountByAppointmentIdQuery(appointmentId);
            var result = await _dispatcher.DispatchQueryAsync<GetTotalAmountByAppointmentIdQuery, TotalAmountResponse>(query);

            return this.OkResponse(result, "Tính tổng tiền thành công");
        }
    }
}
