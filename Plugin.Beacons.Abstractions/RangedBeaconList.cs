using System;
using System.Collections.Generic;

namespace Plugin.Beacons
{
    public class RangedBeaconList
    {
        public RangedBeaconList(BeaconRegion region, IList<Beacon> beacons)
        {
            this.Region = region;
            this.Beacons = beacons;
        }


        public BeaconRegion Region { get; }
        public IList<Beacon> Beacons { get; }
    }
}

