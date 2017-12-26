using System;


namespace Plugin.Beacons
{
    public static class CrossBeacons
    {
        static IBeaconManager current;
        public static IBeaconManager Current
        {
            get
            {
                current = current ?? new BeaconManager();
                return current;
            }
            set => current = value;
        }
    }
}
