using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using IoTMonitoring.Api.Data.DbContext;
using IoTMonitoring.Api.Data.Repositories;
using IoTMonitoring.Api.Data.Repositories.Interfaces;
using IoTMonitoring.Api.Services.Implementations;
using IoTMonitoring.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// DB Context ����
builder.Services.AddDbContext<IoTDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper ���
builder.Services.AddAutoMapper(typeof(Program));

// �������丮 ���
builder.Services.AddScoped<ISensorRepository, SensorRepository>();

// ���� ���
builder.Services.AddScoped<ISensorService, SensorService>();

// ��Ʈ�ѷ� �߰�
builder.Services.AddControllers();

// CORS ��å ����
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Swagger ����
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IoT Monitoring API", Version = "v1" });
});

var app = builder.Build();

// ���� ȯ�濡�� Swagger Ȱ��ȭ
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