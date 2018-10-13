using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Plugin.Beacons;
using ReactiveUI.Fody.Helpers;


namespace Sample
{
    public class LogsViewModel : ViewModel
    {
        readonly IBeaconManager beaconManager;
        readonly LogSqliteConnection conn;


        public LogsViewModel(IBeaconManager beaconManager, LogSqliteConnection conn)
        {
            this.beaconManager = beaconManager;
            this.conn = conn;
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            this.LoadData();
            this.beaconManager
                .WhenRegionStatusChanged()
                .Subscribe(_ => this.LoadData())
                .DisposeWith(this.DisposeWith);
        }


        void LoadData()
        {
            this.Pings = this.conn
                .Beacons
                .OrderByDescending(x => x.CreatedOn)
                .ToList();
        }


        [Reactive] public IList<DbBeaconPing> Pings { get; private set; }
    }
}
