using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Poker.Core.Models
{
    [Table("Hand")]
    public class Hand
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Guid SyncId { get; set; }

        public int? SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Stakes { get; set; }         // "1,2" (no List<int> in DB)
        public string GameType { get; set; }       // ie "NLHE"
        public int ProfitLoss { get; set; }        // Net result for Hero

        public Street CurrentStreet { get; set; }

        // NO: List<Card> HoleCards
        // NO: List<Card> BoardCards
        // NO: List<Player> Players
        // NO: List<Action> Actions

        public bool IsStraddleOn { get; set; }
        public int? StraddleAmount { get; set; }
        public int? StraddleSeatIndex { get; set; }

        public string? Location { get; set; }
        public int ButtonIndex { get; set; }
        public int SmallBlindIndex { get; set; }
        public int BigBlindIndex { get; set; }

        public int HeroPlayerId { get; set; }
        public string Note { get; set; }

        public Hand()
        {
            Timestamp = DateTime.Now;
            Stakes = string.Empty;
            CurrentStreet = Street.Preflop;
            GameType = string.Empty;
            Note = string.Empty;
            SyncId = Guid.NewGuid();
            Location = string.Empty;
        }

        [Ignore]
        public List<int> StakesList
        {
            get => string.IsNullOrWhiteSpace(Stakes)
                ? new List<int>()
                : Stakes.Split(',').Select(s => int.Parse(s)).ToList();
            set => Stakes = string.Join(",", value);
        }
    }
}
