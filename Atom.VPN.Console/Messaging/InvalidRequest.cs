namespace Atom.VPN.Console
{
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
