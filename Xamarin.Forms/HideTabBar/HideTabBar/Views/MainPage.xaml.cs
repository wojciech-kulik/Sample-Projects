using System;
using HideTabBar.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HideTabBar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : HideableTabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
}