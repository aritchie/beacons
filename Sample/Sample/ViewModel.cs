using System;
using System.Reactive.Disposables;
using Prism.AppModel;
using Prism.Navigation;
using ReactiveUI;


namespace Sample
{
    public abstract class ViewModel : ReactiveObject,
                                      IPageLifecycleAware,
                                      INavigatedAware
    {
        CompositeDisposable disposer;
        protected CompositeDisposable DisposeWith
        {
            get
            {
                if (this.disposer == null)
                    this.disposer = new CompositeDisposable();

                return this.disposer;
            }
        }


        public virtual void OnAppearing()
        {
        }


        public virtual void OnDisappearing()
        {
            this.disposer.Dispose();
            this.disposer = null;
        }


        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
        }


        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}
