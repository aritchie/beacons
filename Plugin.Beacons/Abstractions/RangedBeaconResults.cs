using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Plugin.Beacons
{
    public class RangedBeaconResults
    {
        protected RangedBeaconResults() {}
        public RangedBeaconResults(BeaconRegion region, IList<Beacon> beacons)
        {
            this.Region = region;
            this.Beacons = new ReadOnlyCollection<Beacon>(beacons);
        }


        public BeaconRegion Region { get; protected set; }
        public IReadOnlyList<Beacon> Beacons { get; protected set; }
    }
}
