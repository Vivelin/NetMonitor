using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Vivelin.NetMonitor;
using Vivelin.NetMonitor.Networking;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(options => options.FormatterName = "Custom");
builder.Logging.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>();

builder.Services.AddScoped<INetworkService, DefaultNetworkService>();
builder.Services.AddHostedService<NetMonitorService>();

var host = builder.Build();
await host.RunAsync();
