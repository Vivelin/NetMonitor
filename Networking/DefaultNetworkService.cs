using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Vivelin.NetMonitor.Networking;

public class DefaultNetworkService : INetworkService
{
    public NetworkStatus CheckNetworkInterfaces(double expectedMbps = 800)
    {
        var net = GetPrimaryNetworkInterface();
        if (net == null)
            return NetworkStatus.Offline;

        var mbps = net.Speed / 1000000D;
        return mbps < expectedMbps ? NetworkStatus.LowLinkSpeed : NetworkStatus.Normal;
    }

    public IPAddress? GetPrimaryIPv4()
    {
        var net = GetPrimaryNetworkInterface();
        if (net == null)
            return null;

        var ip = net.GetIPProperties();
        return ip.UnicastAddresses
            .Select(x => x.Address)
            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
    }

    public string? GetAdapterName()
    {
        return GetPrimaryNetworkInterface()?.Name;
    }

    private static NetworkInterface? GetPrimaryNetworkInterface()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(x => x.OperationalStatus == OperationalStatus.Up
                     && x.NetworkInterfaceType != NetworkInterfaceType.Loopback
                     && x.Supports(NetworkInterfaceComponent.IPv4))
            .FirstOrDefault();
    }
}
