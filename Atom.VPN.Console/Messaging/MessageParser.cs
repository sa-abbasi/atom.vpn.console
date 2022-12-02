using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using MessageTypes = Atom.VPN.CC.Messaging.MessageType;

namespace Atom.VPN.Console
{
    public interface IMessageParser
    {
        BaseRequest Parse(string message);
    }

    public class MessageParser : IMessageParser
    {
        public BaseRequest Parse(string message)
        {
            JObject jsonData = JObject.Parse(message);

            if (!int.TryParse((string)jsonData["MessageType"], out var msgType))
            {
                return new InvalidRequest() { OriginalMessage = message };
            }

            return Parse(msgType, jsonData, message);

            //string rssTitle = (string)rss["channel"]["title"];
        }

        private BaseRequest Parse(int MessageType, JObject jsonMessage, string message)
        {
            BaseRequest result = null;

            if (!Enum.IsDefined(typeof(MessageTypes), MessageType))
            {
                result = new InvalidRequest() { OriginalMessage = message, MessageType = (int)MessageTypes.Invalid };
                try { result.RequestId = (string)jsonMessage["RequestId"]; } catch { }
                return result;
            }

            if (MessageType == (int)MessageTypes.ConnectSDK)
            {
                result = JsonConvert.DeserializeObject<ConnectRequest>(message);
            }

            if (MessageType == (int)MessageTypes.ConnectVPN)
            {
                result = JsonConvert.DeserializeObject<ConnectVPNRequest>(message);
            }

            if (result == null)
            {
                result = JsonConvert.DeserializeObject<BaseRequest>(message);
            }

            result.OriginalMessage = message;

            return result;
        }
    }



}
