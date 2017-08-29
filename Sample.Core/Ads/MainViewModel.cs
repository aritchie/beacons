//using System;
//using System.Reactive.Linq;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Acr.UserDialogs;
//using Plugin.Beacons;
//using ReactiveUI;


//namespace Sample
//{
//    public class MainViewModel : ReactiveObject
//    {
//        public MainViewModel()
//        {
//            this.WhenAnyValue(x => x.Major)
//                .Skip(1)
//                .Subscribe(x => this.Major = Fix(x));

//            this.WhenAnyValue(x => x.Minor)
//                .Skip(1)
//                .Subscribe(x => this.Minor = Fix(x));

//            this.ToggleAdvertisement = ReactiveCommand.CreateFromTask(_ =>
//            {
//                try
//                {
//                    if (CrossBeacons.Current.AdvertisedBeacon != null)
//                    {
//                            this.StartStopText = "Start Advertising";
//                            CrossBeaconAds.Current.Stop();
//                    }
//                    else
//                    {
//                        this.StartStopText = "Stop Advertising";
//                        CrossBeacon.Current.StartAdvertising(new Beacon(
//                            Guid.Parse(this.Uuid),
//                            Convert.ToUInt16(this.Major),
//                            Convert.ToUInt16(this.Minor)
//                        ));
//                    }
//                }
//                catch (Exception ex)
//                {
//                    UserDialogs.Instance.Alert("ERROR - " + ex);
//                }
//                return Task.FromResult(new object());
//            },
//            this.WhenAny(
//                x => x.Uuid,
//                uuid =>
//                {
//                    var guid = default(Guid);
//                    return Guid.TryParse(uuid.Value, out guid);
//                }
//            ));
//        }


//        static int Fix(int value)
//        {
//            if (value <= 0)
//                return 1;

//            if (value > ushort.MaxValue)
//                value = ushort.MaxValue;

//            return value;
//        }


//        public ICommand ToggleAdvertisement { get; }
//        [Reactive] public string Uuid { get; set; } = "B9407F30-F5F8-466E-AFF9-25556B57FE6D"; // default estimote uuid
//        [Reactive] public int Major { get; set; } = 1;
//        [Reactive] public int Minor { get; set; } = 1;
//        [Reactive] public string StartStopText { get; private set; } = "Start Advertising";
//    }
//}
