using System;
using System.Collections.Generic;
using System.Linq;
using Acr.Settings;


namespace Plugin.Beacons
{
    public class BeaconSettings : AbstractSettingObject
    {
        public static BeaconSettings Current { get; }

        static BeaconSettings()
        {
            Current = Settings.Settings.Local.Bind<BeaconSettings>();
        }


        public List<BeaconRegion> MonitorRegions { get; set; } = new List<BeaconRegion>();


        public void Add(BeaconRegion region)
        {
            this.MonitorRegions.Add(region);
            this.OnPropertyChanged("MonitorRegions");
        }


        public void Remove(BeaconRegion region)
        {
            if (this.MonitorRegions.Remove(region))
                this.OnPropertyChanged("MonitorRegions");
        }


        public void Clear()
        {
            if (!this.MonitorRegions.Any())
                return;

            this.MonitorRegions.Clear();
            this.OnPropertyChanged("MonitorRegions");
        }
    }
}
