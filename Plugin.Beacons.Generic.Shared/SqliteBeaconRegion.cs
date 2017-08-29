using System;
using SQLite;


namespace Plugin.Beacons
{
    public class SqliteBeaconRegion
    {
        [PrimaryKey]
        public string Identifier { get; set; }


        public Guid Uuid { get; set; }
        public ushort? Major { get; set; }
        public ushort? Minor { get; set; }
    }
}
