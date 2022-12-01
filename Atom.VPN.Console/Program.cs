using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Fluent;
using System;

namespace Atom.VPN.Console
{
    internal class Program
    {
        static System.Threading.Mutex singleTonInstance = null;

        static int Main(string[] args)
        {
            if (IsAlreadyRunning())
            {
                return 2;
            }

            NLog.ILogger logger = NLog.LogManager.GetLogger("Program.Main");

            logger.Info("****************Process has started****************");
            bool status = false;

            try
            {
                logger.Info("creating sdkfacade");

                ICommandProcessor sdkFacade = new SDKFacade();


                //MessageBroker uses sdkFacade to execute commands on Atm.VPN Windows SDK
                IMessgeBroker messageBroker = new MessageBroker(sdkFacade);

                //Parses incoming json messages and creates C# Request objects
                IMessageParser messageParser = new MessageParser();

                //Message Listener listens for incoming wbsocket connections
                MessageListener messageListener = new MessageListener(messageParser, messageBroker);
                messageListener.Listen();
                logger.Info("websocket listening loop has started");

                status = true;


            }
            catch (Exception ex)
            {
                logger.Error(ex, "VPN.Console encountered an error");
            }

            if (status)
                System.Console.ReadLine();

            logger.Warn("Application is shutting down");

            return 0;
        }

        static bool IsAlreadyRunning()
        {
            bool mutexCreated = false;
            try
            {
                singleTonInstance = new System.Threading.Mutex(true, @"Local\atm.vpn.console", out mutexCreated);

                if (!mutexCreated)
                {
                    singleTonInstance.Close();
                    return true;

                }
            }
            catch { }

            return false;
        }

    }


}
