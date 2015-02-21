using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using ClientsManager.ViewModels;
using ClientsManager.Storage;

namespace ClientsManager
{
    public class ClientsManagerBootStrapper : PhoneBootstrapper
    {
        PhoneContainer container;
        protected override void Configure()
        {
            container = new PhoneContainer(RootFrame);
            container.RegisterPhoneServices();
            container.PerRequest<IDataContextWrapper, DataContextWrapper<ClientsManagerDataContext>>();
            container.PerRequest<ClientsListViewModel>();
            container.PerRequest<ClientEditViewModel>();
            container.PerRequest<GroupsListViewModel>();
            container.PerRequest<GroupAddViewModel>();
            container.PerRequest<GroupAddClientViewModel>();
            container.PerRequest<PhotoViewModel>();
            container.PerRequest<SkyDriveFolderChooserViewModel>();
            container.PerRequest<ClientsCriteriaListViewModel>();
            container.PerRequest<ClientsCriteriaViewModel>();
        }


        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
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
            MessageBox.Show("Unexpected problem occured. Application will be closed." + 
                            (System.Diagnostics.Debugger.IsAttached ? '\n' + e.ExceptionObject.Message : ""), 
                            "Unexpected problem", MessageBoxButton.OK);
            base.OnUnhandledException(sender, e);
        }
    }
}
