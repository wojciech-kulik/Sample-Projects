using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace IpVersionDetection
{
    // the order is important, if we want to support bitwise OR: IPv4 | IPv6 equals IPv4and6
    public enum IpVersion
    {
        None,
        IPv4,
        IPv6,
        IPv4and6
    }

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Loaded += (s, e) => RefreshUI();
            NetworkInformation.NetworkStatusChanged += NetworkChanged;
        }

        private async void NetworkChanged(object sender)
        {
            await tbIpVersion.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RefreshUI);
        }

        private async void RefreshUI()
        {
            tbIpVersion.Text = (await GetCurrentIpVersion()).ToString();
        }

        public async Task<IpVersion> GetCurrentIpVersion()
        {
            try
            {
                var endPointPairs = await DatagramSocket.GetEndpointPairsAsync(new HostName("google.com"), "0");
                if (endPointPairs == null)
                {
                    return IpVersion.None;
                }

                var result = IpVersion.None;
                foreach (var endPoint in endPointPairs)
                {
                    if (endPoint.RemoteHostName != null && (endPoint.RemoteHostName.Type == HostNameType.Ipv4 || endPoint.RemoteHostName.Type == HostNameType.Ipv6))
                    {
                        result |= (endPoint.RemoteHostName.Type == HostNameType.Ipv4 ? IpVersion.IPv4 : IpVersion.IPv6);
                    }
                }
                return result;
            }
            catch
            {
                return IpVersion.None;
            }
        }
    }
}
