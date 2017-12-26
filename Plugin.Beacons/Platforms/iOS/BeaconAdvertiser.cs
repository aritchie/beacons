//using System;
//using CoreBluetooth;
//using CoreLocation;


//namespace Plugin.BeaconAds
//{
//    public class BeaconAdvertiser : IBeaconAdvertiser
//    {
//        readonly CBPeripheralManager manager;


//        public BeaconAdvertiser()
//        {
//            this.manager = new CBPeripheralManager();
//        }


//        public AdapterStatus AdapterStatus
//        {
//            get
//            {
//                switch (this.manager.State)
//                {
//                    case CBPeripheralManagerState.PoweredOff:
//                        return AdapterStatus.PoweredOff;

//                    case CBPeripheralManagerState.PoweredOn:
//                        return AdapterStatus.PoweredOn;

//                    case CBPeripheralManagerState.Resetting:
//                        return AdapterStatus.Resetting;

//                    case CBPeripheralManagerState.Unauthorized:
//                        return AdapterStatus.Unauthorized;

//                    case CBPeripheralManagerState.Unsupported:
//                        return AdapterStatus.Unsupported;

//                    case CBPeripheralManagerState.Unknown:
//                    default:
//                        return AdapterStatus.Unknown;
//                }
//            }
//        }


//        public Beacon AdvertisedBeacon { get; private set; }
//        public void Start(Beacon beacon)
//        {
//            var native = new CLBeaconRegion(beacon.Uuid.ToNative(), beacon.Major, beacon.Minor, "acr");
//            var payload = native.GetPeripheralData(null); // TODO: takes txpower :)
//            this.manager.StartAdvertising(payload);
//            this.AdvertisedBeacon = beacon;
//        }


//        public void Stop()
//        {
//            this.manager.StopAdvertising();
//        }
//    }
//}
