namespace Vivelin.NetMonitor.Networking;

public interface INetworkService
{
    NetworkStatus CheckNetworkInterfaces(double expectedMbps = 800);
}