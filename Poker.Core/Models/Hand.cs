using System;
using System.Collections.Generic;

namespace Poker.Core.Models;

public class Hand
    {
        public Guid Id { get; set; }
        public Guid? SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public List<int> Stakes { get; set; } // e.g., [1,2] for $1/$2
        public string GameType { get; set; } // e.g., "NLHE"
        public int ProfitLoss { get; set; } // Net result for Hero

        public Street CurrentStreet { get; set; }

        public List<Card> HoleCards { get; set; } // Hero's hole cards
        public List<Card> BoardCards { get; set; } // Flop, turn, river

        public bool IsStraddleOn { get; set; }
        public int? StraddleAmount { get; set; } // nullable for "no straddle"
        public int? StraddleSeatIndex { get; set; }
        public List<Player> Players { get; set; }
        public List<Action> Actions { get; set; }

        public int ButtonIndex { get; set; }
        public int SmallBlindIndex { get; set; }
        public int BigBlindIndex { get; set; }

        public Guid HeroPlayerId { get; set; }
        public string Note { get; set; }

        public Hand()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
            Stakes = new List<int>();
            CurrentStreet = Street.Preflop;
            GameType = string.Empty;
            HoleCards = new List<Card>();
            BoardCards = new List<Card>();
            Players = new List<Player>();
            Actions = new List<Action>();
            Note = string.Empty;
        }
    }