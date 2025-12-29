using HelpDesk.API.Configurations;
using Serilog;

var options = new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot"
};

// Create builder with options
WebApplicationBuilder builder = WebApplication.CreateBuilder(options);

builder.Host.UseSerilog();

// Initialize and execute builder-level configuration (services, DI, middleware setup)
ApplicationConfiguration applicationConfig = new();
applicationConfig.ExecuteBuilderConfiguration(builder);

WebApplication app = builder.Build();

// Execute app-level configuration (middleware pipeline, localization, Swagger, etc.)
applicationConfig.ExecuteAppConfiguration(app);

app.Run();
