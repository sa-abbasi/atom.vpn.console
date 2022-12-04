
using MessageTypes = Atom.VPN.CC.Messaging.MessageType;
using System;
using System.Threading.Tasks;

namespace Atom.VPN.Console
{
    public interface IMessgeBroker
    {
        Task Process(IMessageChannel Channel, BaseRequest Request);
    }

    public class MessageBroker : IMessgeBroker
    {

        ICommandProcessor commandProcessor = null;
        NLog.ILogger logger = NLog.LogManager.GetLogger("MessageBroker");


        public MessageBroker(ICommandProcessor commandProcessor)
        {

            this.commandProcessor = commandProcessor;
        }

        public async Task Process(IMessageChannel Channel, BaseRequest Request)
        {

            if (Request.MessageType == (int)MessageTypes.Invalid)
            {

                logger.Warn("Received invalid request: {0}", Request.ToString());
                var irr = new BaseResponse()
                {
                    IsOK = false,
                    MessageType = ((int)MessageTypes.Invalid),
                    Message = "Invalid request type"
                };

                await Channel.Send(irr);
                return;
            }

            BaseResponse result;
            try
            {
                var messageProcessor = GetMessageProcessor(Request);
                result = messageProcessor.Process(Request);
                result.RequestId = Request.RequestId;
                result.MessageType = Request.MessageType;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "request_processing_error {0}", Request);
                result = new BaseResponse();
                result.IsOK = false;
                result.RequestId = Request.RequestId;
                result.MessageType = Request.MessageType;
                result.Message = ex.Message;
            }

            await Channel.Send(result);

        }

        /// <summary>
        /// A factory method which returns correct message processor for Request
        /// </summary>
        /// <param name="Request">An instance of BaseRequest or one of its derived type</param>
        /// <returns></returns>
        private IMessageProcessor GetMessageProcessor(BaseRequest Request)
        {
            int RequestType = Request.MessageType;

            if (RequestType == (int)MessageTypes.ConnectSDK)
            {
                return new ConnectMessageProcessor(this.commandProcessor);
            }

            if (RequestType == (int)MessageTypes.DisconnectSDK)
            {
                return new DisconnectSDKProcessor(this.commandProcessor);
            }

            if (RequestType == (int)MessageTypes.GetCountryList)
            {
                return new CountryMessageProcessor(this.commandProcessor);
            }

            if (RequestType == (int)MessageTypes.GetProtocols)
            {
                return new ProtocolMessageProcessor(this.commandProcessor);
            }

            if (RequestType == (int)MessageTypes.GetCities)
            {
                return new CityMessageProcessor(this.commandProcessor);
            }

            if (RequestType == (int)MessageTypes.ConnectVPN)
            {
                return new ConnectVPNMessageProcessor(this.commandProcessor);
            }

            if (RequestType == (int)MessageTypes.DisconnectVPN)
            {
                return new DisconnectVPNProcessor(this.commandProcessor);
            }






            return null;

        }

    }


}
