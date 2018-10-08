using System;
using Plugin.Beacons;


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
        }


        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }


        void LoadData()
        {

        }
    }
}
