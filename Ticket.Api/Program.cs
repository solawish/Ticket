using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Rewrite;
using Serilog;
using System.Net;
using Ticket.Api.Infrastructure.ApiVersion;
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

builder.Services.AddControllers();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    var apiProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

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

// 首頁轉址到 swagger
var option = new RewriteOptions().AddRedirect("^$", "swagger", (int)HttpStatusCode.Redirect);
app.UseRewriter(option);

app.Run();