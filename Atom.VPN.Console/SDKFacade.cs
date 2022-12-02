﻿using Atom.Core.Models;
using Atom.SDK.Core.Enumerations;
using Atom.SDK.Core.Models;
using Atom.SDK.Core;
using Atom.SDK.Net;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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

        void ConnectVPN(ConnectVPNRequest Request);
        void DisconnectVPN();
    }

    /// <summary>
    /// This class is a wrapper on Atom VPN Winows SDK and implements ICommandProcessor to execute commands using VPN Sdk
    /// </summary>

    public class SDKFacade : ICommandProcessor
    {

        ILogger logger = NLog.LogManager.GetLogger("SDKFacade");
        AtomManager atomManager = null;

        ConnectRequest Creds { get; set; }
        List<Country> Countries { get; set; }
        List<Protocol> Protocols { get; set; }



        public bool IsSDKInitializing { get; private set; } = false;
        public bool ISSDKInitialized { get; private set; } = false;




        public SDKFacade()
        {

        }

        /// <summary>
        /// ProtocolName is actually ProtocolSlug
        /// </summary>
        /// <param name="CountryISOCode"></param>
        /// <param name="ProtocolName"></param>
        public void ConnectVPN(ConnectVPNRequest Request)
        {
            try
            {
                //System.Threading.Thread.Sleep(15000);

                logger.Trace($"Connecting VPN Country:{Request.Country}, Protocol:{Request.Protocol}");


                var protocol = this.Protocols.FirstOrDefault(x => x.ProtocolSlug == Request.Protocol);

                var country = this.Countries.FirstOrDefault(x => x.CountrySlug == Request.Country);

                bool countrySupportProtocol = country.Protocols.Any(x => x.ProtocolSlug == protocol.ProtocolSlug);

                if (countrySupportProtocol == false)
                {
                    throw new Exception("This country does not support given protocol");
                }


                //  Core.Models.Protocol protocol = country.Protocols.FirstOrDefault(x => x.ProtocolSlug == ProtocolName);

                List<string> protocolNames = new List<string>();
                protocolNames.Add(protocol.ProtocolSlug);

                var secondaryProtocol = country.Protocols.Where(x => x.ProtocolSlug != protocol.ProtocolSlug).FirstOrDefault();
                protocolNames.Add(secondaryProtocol.ProtocolSlug);

                var tertiaryProtocol = country.Protocols.Where(x => !protocolNames.Contains(x.ProtocolSlug)).FirstOrDefault();


                VPNProperties properties = new VPNProperties(country, protocol);


                properties.UseOptimization = false;
                properties.UseSmartDialing = false;

                /*
                else if (UseCityConnection)
                {
                    properties = new VPNProperties(SelectedCity, PrimaryProtocol);
                    properties.UseOptimization = UseOptimization;
                }
                else if (UseChannelConnection)
                {
                    properties = new VPNProperties(SelectedChannel, PrimaryProtocol);
                }
                 */

                secondaryProtocol = tertiaryProtocol = null;

                properties.SecondaryProtocol = secondaryProtocol;
                properties.TertiaryProtocol = tertiaryProtocol;
                properties.UseSplitTunneling = false;
                properties.DoCheckInternetConnectivity = true;
                properties.EnableDNSLeakProtection = false;
                properties.EnableIPv6LeakProtection = false;



                atomManager.Credentials = new Credentials(Creds.UserId, Creds.Password);



                atomManager.Connect(properties);

                logger.Info("VPN Connection has been made successfully");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "vpn_country_connection_error");
                throw;
            }
        }

        public void DisconnectVPN()
        {
            atomManager.Disconnect();
        }

        public void InitializeSDK(string PSK, string UserName, string Password)
        {

            logger.Info("*****************Initialzing SDK****************");



            logger.Info("initializing AtomManager");

            IsSDKInitializing = true;

            atomManager = AtomManager.Initialize(PSK);
            atomManager.SecretKey = PSK;
            atomManager.Credentials = new Credentials(UserName, Password);

            var vpnProperties = new VPNProperties(PSK);

            // atomManager.Connect(vpnProperties);



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



            IsSDKInitializing = false;
            ISSDKInitialized = true;


            logger.Info("********SDK has been initialized successfully**************");
        }



        public bool Connect(ConnectRequest Request)
        {
            if (atomManager == null)
            {
                Creds = Request;
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
            this.Countries = countries;

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
            this.Protocols = protocols;
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
