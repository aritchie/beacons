using System;
using Autofac;
using Plugin.Beacons;


namespace Sample
{
    public class MonitoringTask : IStartable
    {
        readonly IBeaconManager beaconManager;
        readonly LogSqliteConnection conn;


        public MonitoringTask(IBeaconManager beaconManager, LogSqliteConnection conn)
        {
            this.beaconManager = beaconManager;
            this.conn = conn;
        }


        public void Start() => this.beaconManager
            .WhenRegionStatusChanged()
            .Subscribe(x => this.conn.Insert(
                new DbBeaconPing
                {
                    Identifier = x.Region.Identifier,
                    Uuid = x.Region.Uuid,
                    Minor = x.Region.Minor,
                    Major = x.Region.Major,
                    Entered = x.IsEntering
                }
            )
        );
    }
}
