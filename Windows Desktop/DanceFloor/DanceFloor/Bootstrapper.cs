using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using DanceFloor.ViewModels;
using Caliburn.Micro;
using System.Reflection;
using Common;
using ApplicationServices;
using GameLayer;

namespace DanceFloor
{
    public class Bootstrapper : Bootstrapper<MainWindowViewModel>
    {
        private SimpleContainer container;

        protected override void Configure()
        {
            container = new SimpleContainer();
            //TODO: handle animation after resizing window
            
            //Caliburn.Micro
            container.PerRequest<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            
            //ViewModels
            container.PerRequest<MainWindowViewModel>();
            container.PerRequest<MenuViewModel>();
            container.PerRequest<GameViewModel>();
            container.PerRequest<SongsListViewModel>();
            container.PerRequest<GameModeViewModel>();
            container.PerRequest<RecordSequenceViewModel>();
            container.PerRequest<RecordOptionsViewModel>();

            //Popups
            container.PerRequest<ClosingPopupViewModel>();
            container.PerRequest<GameOverPopupViewModel>();
            container.PerRequest<CountdownPopupViewModel>();
            container.PerRequest<ButtonsPopupViewModel>();

            //Game models
            container.PerRequest<IGame, Game>();
            container.PerRequest<ISong, Song>();
            
            //Services
            container.Singleton<ISongsService, SongsService>();
            container.PerRequest<IMusicPlayerService, MusicPlayerService>();
            container.PerRequest<IHighScoresService, HighScoresService>();
            container.Singleton<ISettingsService, SettingsService>();            
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
    }
}