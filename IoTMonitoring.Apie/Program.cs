using System;
using IoTMonitoring.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IoTMonitoring.Api.Services.MQTT.Interfaces;
using IoTMonitoring.Api.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using IoTMonitoring.Api.Services.SignalR.Interfaces;
using IoTMonitoring.Api.Services.SignalR;
using IoTMonitoring.Api.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ���� ���
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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
    //builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


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
    Console.WriteLine($"���� Ʈ���̽�: {ex.StackTrace}");
    throw;
}

// SignalR ���� ���
builder.Services.AddSignalR();
builder.Services.AddScoped<ISignalRService, SignalRService>();

var app = builder.Build();

Console.WriteLine($"���� ȯ��: {app.Environment.EnvironmentName}");
Console.WriteLine($"IsDevelopment: {app.Environment.IsDevelopment()}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IoT Monitoring API V1");
        c.RoutePrefix = "swagger"; // Swagger�� /swagger ��η� ����
    });
}

app.UseStaticFiles();
app.UseDefaultFiles();
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseRouting();

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/debug", () => "API is running!");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var mqttService = scope.ServiceProvider.GetRequiredService<IMqttService>();
    _ = Task.Run(async () => await mqttService.StartAsync());
}


Console.WriteLine("���ø����̼��� ���۵Ǿ����ϴ�.");
Console.WriteLine($"Swagger URL: https://localhost:7051/swagger");
Console.WriteLine($"Debug URL: https://localhost:7051/debug");


// SignalR Hub ��������Ʈ ����
app.MapHub<SensorHub>("/sensorHub");


app.Run();