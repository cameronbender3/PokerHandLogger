using System;
using MauiApp1.ViewModels;
using Poker.Core.Models;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;


namespace MauiApp1;

    public partial class MainPage : ContentPage
{
	private MainPageViewModel ViewModel => BindingContext as MainPageViewModel;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();
	}


	private async void OnAddHandClicked(object sender, EventArgs e)
	{
		await DisplayAlert("Add Hand", "This will open the Hand Edit screen.", "OK");
	}


	private async void OnReceiveHandClicked(object sender, EventArgs e)
	{
    await DisplayAlert("Add Hand", "This will open the Hand Edit screen.", "OK");
	}

	private async void OnHandSelected(object sender, SelectionChangedEventArgs e)
	{
		var selectedHand = e.CurrentSelection.FirstOrDefault() as Hand;
		if (selectedHand == null)
			return;

		((CollectionView)sender).SelectedItem = null;

    await DisplayAlert("Add Hand", "This will open the Hand Edit screen.", "OK");
	}
}
