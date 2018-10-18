using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CoreLocation;


namespace Plugin.Beacons
{
    public class BeaconManager : IBeaconManager
    {
        readonly CLLocationManager manager;


        public BeaconManager()
        {
            this.manager = new CLLocationManager();
        }


        public IObservable<bool> RequestPermission() => Internals.HasPermission();


        public IObservable<Beacon> WhenBeaconRanged(BeaconRegion region)
        {
            var nativeRegion = this.ToNative(region);
            this.manager.StartRangingBeacons(nativeRegion);

            return this.Scan()
                .Where(region.IsBeaconInRegion)
                .Finally(() =>
                    this.manager.StopRangingBeacons(nativeRegion)
                );
        }


        public void StartMonitoring(BeaconRegion region)
        {
            var native = this.ToNative(region);
            this.manager.StartMonitoring(native);
        }


        public void StopMonitoring(BeaconRegion region)
        {
            var native = this.ToNative(region);
            this.manager.StopMonitoring(native);
        }


        public void StopAllMonitoring()
        {
            var allRegions = this
               .manager
               .MonitoredRegions
               .OfType<CLBeaconRegion>();

            foreach (var region in allRegions)
                this.manager.StopMonitoring(region);
        }


        public IReadOnlyList<BeaconRegion> MonitoredRegions => this
           .manager
           .MonitoredRegions
           .OfType<CLBeaconRegion>()
           .Select(this.FromNative)
           .ToList();


        public IObservable<BeaconRegionStatusChanged> WhenRegionStatusChanged() => Observable.Create<BeaconRegionStatusChanged>(ob =>
        {
            var enterHandler = this.CreateHandler(ob, true);
            var leftHandler = this.CreateHandler(ob, false);

            this.manager.RegionEntered += enterHandler;
            this.manager.RegionLeft += leftHandler;

            return () =>
            {
                this.manager.RegionEntered -= enterHandler;
                this.manager.RegionLeft -= leftHandler;
            };
        });


        IObservable<Beacon> beaconScanner;
        protected IObservable<Beacon> Scan()
        {
            this.beaconScanner = this.beaconScanner ?? Observable.Create<Beacon>(ob =>
            {
                var handler = new EventHandler<CLRegionBeaconsRangedEventArgs>((sender, args) =>
                {
                    foreach (var native in args.Beacons)
                    {
                        ob.OnNext(new Beacon
                        (
                            native.ProximityUuid.FromNative(),
                            native.Major.UInt16Value,
                            native.Minor.UInt16Value,
                            native.Accuracy,
                            this.FromNative(native.Proximity)
                        ));
                    }
                });
                this.manager.DidRangeBeacons += handler;
                return () => this.manager.DidRangeBeacons -= handler;
            })
            .Publish()
            .RefCount();

            return this.beaconScanner;
        }

        protected EventHandler<CLRegionEventArgs> CreateHandler(IObserver<BeaconRegionStatusChanged> ob, bool entering)
            => (sender, args) =>
            {
                var br = args.Region as CLBeaconRegion;
                if (br != null)
                {
                    var region = this.FromNative(br);
                    var status = new BeaconRegionStatusChanged(region, entering);
                    ob.OnNext(status);
                }
            };


        protected BeaconRegion FromNative(CLBeaconRegion region)
            => new BeaconRegion(
                region.Identifier,
                region.ProximityUuid.FromNative(),
                region.Major?.UInt16Value,
                region.Minor?.UInt16Value
            );


        protected CLBeaconRegion ToNative(BeaconRegion region)
        {
            if (region.Uuid == null)
                throw new ArgumentException("You must pass a UUID for the Beacon Region");

            var uuid = region.Uuid.ToNative();
            CLBeaconRegion native = null;

            if (region.Major > 0 && region.Minor > 0)
                native = new CLBeaconRegion(uuid, region.Major.Value, region.Minor.Value, region.Identifier);

            else if (region.Major > 0)
                native = new CLBeaconRegion(uuid, region.Major.Value, region.Identifier);

            else
                native = new CLBeaconRegion(uuid, region.Identifier);

            //native.NotifyEntryStateOnDisplay = true;
            native.NotifyOnEntry = true;
            native.NotifyOnExit = true;

            return native;
        }


        protected Proximity FromNative(CLProximity proximity)
        {
            switch (proximity)
            {
                case CLProximity.Far:
                    return Proximity.Far;

                case CLProximity.Immediate:
                    return Proximity.Immediate;

                case CLProximity.Near:
                    return Proximity.Near;

                case CLProximity.Unknown:
                default:
                    return Proximity.Unknown;
            }
        }


        protected CLProximity ToNative(Proximity proximity)
        {
            switch (proximity)
            {
                case Proximity.Far:
                    return CLProximity.Far;

                case Proximity.Immediate:
                    return CLProximity.Immediate;

                case Proximity.Near:
                    return CLProximity.Near;

                case Proximity.Unknown:
                default:
                    return CLProximity.Unknown;
            }
        }
    }
}