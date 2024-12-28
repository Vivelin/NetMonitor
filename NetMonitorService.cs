using System.Net.NetworkInformation;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Vivelin.NetMonitor.Networking;

namespace Vivelin.NetMonitor;

public class NetMonitorService : IHostedService, IDisposable
{
    public NetMonitorService(INetworkService networkService, ILogger<NetMonitorService> logger)
    {
        Timer = new Timer(OnTimerTick, null, Timeout.Infinite, Timeout.Infinite);
        NetworkService = networkService;
        Logger = logger;
    }

    public INetworkService NetworkService { get; }
    public NetworkStatus LastStatus { get; set; } = NetworkStatus.Unknown;
    public DateTimeOffset LastChange { get; set; } = DateTimeOffset.UtcNow;
    protected ILogger<NetMonitorService> Logger { get; }
    protected Timer Timer { get; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        Timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Timer.Change(Timeout.Infinite, Timeout.Infinite);
        NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
        NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Timer.Dispose();
        }
    }

    private void OnTimerTick(object? state)
    {
        var status = NetworkService.CheckNetworkInterfaces();
        if (status != LastStatus)
        {
            LastStatus = status;
            LastChange = DateTimeOffset.UtcNow;

            Logger.Log(status switch
            {
                NetworkStatus.LowLinkSpeed => LogLevel.Warning,
                NetworkStatus.Offline => LogLevel.Error,
                NetworkStatus.Unknown => LogLevel.Debug,
                _ => LogLevel.Information
            }, "{Status}", status);
        }
        else
        {
            var timeSinceLastChange = DateTimeOffset.UtcNow - LastChange;
            if (timeSinceLastChange > TimeSpan.FromHours(1))
            {
                LastChange = DateTimeOffset.UtcNow; // Don't spam this message after one hour
                Logger.LogDebug("No change in last hour.");
            }
        }
    }

    private void NetworkChange_NetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
    {
        Logger.LogDebug("Network availability changed.");
        OnTimerTick(null);
    }

    private void NetworkChange_NetworkAddressChanged(object? sender, EventArgs e)
    {
        Logger.LogDebug("Network address changed.");
        OnTimerTick(null);
    }
}
