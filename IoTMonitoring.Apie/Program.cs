using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using IoTMonitoring.Api.Data.DbContext;
using IoTMonitoring.Api.Data.Repositories;
using IoTMonitoring.Api.Data.Repositories.Interfaces;
using IoTMonitoring.Api.Services.Implementations;
using IoTMonitoring.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// DB Context 설정
builder.Services.AddDbContext<IoTDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper 등록
builder.Services.AddAutoMapper(typeof(Program));

// 리포지토리 등록
builder.Services.AddScoped<ISensorRepository, SensorRepository>();

// 서비스 등록
builder.Services.AddScoped<ISensorService, SensorService>();

// 컨트롤러 추가
builder.Services.AddControllers();

// CORS 정책 설정
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Swagger 설정
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IoT Monitoring API", Version = "v1" });
});

var app = builder.Build();

// 개발 환경에서 Swagger 활성화
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();