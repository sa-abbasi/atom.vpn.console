namespace Atom.VPN.Console
{
    /// <summary>
    /// Processes Disconnect command
    /// </summary>
    public class DisconnectMessageProcessor : IMessageProcessor
    {
        ICommandProcessor commandProcessor = null;

        public DisconnectMessageProcessor(ICommandProcessor CommandProcessor)
        {
            this.commandProcessor = CommandProcessor;
        }
        public BaseResponse Process(BaseRequest Request)
        {
            var Response = new BaseResponse();

            var request = Request as ConnectRequest;

            commandProcessor.Disconnect();
            Response.IsOK = true;
            Response.Message = "Disconnected successfully";
            return Response;
        }
    }

}
