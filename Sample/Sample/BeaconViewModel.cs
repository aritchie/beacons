using System;
using Plugin.Beacons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Sample
{
    public class BeaconViewModel : ReactiveObject
    {
        public BeaconViewModel(Beacon beacon)
        {
            this.Beacon = beacon;
            this.Proximity = beacon.Proximity;
        }


        public Beacon Beacon { get; }
        public ushort Major => this.Beacon.Major;
        public ushort Minor => this.Beacon.Minor;
        public string Identifier => $"Major: {this.Major} - Minor: {this.Minor}";
        [Reactive] public Proximity Proximity { get; set; }
    }
}
