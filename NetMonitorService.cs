using System.Net.NetworkInformation;

using Microsoft.Extensions.Hosting;

using Vivelin.NetMonitor.Networking;

namespace Vivelin.NetMonitor;

public class NetMonitorService : IHostedService, IDisposable
{
    public NetMonitorService(INetworkService networkService)
    {
        Timer = new Timer(OnTimerTick, null, Timeout.Infinite, Timeout.Infinite);
        NetworkService = networkService;
    }

    public INetworkService NetworkService { get; }

    public NetworkStatus LastStatus { get; set; } = NetworkStatus.Unknown;

    public DateTimeOffset LastChange { get; set; } = DateTimeOffset.UtcNow;

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
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}: {status}");
        }
        else
        {
            var timeSinceLastChange = DateTimeOffset.UtcNow - LastChange;
            if (timeSinceLastChange > TimeSpan.FromHours(1))
            {
                LastChange = DateTimeOffset.UtcNow; // Don't spam this message after one hour
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}: No change in last hour.");
            }
        }
    }

    private void NetworkChange_NetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
    {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}: Network availability changed to {e.IsAvailable}.");
        OnTimerTick(null);
    }

    private void NetworkChange_NetworkAddressChanged(object? sender, EventArgs e)
    {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}: Network address changed.");
        OnTimerTick(null);
    }
}
