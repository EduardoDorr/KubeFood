using KubeFood.Core.Telemetry;
using KubeFood.Delivery.Worker.Application;
using KubeFood.Delivery.Worker.Infrastructure;

using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddSerilog(builder.Configuration);
builder.Services.AddApplicationModule();
builder.Services.AddInfrastructureModule(builder.Configuration);

var host = builder.Build();
await host.RunAsync();