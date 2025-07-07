using System;
using SQLite;

namespace Poker.Core.Models
{
    [Table("Player")]
    public class Player
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int HandId { get; set; } // Foreign key to Hand

        public string Name { get; set; } = string.Empty;
        public int InitialStack { get; set; }
        public bool IsHero { get; set; }
        public int SeatIndex { get; set; }

        // For storage, use a comma-separated string for two hole cards, e.g., "As,Kd"
        public string HoleCards { get; set; } = string.Empty;

        public PlayerType Type { get; set; } = PlayerType.Unknown;

        public Player() { }
    }

    public enum PlayerType
    {
        Unknown,
        Fish,
        Whale,
        Shark,
        Nit,
        Reg
    }
}
