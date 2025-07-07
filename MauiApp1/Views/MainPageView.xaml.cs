using System;
using MauiApp1.ViewModels;
using Poker.Core.Models;
using Microsoft.Maui.Controls;


namespace MauiApp1.Views;

    public partial class MainPage : ContentPage
{
	private MainPageViewModel ViewModel => BindingContext as MainPageViewModel;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();
	}

    private void InitializeComponent()
    {
        throw new NotImplementedException();
    }

    // Toolbar: Add Hand
    private async void OnAddHandClicked(object sender, EventArgs e)
	{
		// Navigate to Add/Edit Hand page (implement navigation as needed)
		//await Navigation.PushAsync(new HandEditPage());
	}

	// Toolbar: Receive Hand
	private async void OnReceiveHandClicked(object sender, EventArgs e)
	{
		// Navigate to Receive Hand page (for entering JSON link)
		//await Navigation.PushAsync(new ReceiveHandPage());
	}

	// When a hand is selected in the list
	private async void OnHandSelected(object sender, SelectionChangedEventArgs e)
	{
		var selectedHand = e.CurrentSelection.FirstOrDefault() as Hand;
		if (selectedHand == null)
			return;

		// Deselect so the row doesn't stay highlighted
		((CollectionView)sender).SelectedItem = null;

		// Navigate to hand details (pass the hand ID or the hand itself)
		//await Navigation.PushAsync(new HandDetailsPage(selectedHand));
	}
}
