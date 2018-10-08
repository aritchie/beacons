using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Plugin.Beacons;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Sample
{
    public class RangingViewModel : ViewModel
    {
        IDisposable scanner;


        public RangingViewModel(INavigationService navigationService, IBeaconManager beaconManager)
        {
            this.ScanToggle = ReactiveCommand.CreateFromTask(async _ =>
            {
                var result = await beaconManager.RequestPermission();
                if (!result)
                    return;

                if (this.ScanText == "Scan")
                {
                    //this.Beacons.Clear();
                    this.ScanText = "Stop Scan";
                    this.scanner = beaconManager
                        .WhenRanged(
                            new BeaconRegion("estimote", new Guid("B9407F30-F5F8-466E-AFF9-25556B57FE6D"))
                        )
                        //.Where(x => x.Beacons.Count > 0)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(x =>
                        {
                            //var group = this.Beacons.FirstOrDefault(x => x.Name.Equals(rbl.Region.Identifier));
                            //if (group == null)
                            //{
                            //    group = new Group<BeaconViewModel>(rbl.Region.Identifier);
                            //    foreach (var beacon in rbl.Beacons)
                            //        group.Add(new BeaconViewModel(beacon));

                            //    this.Beacons.Add(group);
                            //}
                            //else
                            //{
                            //    foreach (var beacon in rbl.Beacons)
                            //    {
                            //        var vm = group.FirstOrDefault(x => x.Beacon.Equals(beacon));
                            //        if (vm == null)
                            //        {
                            //            vm = new BeaconViewModel(beacon);
                            //            group.Add(vm);
                            //        }
                            //        vm.Proximity = beacon.Proximity;
                            //    }
                            //}
                        });
                }
                else
                {
                    this.StopScan();
                }
            });
        }


        void StopScan()
        {
            this.ScanText = "Scan";
            this.scanner?.Dispose();
        }


        public override void OnDisappearing()
        {
            base.OnDisappearing();
            this.StopScan();
        }


        public ICommand ScanToggle { get; }
        [Reactive] public string ScanText { get; private set; } = "Scan";
    }
}