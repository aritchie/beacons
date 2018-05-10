using System;


namespace Plugin.Beacons
{
    public static partial class CrossBeacons
    {
        static IBeaconManager current;
        public static IBeaconManager Current
        {
            get
            {
                if (current == null)
                    throw new ArgumentException("[Plugin.Beacons] No platform plugin found.  Did you install the nuget package in your app project as well?");

                return current;
            }
            set => current = value;
        }
    }
}
