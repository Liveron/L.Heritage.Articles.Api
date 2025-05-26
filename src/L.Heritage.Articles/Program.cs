using L.Heritage.Articles.Api;
using L.Heritage.Articles.Extensions;
using L.Heritage.Shared.Extensions.Core;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();

app.MapMetrics();

app.NewVersionedApi()
    .MapArticlesApiV1();

app.Run();