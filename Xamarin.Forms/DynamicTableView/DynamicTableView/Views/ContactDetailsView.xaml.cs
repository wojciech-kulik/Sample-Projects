using DynamicTableView.ViewModels;
using Xamarin.Forms;

namespace DynamicTableView.Views
{
    public partial class ContactDetailsView : ContentPage
    {
        public ContactDetailsView()
        {
            InitializeComponent();
            this.BindingContext = new ContactDetailsViewModel();
        }
    }
}