using System;


namespace Plugin.Beacons
{
    public static partial class CrossBeacons
    {
        static CrossBeacons()
        {
            Current = new BeaconManager();
        }
    }
}
