using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Fluent;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Atom.VPN.Console
{
    internal partial class Program
    {
        static System.Threading.Mutex singleTonInstance = null;
        static NLog.ILogger logger = null;
        static EventHandler _handler;
        static bool exitSystem = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns>0=OK, 1=Exception/Error, 2=Already Running,</returns>
        static int Main(string[] args)
        {
            if (IsAlreadyRunning())
            {
                return 2;
            }

            logger = NLog.LogManager.GetLogger("Program.Main");

            logger.Info("****************Process has started****************");


            int returnCode = -1;

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);


            try
            {
                logger.Info("creating sdkfacade");

                ICommandProcessor sdkFacade = new SDKFacade();


                //MessageBroker uses sdkFacade to execute commands on Atm.VPN Windows SDK
                IMessgeBroker messageBroker = new MessageBroker(sdkFacade);

                //Parses incoming json messages and creates C# Request objects
                IMessageParser messageParser = new MessageParser();

                //Message Listener listens for incoming wbsocket connections
                string ListeningUrl = Properties.Settings.Default.ListeningUrl;
                MessageListener messageListener = new MessageListener(messageParser, messageBroker, ListeningUrl);
                messageListener.Listen();
                logger.Info("websocket listening loop has started");

                returnCode = 0;


            }
            catch (Exception ex)
            {
                logger.Error(ex, "VPN.Console encountered an error");
                returnCode = 1;
            }


            if (returnCode == 0)
            {
                while (!exitSystem)
                {
                    Thread.Sleep(500);
                }
            }

            logger.Warn("Application is shutting down");


            return returnCode;
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





        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private static bool Handler(CtrlType sig)
        {
            logger.Warn("Exiting system due to external CTRL-C, or process kill, or shutdown");

            try { singleTonInstance.Close(); } catch { }


            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(0);

            return true;
        }
        #endregion



    }




}
