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
#if BAIT
                if (current == null)
                    throw new ArgumentException("[Plugin.Beacons] No platform plugin found.  Did you install the nuget package in your app project as well?");
#else
                current = current ?? new BeaconManager();
#endif
                return current;
            }
            set => current = value;
        }
    }
}
