using System;
using System.Collections.Generic;


namespace Plugin.Beacons
{
    public interface IBeaconManager
    {
        //bool IsAdvertisingSupported { get; }
        //void StartAdvertising(Beacon beacon);
        //void StopAdvertising();

        IObservable<bool> RequestPermission();
        IObservable<RangedBeaconList> WhenBeaconsRanged(params BeaconRegion[] regions);

        void StartMonitoring(BeaconRegion region);
        void StopMonitoring(BeaconRegion region);
        void StopAllMonitoring();
        IReadOnlyList<BeaconRegion> MonitoredRegions { get; }
        IObservable<BeaconRegionStatusChanged> WhenRegionStatusChanged();
    }
}