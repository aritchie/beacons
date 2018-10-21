using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Plugin.BluetoothLE;


namespace Plugin.Beacons
{
    public class BeaconManager : IBeaconManager
    {
        readonly IAdapter adapter;
        readonly IPluginRepository repository;
        readonly IDictionary<string, BeaconRegionStatus> regionStates;
        readonly Subject<BeaconRegionStatusChanged> monitorSubject;
        IDisposable monitorScan;

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

            this.monitorSubject = new Subject<BeaconRegionStatusChanged>();
            this.regionStates = new Dictionary<string, BeaconRegionStatus>();
            var regions = this.repository.GetTrackingRegions();
            foreach (var region in regions)
                this.SetRegion(region);
        }


        public IReadOnlyList<BeaconRegion> MonitoredRegions => this.regionStates.Values.Select(x => x.Region).ToList();
        public IObservable<bool> RequestPermission() => Internals.HasPermission();
        public IObservable<Beacon> WhenBeaconRanged(BeaconRegion region) => this.Scan().Where(region.IsBeaconInRegion);
        public IObservable<BeaconRegionStatusChanged> WhenRegionStatusChanged() => this.monitorSubject;


        public void StartMonitoring(BeaconRegion region)
        {
            this.repository.StartMonitoring(region);
            this.SetRegion(region);
        }


        public void StopMonitoring(BeaconRegion region)
        {
            this.repository.StopMonitoring(region);
            lock (this.regionStates)
                this.regionStates.Remove(region.Identifier);

            if (this.regionStates.Count == 0)
                this.CleanupMonitoringScanner();
        }


        public void StopAllMonitoring()
        {
            this.CleanupMonitoringScanner();

            this.repository.StopAllMonitoring();
            lock (this.regionStates)
                this.regionStates.Clear();
        }


        protected void TryStartMonitorScanner()
        {
            if (this.monitorScan != null)
                return;

            this.monitorScan = Observable.Create<BeaconRegionStatusChanged>(ob =>
            {
                var internalScan = this.Scan()
                    .Where(_ => this.regionStates.Count > 0)
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

                var cleanup = Observable
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
                    });

                return () =>
                {
                    internalScan.Dispose();
                    cleanup.Dispose();
                };
            })
            .Subscribe(this.monitorSubject.OnNext);
        }


        protected void CleanupMonitoringScanner()
        {
            this.monitorScan?.Dispose();
            this.monitorScan = null;
        }


        IObservable<Beacon> beaconScanner;
        protected IObservable<Beacon> Scan()
        {
            // TODO: switch to background scan
            this.beaconScanner = this.beaconScanner ?? this.adapter
                .Scan()
                .Where(x => Beacon.IsIBeaconPacket(x.AdvertisementData?.ManufacturerData))
                .Select(x => Beacon.Parse(x.AdvertisementData.ManufacturerData, x.Rssi))
                .Publish()
                .RefCount();

            return this.beaconScanner;
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
            this.TryStartMonitorScanner();
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
