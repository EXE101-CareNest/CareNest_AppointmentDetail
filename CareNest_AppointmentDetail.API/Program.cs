using CareNest_AppointmentDetail.API.Middleware;
using CareNest_AppointmentDetail.Application.Common;
using CareNest_AppointmentDetail.Application.Common.Options;
using CareNest_AppointmentDetail.Application.Features.Commands.Create;
using CareNest_AppointmentDetail.Application.Features.Commands.Delete;
using CareNest_AppointmentDetail.Application.Features.Commands.Update;
using CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging;
using CareNest_AppointmentDetail.Application.Features.Queries.GetById;
using CareNest_AppointmentDetail.Application.Features.Queries.GetTotalAmount;
using CareNest_AppointmentDetail.Application.Features.Queries.Dashboard;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Application.UseCases;
using CareNest_AppointmentDetail.Domain.Repositories;
using CareNest_AppointmentDetail.Infrastructure.Persistences.Configuration;
using CareNest_AppointmentDetail.Infrastructure.Persistences.Database;
using CareNest_AppointmentDetail.Infrastructure.Persistences.Repository;
using CareNest_AppointmentDetail.Infrastructure.Services;
using CareNest_AppointmentDetail.Infrastructure.UOW;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Lấy DatabaseSettings theo ENV ưu tiên, fallback về appsettings (chuẩn cloud)
var config = builder.Configuration;
string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") ?? config["DATABASE_URL"];
string connectionString;
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    // Hỗ trợ Koyeb/Heroku style: postgres://user:pass@host:port/db
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var user = userInfo.Length > 0 ? userInfo[0] : string.Empty;
    var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";
    Console.WriteLine("DATABASE_URL detected -> using parsed connection string");
}
else
{
    DatabaseSettings dbSettings = new DatabaseSettings
    {
        Ip       = config["DB_HOST"] ?? config["DatabaseSettings:Ip"],
        Port     = int.TryParse(config["DB_PORT"], out var port) ? port : (config.GetSection("DatabaseSettings").GetValue<int?>("Port") ?? 5432),
        User     = config["DB_USER"] ?? config["DatabaseSettings:User"],
        Password = config["DB_PASSWORD"] ?? config["DatabaseSettings:Password"],
        Database = config["DB_NAME"] ?? config["DatabaseSettings:Database"]
    };
    dbSettings.Display();
    connectionString = dbSettings.GetConnectionString();
}


// Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString + ";Pooling=true;Maximum Pool Size=5;Minimum Pool Size=0;Timeout=15;", npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
        // npgsqlOptions.CommandTimeout(60);
    }));

builder.Services.AddTransient<DatabaseSeeder>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Đăng ký service thêm chú thích cho api
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    //ADD JWT BEARER SECURITY DEFINITION
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        //Type = SecuritySchemeType.ApiKey,
        Type = SecuritySchemeType.Http,//ko cần thêm token phía trước
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer"
            },
            new List<string>()
        }
    });
});

// Đăng ký các repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//command
builder.Services.AddScoped<ICommandHandler<CreateCommand, AppointmentDetailResponse>, CreateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateCommand, AppointmentDetailResponse>, UpdateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteCommand>, DeleteCommandHandler>();
//query
builder.Services.AddScoped<IQueryHandler<GetAllPagingQuery, PageResult<AppointmentDetailResponse>>, GetAllPagingQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetByIdQuery, AppointmentDetailResponse>, GetByIdQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetTotalAmountByAppointmentIdQuery, TotalAmountResponse>, GetTotalAmountByAppointmentIdQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetDashboardStatsQuery, GetDashboardStatsResponse>, GetDashboardStatsQueryHandler>();
// Cấu hình APIServiceOption: ưu tiên ENV rồi mới tới appsettings
var apiServiceSection = builder.Configuration.GetSection("APIService");
builder.Services.Configure<APIServiceOption>(options =>
{
    options.BaseUrlAppointment = Environment.GetEnvironmentVariable("BASE_URL_APPOINTMENT")
        ?? apiServiceSection["BaseUrlAppointment"]
        ?? string.Empty;
    options.BaseUrlServiceDetail = Environment.GetEnvironmentVariable("BASE_URL_SERVICEDETAIL")
        ?? apiServiceSection["BaseUrlServiceDetail"]
        ?? string.Empty;
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<IAPIService, APIService>();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});


//Đăng ký lấy thông tin từ token
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IServiceDetailService, ServiceDetailServices>();

builder.Services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .SetIsOriginAllowed(origin =>
                {
                    Console.WriteLine($"CORS Origin requested: {origin}");
                    return true; // Cho phép tất cả origin (phù hợp với môi trường dev/test)
                })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});


var app = builder.Build();
// Bật Swagger theo ENV hoặc cấu hình
var swaggerEnabled = app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled");
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    var runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS");
    if (!string.IsNullOrWhiteSpace(runMigrations) && runMigrations.Equals("true", StringComparison.OrdinalIgnoreCase))
    {
        context.Database.Migrate();
    }
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Thứ tự middleware quan trọng cho CORS
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();