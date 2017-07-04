using System;
using System.Reactive.Subjects;
using Plugin.BluetoothLE;
using Moq;


namespace Plugin.Beacons.Tests
{
    public class BeaconManagerMocks
    {
        readonly Subject<IScanResult> scanSubject;


        public BeaconManagerMocks()
        {
            this.scanSubject = new Subject<IScanResult>();
            this.BleAdapter = new Mock<IAdapter>();
            this.BleAdapter.Setup(x => x.Scan()).Returns(this.scanSubject);
            this.Manager = new BeaconManagerImpl(new BeaconSettings(), this.BleAdapter.Object);
        }


        public Mock<IAdapter> BleAdapter { get; }
        public IBeaconManager Manager { get; }


        public void SendRange(Beacon beacon, IDevice device = null)
        {
            if (device == null)
            {
                var dev = new Mock<IDevice>();
                dev.Setup(x => x.Uuid).Returns(Guid.NewGuid());
                dev.Setup(x => x.Name).Returns("test");
                device = dev.Object;
            }

            var bytes = beacon.ToIBeaconPacket();
            var ad = new Mock<IAdvertisementData>();
            ad.Setup(x => x.ManufacturerData).Returns(bytes);
            var scanResult = new ScanResult(device, 0, ad.Object);
            this.scanSubject.OnNext(scanResult);
        }
    }
}
