using System;
using IoTMonitoring.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 서비스 등록
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Swagger 설정
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IoT Monitoring API",
        Version = "v1",
        Description = "IoT 센서 모니터링 시스템 API"
    });

    // JWT 인증 설정
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 사용자 정의 서비스 등록 (순서 중요!)
try
{
    Console.WriteLine("데이터베이스 설정 중...");
    builder.Services.ConfigureDatabase(builder.Configuration);

    Console.WriteLine("로깅 설정 중...");
    builder.Services.ConfigureLogging();

    Console.WriteLine("매퍼 설정 중...");
    builder.Services.ConfigureMappers();

    Console.WriteLine("리포지토리 설정 중...");
    builder.Services.ConfigureRepositories();

    Console.WriteLine("서비스 설정 중...");
    builder.Services.ConfigureServices();

    Console.WriteLine("인증 설정 중...");
    builder.Services.ConfigureAuthentication(builder.Configuration);


    Console.WriteLine("모든 서비스 등록 완료!");
}
catch (Exception ex)
{
    Console.WriteLine($"서비스 등록 중 오류: {ex.Message}");
    throw;
}



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IoT Monitoring API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();