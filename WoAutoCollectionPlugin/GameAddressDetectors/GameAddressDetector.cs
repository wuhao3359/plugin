using System.Net;

namespace WoAutoCollectionPlugin.GameAddressDetectors
{
    public abstract class GameAddressDetector
    {
        protected IPAddress Address { get; set; }

        public abstract IPAddress GetAddress(bool verbose = false);
    }
}