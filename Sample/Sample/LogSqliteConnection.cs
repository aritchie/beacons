using System;
using System.IO;
using Acr.IO;
using SQLite;


namespace Sample
{
    public class DbBeaconPing
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Identifier { get; set; }
        public Guid Uuid { get; set; }
        public int? Major { get; set; }
        public int? Minor { get; set; }
        public bool Entered { get; set; }
    }


    public class LogSqliteConnection : SQLiteConnection
    {
        public LogSqliteConnection() : base(Path.Combine(FileSystem.Current.AppData.FullName, "beacons.db"), true, null)
        {
            this.CreateTable<DbBeaconPing>();
        }


        public TableQuery<DbBeaconPing> Beacons => this.Table<DbBeaconPing>();
    }
}
