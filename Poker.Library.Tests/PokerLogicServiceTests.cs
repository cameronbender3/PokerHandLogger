using System;
using System.Collections.Generic;
using Poker.Core.Models;
using Poker.Core.Services; 
using Xunit;

namespace Poker.Library.Tests;

public class PokerLogicServiceTests
{
    private readonly PokerLogicService _logic;

    public PokerLogicServiceTests()
    {
        _logic = new PokerLogicService();
    }

    [Fact]
    public void FullHand_ActionFlow_MatchesExpectedActions()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 },
            new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        };

        var hand = new Hand
        {
            Id = 1,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop
        };
        hand.StakesList = new List<int> { 1, 2 };

        var sb = players[0];
        var bb = players[1];
        var btn = players[2];
        int seq = 1;

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = sb.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 5, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Call, Amount = 5, Street = Street.Preflop, Sequence = seq++ });

        hand.CurrentStreet = Street.Flop;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Flop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 5, Street = Street.Flop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Raise, Amount = 15, Street = Street.Flop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Call, Amount = 15, Street = Street.Flop, Sequence = seq++ });

        hand.CurrentStreet = Street.Turn;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Turn, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Turn, Sequence = seq++ });

        hand.CurrentStreet = Street.River;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.River, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 10, Street = Street.River, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.River, Sequence = seq++ });

        Assert.Equal(ActionType.Fold, hand.Actions[0].ActionType);
        Assert.Equal(ActionType.Raise, hand.Actions[1].ActionType);
        Assert.Equal(ActionType.Call, hand.Actions[2].ActionType);
        Assert.Equal(ActionType.Check, hand.Actions[3].ActionType);
        Assert.Equal(ActionType.Raise, hand.Actions[4].ActionType);
        Assert.Equal(ActionType.Raise, hand.Actions[5].ActionType);
        Assert.Equal(ActionType.Call, hand.Actions[6].ActionType);
        Assert.Equal(ActionType.Check, hand.Actions[7].ActionType);
        Assert.Equal(ActionType.Check, hand.Actions[8].ActionType);
        Assert.Equal(ActionType.Check, hand.Actions[9].ActionType);
        Assert.Equal(ActionType.Raise, hand.Actions[10].ActionType);
        Assert.Equal(ActionType.Fold, hand.Actions[11].ActionType);
    }

    private Hand CreateBasicHand()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100, IsHero = false },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100, IsHero = false },
            new Player { Id = 3, SeatIndex = 2, InitialStack = 100, IsHero = false },
            new Player { Id = 4, SeatIndex = 3, InitialStack = 100, IsHero = false },
        };
        var hand = new Hand
        {
            Id = 10,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop
        };
        hand.StakesList = new List<int> { 1, 2 };
        return hand;
    }

    [Fact]
    public void SimpleHeadsUp_HandEndsOnFlop()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 2,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop
        };
        hand.StakesList = new List<int> { 1, 2 };

        var sb = players[0];
        var bb = players[1];
        int seq = 1;

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = sb.Id, ActionType = ActionType.Call, Amount = 2, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Preflop, Sequence = seq++ });

        hand.CurrentStreet = Street.Flop;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = sb.Id, ActionType = ActionType.Raise, Amount = 4, Street = Street.Flop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Flop, Sequence = seq++ });

        Assert.Equal(ActionType.Call, hand.Actions[0].ActionType);
        Assert.Equal(ActionType.Check, hand.Actions[1].ActionType);
        Assert.Equal(ActionType.Raise, hand.Actions[2].ActionType);
        Assert.Equal(ActionType.Fold, hand.Actions[3].ActionType);
    }

    [Fact]
    public void FourWay_AllPlayersAct_PreflopBigRaise_ThreeFolds()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 },
            new Player { Id = 3, SeatIndex = 2, InitialStack = 100 },
            new Player { Id = 4, SeatIndex = 3, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 3,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop
        };
        hand.StakesList = new List<int> { 1, 2 };

        var utg = players[0];
        var mp = players[1];
        var co = players[2];
        var btn = players[3];
        int seq = 1;

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = utg.Id, ActionType = ActionType.Raise, Amount = 10, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = mp.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Call, Amount = 10, Street = Street.Preflop, Sequence = seq++ });

        hand.CurrentStreet = Street.Flop;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = utg.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Flop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 20, Street = Street.Flop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = utg.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Flop, Sequence = seq++ });

        Assert.Equal(ActionType.Raise, hand.Actions[0].ActionType);
        Assert.Equal(ActionType.Fold, hand.Actions[1].ActionType);
        Assert.Equal(ActionType.Fold, hand.Actions[2].ActionType);
        Assert.Equal(ActionType.Call, hand.Actions[3].ActionType);
        Assert.Equal(ActionType.Check, hand.Actions[4].ActionType);
        Assert.Equal(ActionType.Raise, hand.Actions[5].ActionType);
        Assert.Equal(ActionType.Fold, hand.Actions[6].ActionType);
    }

    [Fact]
    public void ThreeWay_TwoFoldsOnTurn_HeadsUpToRiver()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 },
            new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 4,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop
        };
        hand.StakesList = new List<int> { 1, 2 };

        var hj = players[0];
        var co = players[1];
        var btn = players[2];
        int seq = 1;

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = hj.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Call, Amount = 2, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 8, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Call, Amount = 8, Street = Street.Preflop, Sequence = seq++ });

        hand.CurrentStreet = Street.Flop;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Flop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Flop, Sequence = seq++ });

        hand.CurrentStreet = Street.Turn;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Raise, Amount = 10, Street = Street.Turn, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Turn, Sequence = seq++ });

        Assert.Equal(ActionType.Fold, hand.Actions[0].ActionType);
        Assert.Equal(ActionType.Call, hand.Actions[1].ActionType);
        Assert.Equal(ActionType.Raise, hand.Actions[2].ActionType);
        Assert.Equal(ActionType.Call, hand.Actions[3].ActionType);
        Assert.Equal(ActionType.Check, hand.Actions[4].ActionType);
        Assert.Equal(ActionType.Check, hand.Actions[5].ActionType);
        Assert.Equal(ActionType.Raise, hand.Actions[6].ActionType);
        Assert.Equal(ActionType.Fold, hand.Actions[7].ActionType);
    }

    [Fact]
    public void UndoLastAction_RemovesUserActionAndPrecedingAutos()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 },
            new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 5,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop
        };
        hand.StakesList = new List<int> { 1, 2 };
        int seq = 1;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = true });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = true });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[2].Id, ActionType = ActionType.Call, Amount = 5, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });

        _logic.UndoLastAction(hand);

        Assert.Empty(hand.Actions);
    }

    [Fact]
    public void GetPotSize_SumsCorrectly()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 6,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Flop
        };
        hand.StakesList = new List<int> { 1, 2 };
        int seq = 1;
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Call, Amount = 2, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Raise, Amount = 4, Street = Street.Flop, Sequence = seq++ });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Call, Amount = 4, Street = Street.Flop, Sequence = seq++ });

        int potPreflop = _logic.GetPotSize(hand, Street.Preflop);
        int potFlop = _logic.GetPotSize(hand, Street.Flop);

        Assert.Equal(2, potPreflop);
        Assert.Equal(10, potFlop);
    }

    [Fact]
    public void GetMinimumRaiseAmount_WorksForPreflopAndPostflop()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 7,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop
        };
        hand.StakesList = new List<int> { 1, 2 };

        int minRaisePreflop = _logic.GetMinimumRaiseAmount(hand, players[0]);
        Assert.Equal(2, minRaisePreflop);

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Raise, Amount = 8, Street = Street.Preflop, Sequence = 1 });
        int minRaiseAfterFirstRaise = _logic.GetMinimumRaiseAmount(hand, players[1]);
        Assert.Equal(16, minRaiseAfterFirstRaise);

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Raise, Amount = 20, Street = Street.Preflop, Sequence = 2 });
        int minRaiseAfterSecondRaise = _logic.GetMinimumRaiseAmount(hand, players[0]);
        Assert.Equal(32, minRaiseAfterSecondRaise);
    }

    [Fact]
    public void ThreeWay_AllInOnFlop_CorrectPotSize()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 20 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 30 },
            new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 8,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Flop
        };
        hand.StakesList = new List<int> { 1, 2 };
        int seq = 1;

        foreach (var p in players)
            hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = p.Id, ActionType = ActionType.Call, Amount = 2, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Raise, Amount = 18, Street = Street.Flop, Sequence = seq++, IsAutoFilled = false });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Raise, Amount = 28, Street = Street.Flop, Sequence = seq++, IsAutoFilled = false });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[2].Id, ActionType = ActionType.Call, Amount = 28, Street = Street.Flop, Sequence = seq++, IsAutoFilled = false });

        int pot = _logic.GetPotSize(hand, Street.Flop);
        Assert.Equal(80, pot);
    }

    [Fact]
    public void Preflop_StraddleAndRaise_CorrectMinimumRaise()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 },
            new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 9,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop,
            IsStraddleOn = true,
            StraddleAmount = 4
        };
        hand.StakesList = new List<int> { 1, 2 };
        int seq = 1;

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Raise, Amount = 10, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });

        int minRaise = _logic.GetMinimumRaiseAmount(hand, players[1]);
        Assert.Equal(20, minRaise);
    }

    [Fact]
    public void Preflop_AllFoldToOnePlayer_IsBettingRoundComplete()
    {
        var players = new List<Player>
        {
            new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
            new Player { Id = 2, SeatIndex = 1, InitialStack = 100 },
            new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        };
        var hand = new Hand
        {
            Id = 10,
            Players = players,
            Actions = new List<Poker.Core.Models.Action>(),
            Stakes = "",
            CurrentStreet = Street.Preflop
        };
        hand.StakesList = new List<int> { 1, 2 };
        int seq = 1;

        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });
        hand.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });

        var test = _logic.GetNextToAct(hand);
        Assert.Null(test);
        bool bettingComplete = _logic.IsBettingRoundComplete(hand, Street.Preflop);
        Assert.True(bettingComplete);
    }
}
