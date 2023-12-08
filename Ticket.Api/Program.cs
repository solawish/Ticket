using Microsoft.AspNetCore.Rewrite;
using System.Net;
using Ticket.Api.Infrastructure.ApiVersion;
using Ticket.Api.Infrastructure.SwaggerConfigs;
using Ticket.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddApplicationServices(builder.Configuration);

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 首頁轉址到 swagger
var option = new RewriteOptions().AddRedirect("^$", "swagger", (int)HttpStatusCode.Redirect);
app.UseRewriter(option);

app.Run();