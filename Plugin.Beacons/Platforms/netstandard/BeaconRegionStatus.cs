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


        public override bool Equals(object obj)
        {
            var other = obj as BeaconRegionStatus;
            if (obj == null)
                return false;

            var r = this.Region.Equals(other.Region);
            return r;
        }


        public override int GetHashCode()
        {
            return this.Region.GetHashCode();
        }
    }
}
