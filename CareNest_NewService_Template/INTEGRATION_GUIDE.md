# Hướng dẫn tích hợp và sử dụng CareNest Service Template

## 1. Tổng quan kiến trúc

### Clean Architecture với 4 layers:
- **API Layer**: Controllers, Middleware, Extensions
- **Application Layer**: Use Cases, Interfaces, Common classes
- **Domain Layer**: Entities, Repositories, Business logic
- **Infrastructure Layer**: Database, External services, Implementations

### Các pattern được sử dụng:
- **CQRS**: Tách biệt Commands và Queries
- **Repository Pattern**: Abstraction cho data access
- **Unit of Work**: Quản lý transactions
- **Dependency Injection**: .NET Core DI container

## 2. Cách sử dụng APIService

### Cấu hình trong appsettings.json:
```json
{
  "APIService": {
    "BaseUrlAppointment": "http://192.168.0.10:8080",
    "BaseUrlAppointmentDetail": "http://192.168.0.11:8080"
  }
}
```

### Sử dụng trong service:
```csharp
public class YourService
{
    private readonly IAPIService _apiService;

    public YourService(IAPIService apiService)
    {
        _apiService = apiService;
    }

    public async Task<AppointmentResponse> GetAppointment(string id)
    {
        var result = await _apiService.GetAsync<AppointmentResponse>(
            "appointment", 
            $"/api/appointment/{id}"
        );

        if (!result.IsSuccess)
        {
            throw new Exception(result.Message);
        }

        return result.Data!.Data!;
    }
}
```

## 3. Cách sử dụng ApiResponse

### Trong Controller:
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    try
    {
        var result = await _yourService.GetById(id);
        return this.OkResponse(result, "Lấy dữ liệu thành công");
    }
    catch (Exception ex)
    {
        return this.ErrorResponse<object>(ex.Message);
    }
}
```

### Response format:
```json
{
  "success": true,
  "message": "Lấy dữ liệu thành công",
  "data": {
    "id": "guid",
    "name": "value"
  }
}
```

## 4. Cách tạo Entity mới

### 1. Tạo Entity trong Domain:
```csharp
// CareNest_NewService.Domain/Entities/YourEntity.cs
public class YourEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

### 2. Thêm vào DatabaseContext:
```csharp
// CareNest_NewService.Infrastructure/Persistences/Database/DatabaseContext.cs
public DbSet<YourEntity> YourEntities { get; set; }
```

### 3. Tạo Migration:
```bash
dotnet ef migrations add AddYourEntity
dotnet ef database update
```

## 5. Cách tạo CQRS Commands/Queries

### Command Example:
```csharp
// Application/Features/Commands/Create/CreateCommand.cs
public class CreateCommand : ICommand<YourEntity>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// Application/Features/Commands/Create/CreateCommandHandler.cs
public class CreateCommandHandler : ICommandHandler<CreateCommand, YourEntity>
{
    private readonly IGenericRepository<YourEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommandHandler(IGenericRepository<YourEntity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<YourEntity> HandleAsync(CreateCommand command)
    {
        var entity = new YourEntity
        {
            Name = command.Name,
            Description = command.Description
        };

        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
```

### Query Example:
```csharp
// Application/Features/Queries/GetById/GetByIdQuery.cs
public class GetByIdQuery : IQuery<YourEntity>
{
    public Guid Id { get; set; }
}

// Application/Features/Queries/GetById/GetByIdQueryHandler.cs
public class GetByIdQueryHandler : IQueryHandler<GetByIdQuery, YourEntity>
{
    private readonly IGenericRepository<YourEntity> _repository;

    public GetByIdQueryHandler(IGenericRepository<YourEntity> repository)
    {
        _repository = repository;
    }

    public async Task<YourEntity> HandleAsync(GetByIdQuery query)
    {
        var entity = await _repository.GetByIdAsync(query.Id);
        if (entity == null)
        {
            throw BaseException.NotFoundResponse("Entity not found");
        }
        return entity;
    }
}
```

## 6. Cách đăng ký Services trong Program.cs

```csharp
// Đăng ký Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Đăng ký Command/Query Handlers
builder.Services.AddScoped<ICommandHandler<CreateCommand, YourEntity>, CreateCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetByIdQuery, YourEntity>, GetByIdQueryHandler>();

// Đăng ký Use Case Dispatcher
builder.Services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();

// Đăng ký MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

## 7. Cách tạo Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class YourController : BaseController
{
    private readonly IUseCaseDispatcher _dispatcher;

    public YourController(IUseCaseDispatcher dispatcher, ICurrentUserService currentUserService) 
        : base(currentUserService)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        try
        {
            var command = new CreateCommand
            {
                Name = request.Name,
                Description = request.Description
            };

            var result = await _dispatcher.DispatchAsync(command);
            return this.OkResponse(result, "Tạo thành công");
        }
        catch (Exception ex)
        {
            return this.ErrorResponse<object>(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetByIdQuery { Id = id };
            var result = await _dispatcher.DispatchAsync(query);
            return this.OkResponse(result, "Lấy dữ liệu thành công");
        }
        catch (Exception ex)
        {
            return this.ErrorResponse<object>(ex.Message);
        }
    }
}
```

## 8. Cách kết nối với các service khác

### 1. Thêm URL vào APIServiceOption:
```csharp
public class APIServiceOption
{
    public string BaseUrlAppointment { get; set; } = string.Empty;
    public string BaseUrlAppointmentDetail { get; set; } = string.Empty;
    public string BaseUrlYourNewService { get; set; } = string.Empty; // Thêm mới
}
```

### 2. Cập nhật APIService:
```csharp
public string GetBaseUrl(string serviceType)
{
    return serviceType.ToLower() switch
    {
        "appointment" => _option.BaseUrlAppointment,
        "appointmentdetail" => _option.BaseUrlAppointmentDetail,
        "yournewservice" => _option.BaseUrlYourNewService, // Thêm mới
        _ => throw new ArgumentException($"Service type '{serviceType}' không hợp lệ!", nameof(serviceType))
    };
}
```

### 3. Sử dụng:
```csharp
var result = await _apiService.GetAsync<YourResponseType>(
    "yournewservice", 
    "/api/your-endpoint"
);
```

## 9. Docker Configuration

### 1. Cập nhật docker-compose.yml:
```yaml
services:
  yournewserviceapi:
    image: your-new-service-api-dev:latest
    restart: always
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8016:8080"  # Port mới
    container_name: your-new-service-api-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    networks:
      carenest-network:
        ipv4_address: 192.168.0.13  # IP mới
```

### 2. Cập nhật appsettings.Development.json:
```json
{
  "DatabaseSettings": {
    "Ip": "100.93.191.32",
    "Port": 5432,
    "User": "postgres",
    "Password": "HoiLamChi123@",
    "Database": "your-new-service-dev"
  },
  "APIService": {
    "BaseUrlAppointment": "http://192.168.0.10:8080",
    "BaseUrlAppointmentDetail": "http://192.168.0.11:8080",
    "BaseUrlYourNewService": "http://192.168.0.12:8080"
  }
}
```

## 10. Commands để chạy

```bash
# Build và chạy
docker-compose up --build

# Chạy với override
docker-compose -f docker-compose.yml -f docker-compose.override.yml up

# Tạo migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Chạy tests
dotnet test
```

## 11. Best Practices

1. **Luôn sử dụng BaseController** cho các controller
2. **Sử dụng CQRS pattern** cho business logic
3. **Handle exceptions** với GlobalExceptionHandlingMiddleware
4. **Sử dụng ApiResponse** cho consistent response format
5. **Logging** với ILogger
6. **Validation** với FluentValidation (có thể thêm)
7. **Unit tests** cho business logic
8. **Integration tests** cho API endpoints

## 12. Troubleshooting

### Lỗi kết nối database:
- Kiểm tra connection string
- Đảm bảo PostgreSQL đang chạy
- Kiểm tra network connectivity

### Lỗi kết nối service:
- Kiểm tra IP addresses trong docker network
- Đảm bảo các service đang chạy
- Kiểm tra firewall settings

### Lỗi migration:
- Xóa migrations cũ nếu cần
- Kiểm tra connection string
- Đảm bảo database tồn tại
