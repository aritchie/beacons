using System;
using Prism.AppModel;
using Prism.Navigation;
using ReactiveUI;


namespace Sample
{
    public abstract class ViewModel : ReactiveObject,
                                      IPageLifecycleAware,
                                      INavigatedAware
    {
        public virtual void OnAppearing()
        {
        }


        public virtual void OnDisappearing()
        {
        }


        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
        }


        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}
