using System.Net.NetworkInformation;

namespace Vivelin.NetMonitor.Networking;

public class DefaultNetworkService : INetworkService
{
    public NetworkStatus CheckNetworkInterfaces(double expectedMbps = 800)
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
        return mbps < expectedMbps ? NetworkStatus.LowLinkSpeed : NetworkStatus.Normal;
    }
}
