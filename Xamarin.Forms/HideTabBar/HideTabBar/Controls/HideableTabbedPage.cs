using System;
using Xamarin.Forms;

namespace HideTabBar.Controls
{
    public class HideableTabbedPage : TabbedPage
    {
        public static readonly BindableProperty IsHiddenProperty =
            BindableProperty.Create(nameof(IsHidden), typeof(bool), typeof(HideableTabbedPage), false);

        public bool IsHidden
        {
            get { return (bool)GetValue(IsHiddenProperty); }
            set { SetValue(IsHiddenProperty, value); }
        }
    }
}
