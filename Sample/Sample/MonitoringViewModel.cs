using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.Beacons;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Sample
{
    public class MonitoringViewModel : ViewModel
    {
        readonly IBeaconManager beaconManager;


        public MonitoringViewModel(INavigationService navigationService,
                                   IUserDialogs dialogs,
                                   IBeaconManager beaconManager)
        {
            this.beaconManager = beaconManager;

            this.Add = ReactiveCommand.CreateFromTask(() => navigationService.NavigateAsync(
                "Create",
                new NavigationParameters
                {
                    { "Monitoring", true }
                }
            ));

            this.StopAllMonitoring = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await dialogs.ConfirmAsync("Are you sure you wish to stop all monitoring");
                if (result)
                {
                    this.beaconManager.StopAllMonitoring();
                    this.LoadData();
                }
            });
        }


        public ICommand Add { get; }
        public ICommand StopAllMonitoring { get; }
        [Reactive] public IList<ItemViewModel> Regions { get; private set; }


        public override void OnAppearing()
        {
            this.LoadData();
        }


        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingFrom(parameters);
            var newRegion = parameters.GetValue<BeaconRegion>(nameof(BeaconRegion));
            if (newRegion != null)
            {
                this.beaconManager.StartMonitoring(newRegion);
                this.LoadData();
            }
        }


        void LoadData()
        {
            this.Regions = this.beaconManager
                .MonitoredRegions
                .Select(x => new ItemViewModel
                {
                    Text = $"{x.Identifier}",
                    Detail = $"{x.Uuid}/{x.Major ?? 0}/{x.Minor ?? 0}",
                    Command = ReactiveCommand.Create(() =>
                    {
                        this.beaconManager.StopMonitoring(x);
                        this.LoadData();
                    })
                })
                .ToList();
        }
    }
}