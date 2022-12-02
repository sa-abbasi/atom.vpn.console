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

}
