using CareNest_NewService.API.Extensions;
using CareNest_NewService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareNest_NewService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ICurrentUserService _currentUserService;

        protected BaseController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected string? CurrentUserId => _currentUserService.UserId;
        protected string? CurrentUserRole => _currentUserService.Role;
    }
}
