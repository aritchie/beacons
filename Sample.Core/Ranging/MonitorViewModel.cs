using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr;
using Acr.iBeacons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Services;


namespace Samples.ViewModels.Beacons
{
    public class MonitorViewModel : AbstractRootViewModel
    {
        public MonitorViewModel(ICoreServices services) : base(services)
        {
            this.Refresh = new Command(() =>
            {
                this.IsRefreshing = true;
                this.LoadData();
                this.IsRefreshing = false;
            });
            this.WhenAnyValue(x => x.IsMonitoringEnabled)
                .Subscribe(async enabled =>
                {
                    var result = await this.Core.AssertLocationPermission();
                    if (!result)
                        return;

                    this.BeaconManager.StopAllMonitoring();
                    if (enabled)
                    {
                        this.BeaconManager.StartMonitoring(new BeaconRegion("estimote", new Guid("B9407F30-F5F8-466E-AFF9-25556B57FE6D")));
                    }
                });
        }


        public override void OnActivate()
        {
            base.OnActivate();
            this.LoadData();
        }


        void LoadData()
        {
            Task.Delay(500)
                .ContinueWith(_ =>
                {
                    this.List = this
                        .SqlConnection
                        .BeaconPings
                        .OrderByDescending(x => x.DateCreated)
                        .Select(x => new MonitoredViewModel(x))
                        .ToList();
                });
        }


        public ICommand Refresh { get; }

        [Reactive] public bool IsRefreshing { get; private set; }
        [Reactive] public IList<MonitoredViewModel> List { get; private set; }
        [Reactive] public bool IsMonitoringEnabled { get; set; }
    }
}