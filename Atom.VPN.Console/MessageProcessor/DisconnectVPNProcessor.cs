namespace Atom.VPN.Console
{
    public class DisconnectVPNProcessor : IMessageProcessor
    {
        ICommandProcessor commandProcessor = null;

        public DisconnectVPNProcessor(ICommandProcessor CommandProcessor)
        {
            this.commandProcessor = CommandProcessor;
        }
        public BaseResponse Process(BaseRequest Request)
        {
            var Response = new BaseResponse();

            var request = Request as ConnectRequest;

            commandProcessor.DisconnectVPN();
            Response.IsOK = true;
            Response.Message = "Disconnected successfully";
            return Response;
        }
    }

}
