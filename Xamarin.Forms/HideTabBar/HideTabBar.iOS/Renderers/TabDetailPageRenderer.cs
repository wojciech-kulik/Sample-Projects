using HideTabBar.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabDetailPage),
                          typeof(HideTabBar.iOS.CustomRenderer.TabDetailPageRenderer))]

namespace HideTabBar.iOS.CustomRenderer
{
    public class TabDetailPageRenderer : PageRenderer
    {
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Show TabBar when main page is displayed
            if (this.NavigationController.ViewControllers.Length == 1)
            {
                (Xamarin.Forms.Application.Current.MainPage as HideableTabbedPage).IsHidden = false;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Hide TabBar when details are displayed
            if (this.NavigationController.ViewControllers.Length > 1)
            {
                (Xamarin.Forms.Application.Current.MainPage as HideableTabbedPage).IsHidden = true;
            }
        }
    }
}
