# ACR Reactive Beacons Plugin for Xamarin & Windows

Scans for iBeacons

[![NuGet](https://img.shields.io/nuget/v/Plugin.Beacons.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.Beacons/)
[![Build status](https://allanritchie.visualstudio.com/Plugins/_apis/build/status/Beacons)](https://allanritchie.visualstudio.com/Plugins/_build/latest?definitionId=0)

[Change Log](changelog.md)

|Platform|Version|
|--------|-------|
|iOS|8+|
|Android|4.3+|
|Windows UWP|16299+|

### [SUPPORT THIS PROJECT](https://github.com/aritchie/home)

## SETUP

Install the following nuget package to all of your platform code and PCL/Core libraries

[![NuGet](https://img.shields.io/nuget/v/Plugins.Beacons.svg?maxAge=2592000)](https://www.nuget.org/packages/Acr.Ble/)

**Android - add the following to your Android app manifest**
```xml
<uses-permission android:name="android.permission.BLUETOOTH"/>
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN"/>

<!--this is necessary for Android v6+ to get the device name and address-->
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
```

**iOS - add the following to your Info.plist if monitoring**
_Add the following for iBeacon background scanning_
```xml 
<key>NSLocationAlwaysUsageDescription</key>
<string>The beacons always have you!</string>
```

```xml    
<array>
<string>bluetooth-central</string>
</array>

<!--To add a description to the Bluetooth request message (on iOS 10 this is required!)-->
  
<key>NSBluetoothPeripheralUsageDescription</key>
<string>YOUR CUSTOM MESSAGE</string>
```

## HOW TO USE

### Ranging for Beacons

var scanner = CrossBeacons.Current.Scan().Subscribe(scanResult => {
    // do something with it
});

scanner.Dispose(); // to stop scanning

### Background Monitoring for Beacons

Once you have successfully scanned for a device, use the instance

    Device.WhenServicesDiscovered().Subscribe(service => 
    {
        service.W
    });

## FAQ
Q) Why is everything reactive instead of events/async
> I wanted event streams as I was scanning devices.  I also wanted to throttle things like characteristic notification feeds.  Lastly, was the proper cleanup of events and resources.   

Q) Why can't I scan for all beacons (no uuid)
> Because this isn't really how beacons are intended to work, so I haven't exposed this functionality intentionally (nor will I take a FR/PR for it)!

Q) How many region configurations can I scan for at a time.
> On iOS, 20 max.  You shouldn't really ever go beyond this on other platforms either

Q) Can I scan for Eddystones with this library
> No, as the title of this library says, it is currently for iBeacons only!