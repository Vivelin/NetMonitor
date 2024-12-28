using System.Net.NetworkInformation;

using var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
Console.CancelKeyPress += (sender, e) =>
{
    waitHandle.Set();
    e.Cancel = true;
};

var lastStatus = NetworkStatus.Unknown;
var lastChange = DateTimeOffset.UtcNow;

NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

using var timer = new Timer(OnTick, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
waitHandle.WaitOne();

NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;

void OnTick(object? state)
{
    var status = CheckNetworkInterfaces();
    if (status != lastStatus)
    {
        lastStatus = status;
        lastChange = DateTimeOffset.UtcNow;
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}: {status}");
    }
    else
    {
        var timeSinceLastChange = DateTimeOffset.UtcNow - lastChange;
        if (timeSinceLastChange > TimeSpan.FromHours(1))
        {
            lastChange = DateTimeOffset.UtcNow; // Don't spam this message after one hour
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}: No change in last hour.");
        }
    }
}

void NetworkChange_NetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
{
    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}: Network availability changed to {e.IsAvailable}.");
    OnTick(null);
}

void NetworkChange_NetworkAddressChanged(object? sender, EventArgs e)
{
    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd'T'HH:mm:ss}: Network address changed.");
    OnTick(null);
}

static NetworkStatus CheckNetworkInterfaces(double expectedMbps = 800)
{
    var net = NetworkInterface.GetAllNetworkInterfaces()
        .Where(x => x.OperationalStatus == OperationalStatus.Up
                 && x.NetworkInterfaceType != NetworkInterfaceType.Loopback)
        .FirstOrDefault();

    if (net == null)
    {
        return NetworkStatus.Offline;
    }

    var mbps = net.Speed / 1000000D;
    if (mbps < expectedMbps)
    {
        return NetworkStatus.LowLinkSpeed;
    }

    return NetworkStatus.Normal;
}


enum NetworkStatus
{
    Unknown = 0,
    Normal,
    Offline,
    LowLinkSpeed,
}