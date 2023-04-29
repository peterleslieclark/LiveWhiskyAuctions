using LiveWhiskyAuctions.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace LiveWhiskyAuctions.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}