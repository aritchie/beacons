using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Plugin.Beacons
{
    public interface IBeaconManager
    {
        //bool IsAdvertisingSupported { get; }
        //void StartAdvertising(Beacon beacon);
        //void StopAdvertising();

        Task<bool> RequestPermission();

        IObservable<RangedBeaconList> WhenBeaconsRanged(params BeaconRegion[] regions);

        void StartMonitoring(BeaconRegion region);
        void StopMonitoring(BeaconRegion region);
        void StopAllMonitoring();
        IReadOnlyList<BeaconRegion> MonitoredRegions { get; }
        IObservable<BeaconRegionStatusChanged> WhenRegionStatusChanged();
    }
}