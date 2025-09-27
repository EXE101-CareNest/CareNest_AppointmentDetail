# Ví dụ sử dụng CareNest Service Template

## 1. Ví dụ tạo một Service hoàn chỉnh

### Tạo Entity - Patient
```csharp
// CareNest_NewService.Domain/Entities/Patient.cs
public class Patient : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
}
```

### Tạo Commands
```csharp
// CareNest_NewService.Application/Features/Commands/Create/CreatePatientCommand.cs
public class CreatePatientCommand : ICommand<Patient>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
}

// CareNest_NewService.Application/Features/Commands/Create/CreatePatientCommandHandler.cs
public class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, Patient>
{
    private readonly IGenericRepository<Patient> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePatientCommandHandler(IGenericRepository<Patient> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Patient> HandleAsync(CreatePatientCommand command)
    {
        var patient = new Patient
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            DateOfBirth = command.DateOfBirth,
            Address = command.Address
        };

        await _repository.AddAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        return patient;
    }
}
```

### Tạo Queries
```csharp
// CareNest_NewService.Application/Features/Queries/GetAll/GetAllPatientsQuery.cs
public class GetAllPatientsQuery : IQuery<IEnumerable<Patient>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// CareNest_NewService.Application/Features/Queries/GetAll/GetAllPatientsQueryHandler.cs
public class GetAllPatientsQueryHandler : IQueryHandler<GetAllPatientsQuery, IEnumerable<Patient>>
{
    private readonly IGenericRepository<Patient> _repository;

    public GetAllPatientsQueryHandler(IGenericRepository<Patient> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Patient>> HandleAsync(GetAllPatientsQuery query)
    {
        // Implement pagination logic here
        return await _repository.GetAllAsync();
    }
}
```

### Tạo Controller
```csharp
// CareNest_NewService.API/Controllers/PatientsController.cs
[ApiController]
[Route("api/[controller]")]
public class PatientsController : BaseController
{
    private readonly IUseCaseDispatcher _dispatcher;

    public PatientsController(IUseCaseDispatcher dispatcher, ICurrentUserService currentUserService) 
        : base(currentUserService)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
    {
        try
        {
            var command = new CreatePatientCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address
            };

            var result = await _dispatcher.DispatchAsync(command);
            return this.OkResponse(result, "Tạo bệnh nhân thành công");
        }
        catch (Exception ex)
        {
            return this.ErrorResponse<object>(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetAllPatientsQuery 
            { 
                PageNumber = pageNumber, 
                PageSize = pageSize 
            };
            var result = await _dispatcher.DispatchAsync(query);
            return this.OkResponse(result, "Lấy danh sách bệnh nhân thành công");
        }
        catch (Exception ex)
        {
            return this.ErrorResponse<object>(ex.Message);
        }
    }
}
```

## 2. Ví dụ kết nối với service khác

### Tạo Service để gọi Appointment API
```csharp
// CareNest_NewService.Infrastructure/Services/AppointmentService.cs
public class AppointmentService : IAppointmentService
{
    private readonly IAPIService _apiService;

    public AppointmentService(IAPIService apiService)
    {
        _apiService = apiService;
    }

    public async Task<AppointmentResponse> GetAppointmentById(string id)
    {
        var result = await _apiService.GetAsync<AppointmentResponse>(
            "appointment", 
            $"/api/appointment/{id}"
        );

        if (!result.IsSuccess)
        {
            throw BaseException.BadRequestBadRequestResponse("Không tìm thấy lịch hẹn");
        }

        return result.Data!.Data!;
    }

    public async Task<IEnumerable<AppointmentResponse>> GetAppointmentsByPatientId(string patientId)
    {
        var result = await _apiService.GetAsync<IEnumerable<AppointmentResponse>>(
            "appointment", 
            $"/api/appointment/patient/{patientId}"
        );

        if (!result.IsSuccess)
        {
            throw BaseException.BadRequestBadRequestResponse("Không tìm thấy lịch hẹn");
        }

        return result.Data!.Data!;
    }
}
```

### Interface cho AppointmentService
```csharp
// CareNest_NewService.Application/Interfaces/Services/IAppointmentService.cs
public interface IAppointmentService
{
    Task<AppointmentResponse> GetAppointmentById(string id);
    Task<IEnumerable<AppointmentResponse>> GetAppointmentsByPatientId(string patientId);
}
```

### Sử dụng trong Controller
```csharp
[HttpGet("{patientId}/appointments")]
public async Task<IActionResult> GetPatientAppointments(Guid patientId)
{
    try
    {
        var appointments = await _appointmentService.GetAppointmentsByPatientId(patientId.ToString());
        return this.OkResponse(appointments, "Lấy lịch hẹn của bệnh nhân thành công");
    }
    catch (Exception ex)
    {
        return this.ErrorResponse<object>(ex.Message);
    }
}
```

## 3. Ví dụ sử dụng CurrentUserService

```csharp
public class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, Patient>
{
    private readonly IGenericRepository<Patient> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreatePatientCommandHandler(
        IGenericRepository<Patient> repository, 
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Patient> HandleAsync(CreatePatientCommand command)
    {
        var patient = new Patient
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            DateOfBirth = command.DateOfBirth,
            Address = command.Address,
            CreatedBy = _currentUserService.UserId // Lưu thông tin người tạo
        };

        await _repository.AddAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        return patient;
    }
}
```

## 4. Ví dụ sử dụng PageResult

```csharp
// CareNest_NewService.Application/Features/Queries/GetAllPaging/GetAllPatientsPagingQuery.cs
public class GetAllPatientsPagingQuery : IQuery<PageResult<Patient>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

// CareNest_NewService.Application/Features/Queries/GetAllPaging/GetAllPatientsPagingQueryHandler.cs
public class GetAllPatientsPagingQueryHandler : IQueryHandler<GetAllPatientsPagingQuery, PageResult<Patient>>
{
    private readonly DatabaseContext _context;

    public GetAllPatientsPagingQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<PageResult<Patient>> HandleAsync(GetAllPatientsPagingQuery query)
    {
        var patientsQuery = _context.Patients.AsQueryable();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            patientsQuery = patientsQuery.Where(p => 
                p.FirstName.Contains(query.SearchTerm) || 
                p.LastName.Contains(query.SearchTerm) ||
                p.Email.Contains(query.SearchTerm));
        }

        var totalItems = await patientsQuery.CountAsync();
        
        var patients = await patientsQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<Patient>(patients, totalItems, query.PageNumber, query.PageSize);
    }
}
```

## 5. Ví dụ sử dụng trong Controller với Paging

```csharp
[HttpGet("paging")]
public async Task<IActionResult> GetAllPaging(
    [FromQuery] int pageNumber = 1, 
    [FromQuery] int pageSize = 10,
    [FromQuery] string? searchTerm = null)
{
    try
    {
        var query = new GetAllPatientsPagingQuery 
        { 
            PageNumber = pageNumber, 
            PageSize = pageSize,
            SearchTerm = searchTerm
        };
        var result = await _dispatcher.DispatchAsync(query);
        return this.OkResponse(result, "Lấy danh sách bệnh nhân thành công");
    }
    catch (Exception ex)
    {
        return this.ErrorResponse<object>(ex.Message);
    }
}
```

## 6. Ví dụ Response với Paging

```json
{
  "success": true,
  "message": "Lấy danh sách bệnh nhân thành công",
  "data": {
    "items": [
      {
        "id": "guid",
        "firstName": "John",
        "lastName": "Doe",
        "email": "john@example.com"
      }
    ],
    "totalItems": 100,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 10
  }
}
```

## 7. Ví dụ Error Handling

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientRequest request)
{
    try
    {
        var command = new UpdatePatientCommand
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address
        };

        var result = await _dispatcher.DispatchAsync(command);
        return this.OkResponse(result, "Cập nhật bệnh nhân thành công");
    }
    catch (BaseException.ErrorException ex)
    {
        // Exception này sẽ được GlobalExceptionHandlingMiddleware xử lý
        throw;
    }
    catch (Exception ex)
    {
        return this.ErrorResponse<object>($"Lỗi không xác định: {ex.Message}");
    }
}
```

## 8. Ví dụ Docker Compose với nhiều services

```yaml
services:
  newserviceapi:
    image: new-service-api-dev:latest
    restart: always
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8015:8080"
    container_name: new-service-api-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    networks:
      carenest-network:
        ipv4_address: 192.168.0.12
    depends_on:
      - postgres

  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: new-service-dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: HoiLamChi123@
    ports:
      - "5433:5432"
    networks:
      carenest-network:
        ipv4_address: 192.168.0.20

networks:
  carenest-network:
    external: true
```

Đây là các ví dụ chi tiết về cách sử dụng template để tạo một service hoàn chỉnh trong hệ thống CareNest.
