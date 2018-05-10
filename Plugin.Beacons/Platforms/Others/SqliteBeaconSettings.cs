using System;
using System.Collections.Generic;
using SQLite;


namespace Plugin.Beacons
{
    public class SqliteBeaconSettings
    {
        readonly SQLiteConnection conn;


        public SqliteBeaconSettings()
        {
            this.conn = new SQLiteConnectionWithLock(
                new SQLiteConnectionString("", false),
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex
            );
        }


        public IEnumerable<BeaconRegion> GetTrackingRegions() => this.conn
            .Table<SqliteBeaconRegion>()
            .Select(x => new BeaconRegion(
                x.Identifier,
                x.Uuid,
                x.Major,
                x.Minor)
            );


        public void StartTracking(BeaconRegion region) => this.conn.Insert(new SqliteBeaconRegion
        {
            Identifier = region.Identifier,
            Uuid = region.Uuid,
            Major = region.Major,
            Minor = region.Minor
        });
        public void StopTracking(BeaconRegion region) => this.conn.Delete(region.Identifier);
        public void StopAllTracking() => this.conn.DeleteAll<SqliteBeaconRegion>();
    }
}
