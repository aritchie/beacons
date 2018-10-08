using System;
using System.Collections.Generic;


namespace Plugin.Beacons
{
    public interface IBeaconManager
    {
        //bool IsAdvertisingSupported { get; }
        //void StartAdvertising(Beacon beacon);
        //void StopAdvertising();

        //bool IsBluetoothAdapterReady { get; }
        IObservable<bool> RequestPermission();
        IObservable<Beacon> WhenBeaconRanged(BeaconRegion region);

        void StartMonitoring(BeaconRegion region);
        void StopMonitoring(BeaconRegion region);
        void StopAllMonitoring();
        IReadOnlyList<BeaconRegion> MonitoredRegions { get; }
        IObservable<BeaconRegionStatusChanged> WhenRegionStatusChanged();
    }
}