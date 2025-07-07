using System;
using MauiApp1.ViewModels;
using Poker.Core.Models;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        private async void OnAddHandClicked(object sender, EventArgs e)
        {
            // Example of navigation to HandEdit page
            await Shell.Current.GoToAsync("//HandEdit");
        }

        private async void OnReceiveHandClicked(object sender, EventArgs e)
        {
            // Navigation or handling logic
            await Shell.Current.GoToAsync("//HandEdit");
        }

        private async void OnHandSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedHand = e.CurrentSelection.FirstOrDefault() as Hand;
            if (selectedHand == null)
                return;

            ((CollectionView)sender).SelectedItem = null;

            // Navigate to a detailed page with the selected hand's ID, if needed
            await Shell.Current.GoToAsync($"//HandDetails?handId={selectedHand.Id}");
        }
    }
}
