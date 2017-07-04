using System;
using Foundation;


namespace Plugin.Beacons
{
    public static class Extensions
    {
        public static NSUuid ToNative(this Guid guid)
        {
            return new NSUuid(guid.ToString());
        }


        public static Guid FromNative(this NSUuid uuid)
        {
            return new Guid(uuid.AsString());
        }
    }
}
