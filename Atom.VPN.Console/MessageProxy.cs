
using System;

namespace Atom.VPN.Console
{
    public interface IMessagePublisher
    {
        void Publish(string data);
    }

    public interface IMessageNotifier
    {
        Action<string> OnMessageReceived { get; set; }
    }

    public class MessageProxy : IMessagePublisher, IMessageNotifier
    {
        public Action<string> OnMessageReceived { get; set; }
        public void Publish(string data)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(data);
            }
        }
    }

}
