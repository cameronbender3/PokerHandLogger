using System;
using SQLite;

namespace Poker.Core.Models
{
    [Table("Action")]
    public class Action
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Primary key for Action

        public int HandId { get; set; }   // Foreign key to Hand
        public int PlayerId { get; set; } // Foreign key to Player

        public ActionType ActionType { get; set; }
        public int Amount { get; set; }
        public Street Street { get; set; }
        public int Sequence { get; set; }
        public bool IsAutoFilled { get; set; }

        public Action()
        {
            ActionType = ActionType.Fold;
            Amount = 0;
            Street = Street.Preflop;
            Sequence = 0;
            IsAutoFilled = false;
        }
    }

    public enum ActionType
    {
        Check,
        Call,
        Bet,
        Raise,
        Fold,
        Straddle
    }

    public enum Street
    {
        Preflop,
        Flop,
        Turn,
        River
    }
}
