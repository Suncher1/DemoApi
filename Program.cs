using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Register Services
builder.Services.AddSerilog((services, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

builder.Services.AddOpenApi();

// Build web application
var app = builder.Build();

// Add middleware
app.MapOpenApi();
app.MapScalarApiReference();

app.Logger.LogInformation("----");

app.Logger.LogWarning("This is a warning");
app.Logger.LogError("This is an error");

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("###");
    await next(context);

});

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Före: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next(context);
    app.Logger.LogInformation("Efter: {StatusCode}", context.Response.StatusCode);
});

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Andra middlewere före: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next(context);
    app.Logger.LogInformation("Andra middlewere Efter: {StatusCode}", context.Response.StatusCode);
});


app.MapGet("/", () => "API is running");

app.MapGet("/hello/{name}", (string name) =>
{
    var settingName = builder.Configuration["CustomSettings:SettingName"];
    return new
    {
        Message = $"Hello {name} {settingName}"
    };
});

app.Run();
