using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Pizzernator.ViewModels;
using System.Windows;
using Microsoft.Phone.Shell;
using Pizzernator.GoogleAPI;
using Pizzernator.Storage;

namespace Pizzernator
{
    public class AppBootstrapper : PhoneBootstrapperBase
    {
        public AppBootstrapper()
        {
            Start();
        }

        PhoneContainer container;
        protected override void Configure()
        {
            container = new PhoneContainer();
            if (!Execute.InDesignMode)
                container.RegisterPhoneServices(RootFrame);
            container.PerRequest<MainPageViewModel>();
            container.PerRequest<IGoogleApiService, GoogleApiService>();
            container.PerRequest<PizzernatorDataContext>();
            container.PerRequest<RestaurantPreviewViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void PrepareApplication()
        {
            base.PrepareApplication();
            Resurecting = false;
        }

        public static bool Resurecting { get; set; }

        protected override void OnActivate(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                Resurecting = true;
            }
            base.OnActivate(sender, e);
        }

        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unexpected problem occured. Application will be closed. " + e.ExceptionObject.Message + " " + e.ExceptionObject.Data, "Unexpected problem", MessageBoxButton.OK);
            base.OnUnhandledException(sender, e);
        }
    }
}
