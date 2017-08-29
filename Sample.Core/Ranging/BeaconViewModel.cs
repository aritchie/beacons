using System;
using Plugin.Beacons;
using ReactiveUI;


namespace Sample.Ranging
{
    public class BeaconViewModel : AbstractViewModel
    {
        public BeaconViewModel(Beacon beacon)
        {
            this.Beacon = beacon;
            this.Proximity = beacon.Proximity; // initial value
        }


        public Beacon Beacon { get; }
        public ushort Major => this.Beacon.Major;
        public ushort Minor => this.Beacon.Minor;
        public string Identifier => $"Major: {this.Major} - Minor: {this.Minor}";


        Proximity prox;
        public Proximity Proximity
        {
            get => this.prox;
            set => this.RaiseAndSetIfChanged(ref this.prox, value);
        }
    }
}
