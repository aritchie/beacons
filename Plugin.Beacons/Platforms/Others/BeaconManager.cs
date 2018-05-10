using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Plugin.BluetoothLE;


namespace Plugin.Beacons
{
    public class BeaconManager : IBeaconManager
    {
        readonly IAdapter adapter;
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
        public BeaconManager(IAdapter adapter = null)
        {
            this.adapter = adapter ?? CrossBleAdapter.Current;
            this.regionStates = new Dictionary<string, BeaconRegionStatus>();

            var regions = Services.Settings.GetTrackingRegions();
            foreach (var region in regions)
                this.SetRegion(region);
        }


        public IObservable<bool> RequestPermission()
            => Observable.Return(this.adapter.Status == AdapterStatus.PoweredOn);


        public IObservable<RangedBeaconResults> WhenBeaconsRanged(params BeaconRegion[] regions)
        {
            foreach (var region in regions)
                this.SetRegion(region);

            return Observable.Create<RangedBeaconResults>(ob => this
                .Scan()
                .Buffer(TimeSpan.FromSeconds(2))
                .Subscribe(beacons =>
                {
                    foreach (var region in regions)
                    {
                        var list = new List<Beacon>();
                        //var rbl = new RangedBeaconList(region, list);

                        //foreach (var beacon in beacons)
                        //    if (region.IsBeaconInRegion(beacon))
                        //        list.Add(beacon);

                        //ob.OnNext(rbl);
                    }
                })
            );
        }


        public void StartMonitoring(BeaconRegion region)
        {
            //this.settings.Add(region);
            this.SetRegion(region);
        }


        public void StopMonitoring(BeaconRegion region)
        {
            //this.settings.Remove(region);
            // TODO: if user doesn't unhook observables, scanning will continue
        }


        public void StopAllMonitoring()
        {
            // TODO: if user doesn't unhook observables, scanning will continue
            //this.settings.Clear();
        }


        //public IReadOnlyList<BeaconRegion> MonitoredRegions => this.settings.MonitorRegions;
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
            this.beaconScanner = this.beaconScanner ?? Observable
                .Create<Beacon>(ob => this.adapter
                    .Scan()
                    .Subscribe(sr =>
                    {
                        var md = sr.AdvertisementData?.ManufacturerData;
                        if (md != null && Beacon.IsIBeaconPacket(md))
                        {
                            var beacon = Beacon.Parse(md, sr.Rssi);
                            ob.OnNext(beacon);
                        }
                    })
                )
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

            if (this.regionStates.ContainsKey(key))
            {
                status = this.regionStates[key];
            }
            else
            {
                status = new BeaconRegionStatus(region);
                this.regionStates.Add(key, status);
            }
            return status;
        }


        protected virtual IEnumerable<BeaconRegionStatus> GetRegionStatesForBeacon(IEnumerable<BeaconRegion> regionList, Beacon beacon)
        {
            foreach (var region in regionList)
            {
                var state = this.regionStates[region.ToString()];
                if (state.Region.IsBeaconInRegion(beacon))
                    yield return state;
            }
        }
    }
}
