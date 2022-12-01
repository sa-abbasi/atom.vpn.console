namespace Atom.VPN.Console
{
    /// <summary>
    /// Processes get Protocol List command
    /// </summary>
    public class ProtocolMessageProcessor : IMessageProcessor
    {
        ICommandProcessor commandProcessor = null;

        public ProtocolMessageProcessor(ICommandProcessor CommandProcessor)
        {
            this.commandProcessor = CommandProcessor;
        }
        public BaseResponse Process(BaseRequest Request)
        {
            var Response = new BaseResponse();

            var items = commandProcessor.GetProtocolList();
            Response.IsOK = true;
            Response.Message = string.Empty;
            Response.Result = items;

            return Response;
        }
    }

}
