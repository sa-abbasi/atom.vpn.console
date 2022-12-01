using Atom.Core.Models;
using Atom.SDK.Core.Enumerations;
using Atom.SDK.Core.Models;
using Atom.SDK.Core;
using Atom.SDK.Net;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atom.VPN.Console
{


    public interface ICommandProcessor
    {
        /// <summary>
        /// Used to make connection with VPN SDK
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        bool Connect(ConnectRequest Request);

        bool Disconnect();

        /// <summary>
        /// Returns country list
        /// </summary>
        /// <returns></returns>
        List<Country> GetCountryList();

        /// <summary>
        /// Returns city list
        /// </summary>
        /// <returns></returns>

        List<City> GetCityList();


        /// <summary>
        /// Returns protocol list
        /// </summary>
        /// <returns></returns>
        List<Protocol> GetProtocolList();
    }

    /// <summary>
    /// This class is a wrapper on Atom VPN Winows SDK and implements ICommandProcessor to execute commands using VPN Sdk
    /// </summary>

    public class SDKFacade : ICommandProcessor
    {

        ILogger logger = NLog.LogManager.GetLogger("SDKFacade");
        AtomManager atomManager = null;





        public bool IsSDKInitializing { get; private set; } = false;
        public bool ISSDKInitialized { get; private set; } = false;




        public SDKFacade()
        {

        }


        public void InitializeSDK(string PSK, string UserName, string Password)
        {

            logger.Info("*****************Initialzing SDK****************");



            logger.Info("initializing AtomManager");

            IsSDKInitializing = true;

            atomManager = AtomManager.Initialize(PSK);
            atomManager.Credentials = new Credentials(UserName, Password);

            var vpnProperties = new VPNProperties(PSK);

            atomManager.Connect(vpnProperties);



            atomManager.Connected += AtomManagerInstance_Connected;
            atomManager.DialError += AtomManagerInstance_DialError;
            atomManager.Disconnected += AtomManagerInstance_Disconnected;
            atomManager.StateChanged += AtomManagerInstance_StateChanged;
            atomManager.Redialing += AtomManagerInstance_Redialing;
            atomManager.OnUnableToAccessInternet += AtomManagerInstance_OnUnableToAccessInternet;
            atomManager.SDKAlreadyInitialized += AtomManagerInstance_SDKAlreadyInitialized;
            atomManager.ConnectedLocation += AtomManagerInstance_ConnectedLocation;
            atomManager.AtomInitialized += AtomManagerInstance_AtomInitialized;
            atomManager.AtomDependenciesMissing += AtomManagerInstance_AtomDependenciesMissing;

            // Add sensitive application that needs to be close if network connections drops automatically.
            atomManager.SensitiveApplications = new List<SensitiveApplication>()
                {
                    new SensitiveApplication() { CompleteExePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" }
                };

            atomManager.AutoRedialOnConnectionDrop = true;

            //Old code for reference only
            //AtomHelper lets you use the functionality of above created instance in all usercontrols and pages
            // AtomHelper.SetAtomManagerInstance(atomManagerInstance);


            // ConnectionWithDedicatedIP.Initialize(protocols, countries);
            //  ConnectionWithParams.Initialize(protocols, countries);


            IsSDKInitializing = false;
            ISSDKInitialized = true;


            logger.Info("********SDK has been initialized successfully**************");
        }



        public bool Connect(ConnectRequest Request)
        {
            if (atomManager == null)
            {

                InitializeSDK(Request.PSK, Request.UserId, Request.Password);
                return atomManager.VPNConnectionState == VPNState.CONNECTED;
            }


            var vpnProps = new VPNProperties(Request.PSK);
            this.atomManager.Credentials = new Credentials(Request.UserId, Request.Password);
            this.atomManager.Connect(vpnProps);

            return atomManager.VPNConnectionState == VPNState.CONNECTED;
        }

        public bool Disconnect()
        {
            if (atomManager.VPNConnectionState == VPNState.DISCONNECTED)
                return true;

            if (atomManager.VPNConnectionState == VPNState.CONNECTING)
            {
                atomManager.Cancel();
            }
            else
            {
                atomManager.Disconnect();
            }

            return atomManager.VPNConnectionState == VPNState.DISCONNECTED;
        }

        public List<Country> GetCountryList()
        {

            var countries = atomManager.GetCountries();

            return countries;

        }

        public List<City> GetCityList()
        {
            var cities = atomManager.GetCities();
            return cities;
        }

        public List<Protocol> GetProtocolList()
        {
            var protocols = atomManager.GetProtocols();
            return protocols;
        }


        #region AtomRegisteredEvents
        private void AtomManagerInstance_ConnectedLocation(object sender, ConnectedLocationEventArgs e)
        {

            logger.Info("Entered");
            if (e.Error != null)
            {
                logger.Error(e.Error, "ConnectedLocation_callback_error");
            }

            logger.Info($" IP: {e.ConnectedLocation.Ip}, Country: {e.ConnectedLocation.Country}, Server: {e.ConnectedLocation.Server}");


        }



        private void AtomManagerInstance_SDKAlreadyInitialized(object sender, ErrorEventArgs e)
        {
            logger.Error(e.Exception, "ConnectedLocation_callback_error {0}", e.Message);


        }

        private void AtomManagerInstance_OnUnableToAccessInternet(object sender, SDK.Core.CustomEventArgs.UnableToAccessInternetEventArgs e)
        {
            logger.Error(e.Exception, "Unable to access internet {0}", e.Message);
            //var a = e.ConnectionDetails;

        }

        private void AtomManagerInstance_StateChanged(object sender, StateChangedEventArgs e)
        {
            logger.Info("NewState {0}", e.State);
            if (e.State == VPNState.RECONNECTING)
            {
                //Send a message to front end
            }


        }

        private void AtomManagerInstance_Connected(object sender, EventArgs e)
        {
            logger.Info("OnConnected callback");

            if (atomManager.VPNProperties.UseSplitTunneling)
            {
                atomManager.ApplySplitTunneling(new SDK.Core.Models.SplitApplication() { CompleteExePath = "Chrome.exe" });
            }
        }

        private void AtomManagerInstance_Disconnected(object sender, DisconnectedEventArgs e)
        {
            logger.Info("OnDisconnected callback {0}", e.Message);

        }

        private void AtomManagerInstance_DialError(object sender, DialErrorEventArgs e)
        {
            logger.Info("OnDialError callback {0}", e.Message);


        }

        private void AtomManagerInstance_Redialing(object sender, ErrorEventArgs e)
        {
            logger.Info("OnRedialling callback {0}", e.Message);


        }

        private void AtomManagerInstance_AtomInitialized(object sender, SDK.Core.CustomEventArgs.AtomInitializedEventArgs e)
        {
            logger.Info("OnIitialized callback {0}", e.IsInitializedSuccessfully);
            if (e.Exception != null)
            {
                logger.Error(e.Exception, "Initialization_error");
            }


        }

        private void AtomManagerInstance_AtomDependenciesMissing(object sender, SDK.Core.CustomEventArgs.AtomDependenciesMissingEventArgs e)
        {
            logger.Error(e.Exception, "Dependencies are missing ");


        }

        //We need to send messages to front end from all above events

        #endregion


    }


}
