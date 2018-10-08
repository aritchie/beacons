using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Plugin.BluetoothLE;


namespace Plugin.Beacons
{
    public class BeaconManager : IBeaconManager
    {
        readonly IAdapter adapter;
        readonly IPluginRepository repository;
        readonly IDictionary<string, BeaconRegionStatus> regionStates;

        /*
         *  if (this.AdvertisedBeacon != null)
                throw new ArgumentException("You are already advertising a beacon");

            var settings = new AdvertiseSettings.Builder()
                .SetAdvertiseMode(AdvertiseMode.Balanced)
                .SetConnectable(false);

            var adData = new AdvertiseData.Builder()
                .AddManufacturerData(0x004C, beacon.ToIBeaconPacket(10)); // Apple

            this.manager
                .Adapter
                .BluetoothLeAdvertiser
                .StartAdvertising(
                    settings.Build(),
                    adData.Build(),
                    this.adCallbacks
                );

            this.AdvertisedBeacon = beacon;
         */
        public BeaconManager(IAdapter adapter = null, IPluginRepository repository = null)
        {
            this.adapter = adapter ?? CrossBleAdapter.Current;
            this.repository = repository ?? new SqlitePluginRepository();
            this.regionStates = new Dictionary<string, BeaconRegionStatus>();
            var regions = this.repository.GetTrackingRegions();
            foreach (var region in regions)
                this.SetRegion(region);
        }


        public IObservable<bool> RequestPermission() => Internals.HasPermission();


        public IObservable<Beacon> WhenBeaconsRanged(BeaconRegion region)
        {
            this.SetRegion(region);
            return this.Scan()
                .Where(region.IsBeaconInRegion)
                .Finally(() =>
                {
                    // TODO: remove filter target
                });
        }


        public void StartMonitoring(BeaconRegion region)
        {
            this.repository.StartTracking(region);
            this.SetRegion(region);
        }


        public void StopMonitoring(BeaconRegion region)
        {
            this.repository.StopTracking(region);
            lock (this.regionStates)
                this.regionStates.Remove(region.Identifier);
        }


        public void StopAllMonitoring()
        {
            this.repository.StopAllTracking();
            lock (this.regionStates)
                this.regionStates.Clear();
        }


        public IReadOnlyList<BeaconRegion> MonitoredRegions { get; }


        IObservable<BeaconRegionStatusChanged> regionOb;
        public IObservable<BeaconRegionStatusChanged> WhenRegionStatusChanged()
        {
            this.regionOb = this.regionOb ?? Observable.Create<BeaconRegionStatusChanged>(ob =>
            {
                var scan = this.Scan()
                    .Subscribe(beacon =>
                    {
                        var states = this.GetRegionStatesForBeacon(this.MonitoredRegions, beacon);
                        foreach (var state in states)
                        {
                            if (state.IsInRange != null && !state.IsInRange.Value)
                            {
                                ob.OnNext(new BeaconRegionStatusChanged(state.Region, true));
                            }
                            state.IsInRange = true;
                            state.LastPing = DateTimeOffset.UtcNow;
                        }
                    });

                    var cleanup = this
                        .WhenRegionExited()
                        .Subscribe(ob.OnNext);

                return () =>
                {
                    scan.Dispose();
                    cleanup.Dispose();
                };
            })
            .Publish()
            .RefCount();

            return this.regionOb;
        }


        IObservable<Beacon> beaconScanner;
        IObservable<Beacon> Scan()
        {
            // TODO: switch to background scan
            this.beaconScanner = this.beaconScanner ?? this.adapter
                .Scan()
                .Select(sr =>
                {
                    Beacon beacon = null;
                    var md = sr.AdvertisementData?.ManufacturerData;
                    if (md != null && Beacon.IsIBeaconPacket(md))
                        beacon = Beacon.Parse(md, sr.Rssi);

                    return beacon;
                })
                .Where(x => x != null)
                .Publish()
                .RefCount();

            return this.beaconScanner;
        }


        IObservable<BeaconRegionStatusChanged> regionExitOb;
        protected virtual IObservable<BeaconRegionStatusChanged> WhenRegionExited()
        {
            this.regionExitOb = this.regionExitOb ?? Observable.Create<BeaconRegionStatusChanged>(ob => Observable
                .Interval(TimeSpan.FromSeconds(5)) // TODO: configurable
                .Subscribe(x =>
                {
                    var maxAge = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(5));

                    foreach (var state in this.regionStates.Values)
                    {
                        if (state.IsInRange == true && state.LastPing > maxAge)
                        {
                            state.IsInRange = false;
                            ob.OnNext(new BeaconRegionStatusChanged(state.Region, false));
                        }
                    }
                })
            )
            .Publish()
            .RefCount();
            return this.regionExitOb;
        }


        protected virtual BeaconRegionStatus SetRegion(BeaconRegion region)
        {
            var key = region.ToString();
            BeaconRegionStatus status = null;

            lock (this.regionStates)
            {
                if (this.regionStates.ContainsKey(key))
                {
                    status = this.regionStates[key];
                }
                else
                {
                    status = new BeaconRegionStatus(region);
                    this.regionStates.Add(key, status);
                }
            }
            return status;
        }


        protected virtual IEnumerable<BeaconRegionStatus> GetRegionStatesForBeacon(IEnumerable<BeaconRegion> regionList, Beacon beacon)
        {
            var copy = this.regionStates.ToDictionary(x => x.Key, x => x.Value);

            foreach (var region in regionList)
            {
                var state = copy[region.ToString()];
                if (state.Region.IsBeaconInRegion(beacon))
                    yield return state;
            }
        }
    }
}
