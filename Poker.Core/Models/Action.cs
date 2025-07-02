using System;

namespace Poker.Core.Models;

public class Action
    {
        public Guid PlayerId { get; set; }
        public ActionType ActionType { get; set; }
        public int Amount { get; set; }
        public Street Street { get; set; }
        public int Sequence { get; set; }
        public bool IsAutoFilled { get; set; }

        public Action()
        {
            PlayerId = Guid.Empty;
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
