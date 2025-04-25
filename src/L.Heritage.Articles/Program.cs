using L.Heritage.Articles.Api;
using L.Heritage.Articles.Extensions;
using L.Heritage.Shared.Extensions.Core;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.NewVersionedApi()
    .MapArticlesApiV1();

app.Run();