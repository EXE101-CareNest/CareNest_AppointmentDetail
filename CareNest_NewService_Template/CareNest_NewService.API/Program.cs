using CareNest_NewService.API.Middleware;
using CareNest_NewService.Application.Common;
using CareNest_NewService.Application.Common.Options;
using CareNest_NewService.Application.Interfaces.Services;
using CareNest_NewService.Application.Interfaces.UOW;
using CareNest_NewService.Infrastructure.Persistences.Configuration;
using CareNest_NewService.Infrastructure.Persistences.Database;
using CareNest_NewService.Infrastructure.Persistences.Repository;
using CareNest_NewService.Infrastructure.Services;
using CareNest_NewService.Infrastructure.UOW;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Lấy DatabaseSettings từ configuration
DatabaseSettings dbSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>()!;
dbSettings.Display();
string connectionString = dbSettings.GetConnectionString();

// Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
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
        Type = SecuritySchemeType.Http,
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

// Đăng ký API Service
builder.Services.Configure<APIServiceOption>(builder.Configuration.GetSection("APIService"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAPIService, APIService>();

// Đăng ký JWT Settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

// Đăng ký Current User Service
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Đăng ký Use Case Dispatcher
builder.Services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

//Đăng ký cho FE
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auto migrate database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
