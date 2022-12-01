namespace Atom.VPN.Console
{
    /// <summary>
    /// Processes get country command
    /// </summary>
    public class CountryMessageProcessor : IMessageProcessor
    {
        ICommandProcessor commandProcessor = null;

        public CountryMessageProcessor(ICommandProcessor CommandProcessor)
        {
            this.commandProcessor = CommandProcessor;
        }
        public BaseResponse Process(BaseRequest Request)
        {
            var Response = new BaseResponse();

            var countries = commandProcessor.GetCountryList();
            Response.IsOK = true;
            Response.Message = string.Empty;
            Response.Result = countries;

            return Response;
        }
    }

}
