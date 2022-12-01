namespace Atom.VPN.Console
{

    /// <summary>
    /// BaseRequest type is a base class for all other messages. If a command/message does not need extra parameters then instance of this class is sufficient to represent a request message
    /// MessageType <see cref="MessageType"/> denotes unique request/command.
    /// </summary>
    public class BaseRequest
    {
        /// <summary>
        /// Used to identify incoming request type. See <seealso cref="Atom.VPN.CC.Messaging.MessageType"/> for MessageType values
        /// </summary>
        public int MessageType { get; set; }

        public string RequestId { get; set; }

        /// <summary>
        /// Original json message received from client websocket
        /// </summary>
        public string OriginalMessage { get; set; }

        public override string ToString()
        {
            return $"{{ MessageType: {MessageType}, Text={OriginalMessage} }}";
        }

    }

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

    /// <summary>
    /// Invalid request denotes a message sent by client where MessageType is not one of the correct value
    /// </summary>
    public class InvalidRequest : BaseRequest
    {

        public override string ToString()
        {
            return $"{{ Req:InvalidRequest, MessageType: {MessageType}, Text={OriginalMessage} }}";
        }

    }

}
