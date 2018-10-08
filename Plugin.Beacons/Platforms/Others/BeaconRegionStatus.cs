using System;


namespace Plugin.Beacons
{
    public class BeaconRegionStatus
    {
        public BeaconRegionStatus(BeaconRegion region)
        {
            this.Region = region;
        }


        public BeaconRegion Region { get; }
        public bool? IsInRange { get; set; }
        public DateTimeOffset LastPing { get; set; }
    }
}
