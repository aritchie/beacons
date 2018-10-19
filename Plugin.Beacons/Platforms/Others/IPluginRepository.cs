using System;
using System.Collections.Generic;


namespace Plugin.Beacons
{
    public interface IPluginRepository
    {
        IEnumerable<BeaconRegion> GetTrackingRegions();
        void StartMonitoring(BeaconRegion region);
        void StopMonitoring(BeaconRegion region);
        void StopAllMonitoring();
    }
}
