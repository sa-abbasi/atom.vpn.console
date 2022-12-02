namespace Atom.VPN.Console
{
    /// <summary>
    /// Connect request message received from client websocket
    /// </summary>
    public class ConnectVPNRequest : BaseRequest
    {

        public string Country { get; set; }


        public string Protocol { get; set; }


        public string SecondaryProtocol { get; set; }
        public string tertiaryProtocol { get; set; }


        public bool UseOptimization { get; set; }
        public bool UseSmartDialing { get; set; }

        public bool UseSplitTunneling { get; set; }
        public bool DoCheckInternetConnectivity { get; set; }
        public bool EnableDNSLeakProtection { get; set; }
        public bool EnableIPv6LeakProtection { get; set; }





    }

}
