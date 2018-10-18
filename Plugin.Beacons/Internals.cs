using System;
using System.Reactive.Linq;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace Plugin.Beacons
{
    static class Internals
    {

        public static IObservable<bool> HasPermission() => Observable.FromAsync(async _ =>
        {
            var values = await CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationAlways);
            return values[Permission.LocationAlways] == PermissionStatus.Granted;
        });
    }
}
