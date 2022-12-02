namespace Atom.VPN.Console
{
    /// <summary>
    /// Connect request message received from client websocket
    /// </summary>
    public class ConnectRequest : BaseRequest
    {
        /// <summary>
        /// Preshared Key
        /// </summary>
        public string PSK { get; set; }

        /// <summary>
        /// UserId or UserName
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; }
    }

}
