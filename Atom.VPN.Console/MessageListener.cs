
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using NLog;
using Fleck;
using Newtonsoft.Json;
using System.Security.Principal;

namespace Atom.VPN.Console
{
    /// <summary>
    /// It listens for incoming websocket connections, accept a connection and then receives data on it.
    /// Received message is parsed using IMessageParserver and processed through IMessageBroker.
    /// This class implements IMessageChannel to send reply messages to websocket client
    /// </summary>
    public class MessageListener : IMessageChannel
    {
        WebSocketServer serverSocket = null;

        //As we are using this application on a single machine so we shall have one active socket at a time
        IWebSocketConnection clientSocket = null;


        ILogger logger = NLog.LogManager.GetLogger("MessageListener");


        IMessageParser messageParser = null;
        IMessgeBroker messageBroker = null;

        public SocketState CurrentState { get; private set; }

        public MessageListener(IMessageParser MessageParser, IMessgeBroker MessageBroker, string ListeningUrl = "ws://0.0.0.0:8081")
        {
            logger.Info("opening websocket to listen on {0}", ListeningUrl);

            this.serverSocket = new WebSocketServer(ListeningUrl);
            CurrentState = SocketState.None;
            this.messageParser = MessageParser;
            this.messageBroker = MessageBroker;

        }

        /// <summary>
        /// Call this method once after making object of this class
        /// </summary>
        public void Listen()
        {
            try
            {


                serverSocket.Start(socket =>
                {
                    logger.Info("Received connection {0}", socket.ConnectionInfo.ClientIpAddress);

                    CloseClientConnection();

                    this.clientSocket = socket;

                    socket.OnOpen = () =>
                    {
                        CurrentState = SocketState.Opened;
                    };

                    socket.OnClose = () =>
                    {
                        CurrentState = SocketState.Closed;
                    };

                    socket.OnMessage = OnMessageReceived;
                    socket.OnError = OnSockerError;
                    socket.OnBinary = OnBinaryMessageReceived;
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "web_socket_listener_error");
            }

        }


        private void OnBinaryMessageReceived(byte[] Data)
        {
            logger.Warn("Received_binary messaage, which is not implemented");

        }

        private void CloseClientConnection()
        {
            if (clientSocket != null)
            {
                clientSocket.Close();

            }
        }

        /// <summary>
        /// This method is called whenever a json message is received on accepted socket
        /// </summary>
        /// <param name="Message"></param>
        private void OnMessageReceived(string Message)
        {
            try
            {
                logger.Trace("Received_message {0}", Message);

                var request = this.messageParser.Parse(Message);

                messageBroker.Process(this, request).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "error occured while processing received message");
            }


        }



        private void OnSockerError(Exception ex)
        {
            logger.Error(ex, "client_socker_exception");
        }


        /// <summary>
        /// This method is used to send reply messages to connected client
        /// </summary>
        /// <param name="Response"></param>
        /// <returns></returns>
        public async Task Send(BaseResponse Response)
        {
            try
            {
                string data = JsonConvert.SerializeObject(Response);

                logger.Trace("Sending_response: {0}", data.LeftOf(30));

                await clientSocket.Send(data);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "response_sending_error");
            }
        }


    }



    public enum SocketState
    {
        None = 0,
        Opened = 1,
        Closed = 2
    }
}
