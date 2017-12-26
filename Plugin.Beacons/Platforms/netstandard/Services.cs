using System;


namespace Plugin.Beacons
{
    public static class Services
    {
        public static IBeaconSettings Settings { get; set; } = new SqliteBeaconSettings();
    }
}
