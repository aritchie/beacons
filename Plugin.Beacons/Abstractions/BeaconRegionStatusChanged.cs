using System;


namespace Plugin.Beacons
{
    public class BeaconRegionStatusChanged : EventArgs
    {
        public BeaconRegionStatusChanged(BeaconRegion region, bool entering)
        {
            this.Region = region;
            this.IsEntering = entering;
        }


        public bool IsEntering { get; }
        public BeaconRegion Region { get; }
    }
}
