# CareNest New Service Template

Template dự án mới để kết nối với hệ thống CareNest, sử dụng cùng kiến trúc và pattern với CareNest_AppointmentDetail.

## Cấu trúc dự án

```
CareNest_NewService/
├── CareNest_NewService.API/           # API Layer
├── CareNest_NewService.Application/   # Application Layer  
├── CareNest_NewService.Domain/        # Domain Layer
├── CareNest_NewService.Infrastructure/ # Infrastructure Layer
├── Shared/                            # Shared Components
├── docker-compose.yml                 # Docker Configuration
├── Dockerfile                         # Docker Build
└── CareNest_NewService.sln           # Solution File
```

## Kiến trúc

- **Clean Architecture** với 4 layers chính
- **CQRS Pattern** cho Commands và Queries
- **Repository Pattern** với Unit of Work
- **Dependency Injection** với .NET Core DI
- **Docker Support** với multi-stage build

## Cách sử dụng

1. Copy toàn bộ template này
2. Đổi tên từ "NewService" thành tên service của bạn
3. Cập nhật config trong appsettings
4. Chạy với Docker Compose

## Kết nối với hệ thống

- Sử dụng `APIService` để gọi các service khác
- Sử dụng `ApiResponse<T>` cho response format
- Sử dụng `ResponseResult<T>` cho internal results
- Cấu hình network với static IP trong Docker
