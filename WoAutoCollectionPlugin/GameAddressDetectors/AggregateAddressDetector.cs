﻿using Dalamud.Game.ClientState;
using Dalamud.Logging;
using System;
using System.Net;

namespace WoAutoCollectionPlugin.GameAddressDetectors
{
    public class AggregateAddressDetector : GameAddressDetector
    {
        private bool ipHlpDidError;
        private readonly IpHlpApiAddressDetector ipHlpApiDetector;
        private readonly ClientStateAddressDetector clientStateDetector;

        public AggregateAddressDetector(ClientState clientState)
        {
            this.ipHlpApiDetector = new IpHlpApiAddressDetector();
            this.clientStateDetector = new ClientStateAddressDetector(clientState);
        }

        public override IPAddress GetAddress(bool verbose = false)
        {
            var address = IPAddress.Loopback;

            // Try to read the server address from the TCP table
            GameAddressDetector bestDetector = this.ipHlpApiDetector;
            if (!this.ipHlpDidError)
            {
                try
                {
                    address = this.ipHlpApiDetector.GetAddress(verbose);
                }
                catch (Exception e)
                {
                    this.ipHlpDidError = true;
                    PluginLog.LogError(e, "Exception occurred in TCP table reading. Falling back to client state detection.");
                }
            }

            if (Equals(address, IPAddress.Loopback))
            {
                // That didn't work, fall back to the client state address detector
                bestDetector = this.clientStateDetector;
                try
                {
                    address = this.clientStateDetector.GetAddress(verbose);
                }
                catch (Exception e)
                {
                    PluginLog.LogError(e, "Exception occurred in client state IP address detection. This should never happen!");
                }
            }

            if (verbose && !Equals(address, IPAddress.Loopback) && !Equals(address, Address))
            {
                PluginLog.Log($"Got new server address {address} from detector {bestDetector.GetType().Name}");
            }

            Address = address;
            return Address;
        }
    }
}