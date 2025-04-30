using L.Heritage.Articles.Api;
using L.Heritage.Articles.Extensions;
using L.Heritage.Shared.Extensions.Core;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.NewVersionedApi()
    .MapArticlesApiV1();

app.Run();