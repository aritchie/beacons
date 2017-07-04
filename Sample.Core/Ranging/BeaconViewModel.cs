using System;
using Acr.iBeacons;
using ReactiveUI.Fody.Helpers;

namespace Samples.ViewModels.Beacons
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
        [Reactive] public Proximity Proximity { get; set; }
    }
}
