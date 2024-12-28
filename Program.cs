using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Vivelin.NetMonitor;
using Vivelin.NetMonitor.Networking;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<INetworkService, DefaultNetworkService>();
builder.Services.AddHostedService<NetMonitorService>();

var host = builder.Build();
await host.RunAsync();
