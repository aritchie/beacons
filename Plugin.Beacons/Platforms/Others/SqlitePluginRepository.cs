using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Acr.IO;
using SQLite;


namespace Plugin.Beacons
{
    public class SqlitePluginRepository : IPluginRepository
    {
        readonly SQLiteConnection conn;


        public SqlitePluginRepository()
        {
            this.conn = new SQLiteConnectionWithLock(
                new SQLiteConnectionString(Path.Combine(FileSystem.Current.AppData.FullName, "beaconplugin.db"), true, null),
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex
            );
        }


        public IEnumerable<BeaconRegion> GetTrackingRegions() => this.conn
            .Table<DbBeaconRegion>()
            .Select(x => new BeaconRegion(
                x.Identifier,
                x.Uuid,
                x.Major,
                x.Minor
            ));


        public void StartTracking(BeaconRegion region) => this.conn.Insert(new DbBeaconRegion
        {
            Identifier = region.Identifier,
            Uuid = region.Uuid,
            Major = region.Major,
            Minor = region.Minor
        });
        public void StopTracking(BeaconRegion region) => this.conn.Delete(region.Identifier);
        public void StopAllTracking() => this.conn.DeleteAll<DbBeaconRegion>();
    }


    public class DbBeaconRegion
    {
        [PrimaryKey]
        public string Identifier { get; set; }

        public Guid Uuid { get; set; }
        public ushort? Major { get; set; }
        public ushort? Minor { get; set; }
    }
}
