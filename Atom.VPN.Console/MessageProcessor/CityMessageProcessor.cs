namespace Atom.VPN.Console
{

    /// <summary>
    /// Processes get City List command
    /// </summary>
    public class CityMessageProcessor : IMessageProcessor
    {
        ICommandProcessor commandProcessor = null;

        public CityMessageProcessor(ICommandProcessor CommandProcessor)
        {
            this.commandProcessor = CommandProcessor;
        }
        public BaseResponse Process(BaseRequest Request)
        {
            var Response = new BaseResponse();

            var items = commandProcessor.GetCityList();
            Response.IsOK = true;
            Response.Message = string.Empty;
            Response.Result = items;

            return Response;
        }
    }

}
