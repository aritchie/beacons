using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.iBeacons;
using Plugin.Beacons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Sample.Ranging;
using Samples.Services;
using Xamarin.Forms;


namespace Samples.Beacons
{
    public class RangingViewModel : AbstractRootViewModel
    {
        IDisposable scanner;


        public RangingViewModel(ICoreServices services) : base(services)
        {
            this.ScanToggle = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var result = await this.Core.AssertLocationPermission();
                if (!result)
                    return;

                if (this.ScanText == "Scan")
                {
                    this.Beacons.Clear();
                    this.ScanText = "Stop Scan";
                    this.scanner = this.BeaconManager
                        .WhenRanged(
                            new BeaconRegion("estimote", new Guid("B9407F30-F5F8-466E-AFF9-25556B57FE6D"))
                        )
                        .Where(x => x.Beacons.Count > 0)
                        .Subscribe(rbl => Device.BeginInvokeOnMainThread(() =>
                        {
                            var group = this.Beacons.FirstOrDefault(x => x.Name.Equals(rbl.Region.Identifier));
                            if (group == null)
                            {
                                group = new Group<BeaconViewModel>(rbl.Region.Identifier);
                                foreach (var beacon in rbl.Beacons)
                                    group.Add(new BeaconViewModel(beacon));

                                this.Beacons.Add(group);
                            }
                            else
                            {
                                foreach (var beacon in rbl.Beacons)
                                {
                                    var vm = group.FirstOrDefault(x => x.Beacon.Equals(beacon));
                                    if (vm == null)
                                    {
                                        vm = new BeaconViewModel(beacon);
                                        group.Add(vm);
                                    }
                                    vm.Proximity = beacon.Proximity;
                                }
                            }
                        }));
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


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            this.StopScan();
        }


        public ICommand ScanToggle { get; }
        public ObservableCollection<Group<BeaconViewModel>> Beacons { get; } = new ObservableCollection<Group<BeaconViewModel>>();
        [Reactive] public string ScanText { get; private set; } = "Scan";
    }
}