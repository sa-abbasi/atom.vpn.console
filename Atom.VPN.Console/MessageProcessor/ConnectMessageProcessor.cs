namespace Atom.VPN.Console
{
    /// <summary>
    /// Processes Connect command
    /// </summary>
    public class ConnectMessageProcessor : IMessageProcessor
    {
        ICommandProcessor commandProcessor = null;

        public ConnectMessageProcessor(ICommandProcessor CommandProcessor)
        {
            this.commandProcessor = CommandProcessor;
        }
        public BaseResponse Process(BaseRequest Request)
        {
            var Response = new BaseResponse();

            var request = Request as ConnectRequest;

            commandProcessor.Connect(request);
            Response.IsOK = true;
            Response.Message = "Connected successfully";
            return Response;
        }
    }

}
