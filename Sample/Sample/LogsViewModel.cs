using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.Beacons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Sample
{
    public class LogsViewModel : ViewModel
    {
        readonly IBeaconManager beaconManager;
        readonly LogSqliteConnection conn;


        public LogsViewModel(IBeaconManager beaconManager,
                             IUserDialogs dialogs,
                             LogSqliteConnection conn)
        {
            this.beaconManager = beaconManager;
            this.conn = conn;

            this.Purge = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await dialogs.ConfirmAsync("Are you sure you wish to purge all logs?");
                if (result)
                {
                    this.conn.DeleteAll<DbBeaconPing>();
                    this.LoadData();
                }
            });
        }


        public ICommand Purge { get; }
        [Reactive] public IList<DbBeaconPing> Pings { get; private set; }


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
    }
}
