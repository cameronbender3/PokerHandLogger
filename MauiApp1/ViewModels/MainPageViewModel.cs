using System.Collections.ObjectModel;
using Poker.Core.Models;
namespace MauiApp1.ViewModels;

 public class MainPageViewModel
    {
        // ObservableCollection so the UI updates when items are added/removed
        public ObservableCollection<Hand> Hands { get; set; }

        public MainPageViewModel()
    {
        // Demo/mock data - replace with real data loading soon
        Hands = new ObservableCollection<Hand>
            {
                new Hand
                {
                    Id = 1,
                    Location = "Seminole Hard Rock",
                    ProfitLoss = 120,
                    Stakes = "2,5",
                    GameType = "NLHE",
                    Timestamp = DateTime.Now,
                    // For simplicity, you can set Hero's cards as a string or however you plan to show it
                },
                new Hand
                {
                    Id = 2,
                    Location = "Home Game",
                    ProfitLoss = -50,
                    Stakes = "1,2",
                    GameType = "PLO",
                    Timestamp = DateTime.Now.AddDays(-1),
                }
            };
    }
    }