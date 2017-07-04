using System;
using FluentAssertions;
using Xunit;


namespace Plugin.Beacons.Tests
{
    public class BeaconParserTests
    {
        [Fact]
        public void ParseBeaconPacketSuccess()
        {
            var bytes = "4C0002159DD3C2D7C05E417492D2CC30629E522700010001C6".FromHex();
            var beacon = Beacon.Parse(bytes, 10);
            beacon.Uuid.Should().Be(new Guid("9dd3c2d7-c05e-4174-92d2-cc30629e5227"));
            beacon.Major.Should().Be(1);
            beacon.Minor.Should().Be(1);
            beacon.Proximity.Should().Be(Proximity.Immediate);
        }


        [Fact]
        public void DetectBeacon()
        {
            var bytes = "4C0002159DD3C2D7C05E417492D2CC30629E522700010001C6".FromHex();
            Beacon.IsIBeaconPacket(bytes).Should().Be(true);
        }


        [Fact]
        public void ToBeaconIsBeacon()
        {
            var beacon = new Beacon(Guid.NewGuid(), 99, 199, 0, Proximity.Far);
            var bytes = beacon.ToIBeaconPacket();
            Beacon.IsIBeaconPacket(bytes).Should().Be(true);
        }


        [Fact]
        public void ToBeacon()
        {
            var beacon = new Beacon(Guid.NewGuid(), 99, 199, 0, Proximity.Far);
            var bytes = beacon.ToIBeaconPacket();
            var beacon2 = Beacon.Parse(bytes, 0);
            beacon.Uuid.Should().Be(beacon2.Uuid);
            beacon.Major.Should().Be(beacon2.Major);
            beacon.Minor.Should().Be(beacon2.Minor);
        }
    }
}
