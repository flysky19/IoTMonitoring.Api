using System;
using IoTMonitoring.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ���� ���
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Swagger ����
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IoT Monitoring API",
        Version = "v1",
        Description = "IoT ���� ����͸� �ý��� API"
    });

    // JWT ���� ����
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

// ����� ���� ���� ��� (���� �߿�!)
try
{
    Console.WriteLine("�����ͺ��̽� ���� ��...");
    builder.Services.ConfigureDatabase(builder.Configuration);

    Console.WriteLine("�α� ���� ��...");
    builder.Services.ConfigureLogging();

    Console.WriteLine("���� ���� ��...");
    builder.Services.ConfigureMappers();

    Console.WriteLine("�������丮 ���� ��...");
    builder.Services.ConfigureRepositories();

    Console.WriteLine("���� ���� ��...");
    builder.Services.ConfigureServices();

    Console.WriteLine("���� ���� ��...");
    builder.Services.ConfigureAuthentication(builder.Configuration);


    Console.WriteLine("��� ���� ��� �Ϸ�!");
}
catch (Exception ex)
{
    Console.WriteLine($"���� ��� �� ����: {ex.Message}");
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