namespace Atom.VPN.Console
{
    public class ConnectVPNMessageProcessor : IMessageProcessor
    {
        ICommandProcessor commandProcessor = null;

        public ConnectVPNMessageProcessor(ICommandProcessor CommandProcessor)
        {
            this.commandProcessor = CommandProcessor;
        }
        public BaseResponse Process(BaseRequest Request)
        {
            var vpnRequest = Request as ConnectVPNRequest;

            var Response = new BaseResponse();

            commandProcessor.ConnectVPN(vpnRequest);
            Response.IsOK = true;
            Response.Message = "VPN has connected successfully";
            Response.Result = "0";

            return Response;
        }
    }

}
