using System.Net;

namespace Vivelin.NetMonitor.Networking;

public interface INetworkService
{
    NetworkStatus CheckNetworkInterfaces(double expectedMbps = 800);
    string? GetAdapterName();
    IPAddress? GetPrimaryIPv4();
}