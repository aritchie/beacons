using System;
using Acr.UserDialogs;
using Autofac;
using Plugin.Beacons;
using Samples;


namespace Sample
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<LogSqliteConnection>().AsSelf().SingleInstance();
            builder.RegisterType<GlobalExceptionHandler>().As<IStartable>().AutoActivate().SingleInstance();
            builder.RegisterType<MonitoringTask>().As<IStartable>().AutoActivate().SingleInstance();
            builder.Register(_ => UserDialogs.Instance).As<IUserDialogs>().SingleInstance();
            builder.Register(_ => CrossBeacons.Current).As<IBeaconManager>().SingleInstance();
        }
    }
}
