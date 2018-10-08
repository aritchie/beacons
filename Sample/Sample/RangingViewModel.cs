using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.Collections;
using Plugin.Beacons;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Sample
{
    public class RangingViewModel : ViewModel
    {
        readonly IBeaconManager beaconManager;
        BeaconRegion region;
        IDisposable scanner;


        public RangingViewModel(INavigationService navigationService, IBeaconManager beaconManager)
        {
            this.region = new BeaconRegion("estimote", new Guid("B9407F30-F5F8-466E-AFF9-25556B57FE6D"));
            this.beaconManager = beaconManager;

            this.Clear = ReactiveCommand.Create(() => this.Beacons.Clear());
            this.CreateRegion = ReactiveCommand.CreateFromTask(_ => navigationService.NavigateAsync("CreatePage", new NavigationParameters
            {
                { nameof(BeaconRegion), this.region }
            }));
            this.ScanToggle = ReactiveCommand.CreateFromTask(async _ =>
            {
                var result = await beaconManager.RequestPermission();
                if (!result)
                    return;

                if (this.ScanText == "Scan")
                    this.StartScan();
                else
                    this.StopScan();
            });
        }


        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var currentRegion = parameters.GetValue<BeaconRegion>(nameof(BeaconRegion));
            if (currentRegion != null)
                this.region = currentRegion;
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            if (this.region != null)
                this.StartScan();
        }


        public override void OnDisappearing()
        {
            base.OnDisappearing();
            this.StopScan();
        }


        public ICommand ScanToggle { get; }
        public ICommand CreateRegion { get; }
        public ICommand Clear { get; }

        public ObservableList<BeaconViewModel> Beacons { get; } = new ObservableList<BeaconViewModel>();
        [Reactive] public string RegionText { get; private set; }
        [Reactive] public string ScanText { get; private set; } = "Scan";


        void StartScan()
        {
            var txt = "Scanning " + this.region.Uuid;
            if (this.region.Minor != null)
                txt += $" (M: {this.region.Major}), m: {this.region.Minor})";

            else if (this.region.Major != null)
                txt += $" (M: {this.region.Major})";

            this.RegionText = txt;
            this.ScanText = "Scan";

            this.scanner = this.beaconManager
                .WhenBeaconRanged(this.region)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    var beacon = this.Beacons.FirstOrDefault(y => x == y.Beacon);
                    if (beacon == null)
                        this.Beacons.Add(new BeaconViewModel(x));
                    else
                        beacon.Proximity = x.Proximity;
                });
        }


        void StopScan()
        {
            this.RegionText = String.Empty;
            this.ScanText = "Scan";
            this.scanner?.Dispose();
        }
    }
}