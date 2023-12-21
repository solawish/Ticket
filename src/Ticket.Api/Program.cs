using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Rewrite;
using Serilog;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ticket.Api.Infrastructure.ApiVersion;
using Ticket.Api.Infrastructure.Middleware;
using Ticket.Api.Infrastructure.SwaggerConfigs;
using Ticket.Application;
using Ticket.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // 只輸出 Json, 移除輸出 XML
    options.OutputFormatters.RemoveType<XmlDataContractSerializerOutputFormatter>();
}).AddJsonOptions(options =>
{
    // 不分大小寫
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    // 序列化, 移除空白
    options.JsonSerializerOptions.WriteIndented = true;

    // 配合前端習慣, ViewModel 與 Parameter 顯示為小駝峰命名
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

    // 非 ASCII 文字不轉碼
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    // Enum 轉換設定
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Api Version
builder.Services.AddApiVersion();

// url 小寫顯示
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Swagger Register
builder.Services.AddSwagger();

// JsonSerializerOptions 設定
builder.Services.AddTransient(_ => new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var apiProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"{description.GroupName}/swagger.json",
                $"Ticket {description.GroupName}");
        }
    });

    app.UseReDoc(options =>
    {
        foreach (var description in apiProvider.ApiVersionDescriptions)
        {
            options.SpecUrl($"/swagger/{description.GroupName}/swagger.json");
            options.RoutePrefix = "redoc";
            options.DocumentTitle = $"Ticket {description.GroupName}";
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionMiddleware();

// 首頁轉址到 swagger
var option = new RewriteOptions().AddRedirect("^$", "swagger", (int)HttpStatusCode.Redirect);
app.UseRewriter(option);

app.UseStaticFiles();

app.Run();