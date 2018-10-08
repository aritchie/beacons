using System;
using System.Windows.Input;
using Plugin.Beacons;
using Prism.Navigation;


namespace Sample
{
    public class MonitoringViewModel : ViewModel
    {
        readonly INavigationService navigationService;
        readonly IBeaconManager beaconManager;


        public MonitoringViewModel(INavigationService navigationService, IBeaconManager beaconManager)
        {
            this.navigationService = navigationService;
            this.beaconManager = beaconManager;
        }


        public ICommand StopAllMonitoring { get; }

        //        public MonitorViewModel(ICoreServices services) : base(services)
        //        {
        //            this.Refresh = ReactiveCommand.Create(() =>
        //            {
        //                this.IsRefreshing = true;
        //                this.LoadData();
        //                this.IsRefreshing = false;
        //            });
        //            this.WhenAnyValue(x => x.IsMonitoringEnabled)
        //                .Subscribe(async enabled =>
        //                {
        //                    var result = await this.Core.AssertLocationPermission();
        //                    if (!result)
        //                        return;

        //                    this.BeaconManager.StopAllMonitoring();
        //                    if (enabled)
        //                    {
        //                        this.BeaconManager.StartMonitoring(new BeaconRegion("estimote", new Guid("B9407F30-F5F8-466E-AFF9-25556B57FE6D")));
        //                    }
        //                });
        //        }


        //        public override void OnActivate()
        //        {
        //            base.OnActivate();
        //            this.LoadData();
        //        }


        //        void LoadData()
        //        {
        //            Task.Delay(500)
        //                .ContinueWith(_ =>
        //                {
        //                    this.List = this
        //                        .SqlConnection
        //                        .BeaconPings
        //                        .OrderByDescending(x => x.DateCreated)
        //                        .Select(x => new MonitoredViewModel(x))
        //                        .ToList();
        //                });
        //        }


        //        public ICommand Refresh { get; }

        //        [Reactive] public bool IsRefreshing { get; private set; }
        //        [Reactive] public IList<MonitoredViewModel> List { get; private set; }
        //        [Reactive] public bool IsMonitoringEnabled { get; set; }
    }
}