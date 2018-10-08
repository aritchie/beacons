using System;
using System.Windows.Input;
using Plugin.Beacons;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Sample
{
    public class CreateViewModel : ViewModel
    {
        public CreateViewModel(INavigationService navigationService)
        {
            this.Title = "Create Region";

            this.Create = ReactiveCommand.CreateFromTask(
                _ => navigationService.GoBackAsync(new NavigationParameters
                {
                    { nameof(BeaconRegion), new BeaconRegion(this.Identifier, Guid.Parse(this.Uuid), this.Major, this.Minor)}
                }),
                this.WhenAny(
                    x => x.Identifier,
                    x => x.Uuid,
                    x => x.Major,
                    x => x.Minor,
                    (id, uuid, M, m) =>
                    {
                        if (String.IsNullOrWhiteSpace(id.GetValue()))
                            return false;

                        if (!Guid.TryParse(uuid.GetValue(), out _))
                            return false;

                        var mv = m.GetValue();
                        if (mv < 0 || mv > ushort.MaxValue)
                            return false;

                        var Mv = M.GetValue();
                        if (Mv < 0 || Mv > ushort.MaxValue)
                            return false;

                        return true;
                    }
                )
            );
        }


        public ICommand Create { get; }
        [Reactive] public string Title { get; private set; }
        [Reactive] public string Identifier { get; set; }
        [Reactive] public string Uuid { get; set; }
        [Reactive] public ushort Major { get; set; }
        [Reactive] public ushort Minor { get; set; }
    }
}
