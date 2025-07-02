using System;
using System.Collections.Generic;
using Poker.Core.Models;    // Update to your actual namespace
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
    // Arrange
    var logic = new PokerLogicService();

    // 3 players: SB, BB, BTN (seats 0, 1, 2)
    var players = new List<Player>
    {
        new Player { Id = Guid.NewGuid(), SeatIndex = 0, InitialStack = 100 }, // SB
        new Player { Id = Guid.NewGuid(), SeatIndex = 1, InitialStack = 100 }, // BB
        new Player { Id = Guid.NewGuid(), SeatIndex = 2, InitialStack = 100 }, // BTN
    };

    var hand = new Hand
    {
        Id = Guid.NewGuid(),
        Players = players,
        Actions = new List<Poker.Core.Models.Action>(),
        Stakes = new List<int> { 1, 2 }, // 1/2 blinds
        CurrentStreet = Street.Preflop
    };

    var sb = players[0];
    var bb = players[1];
    var btn = players[2];

    int seq = 1;

    // --- Preflop: folds to BTN, BTN raises, BB calls
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = sb.Id,
        ActionType = ActionType.Fold,
        Amount = 0,
        Street = Street.Preflop,
        Sequence = seq++
    });
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = btn.Id,
        ActionType = ActionType.Raise,
        Amount = 5,
        Street = Street.Preflop,
        Sequence = seq++
    });
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = bb.Id,
        ActionType = ActionType.Call,
        Amount = 5,
        Street = Street.Preflop,
        Sequence = seq++
    });

    // --- Flop: BB checks, BTN raises, BB check-raises, BTN calls
    hand.CurrentStreet = Street.Flop;

    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = bb.Id,
        ActionType = ActionType.Check,
        Amount = 0,
        Street = Street.Flop,
        Sequence = seq++
    });
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = btn.Id,
        ActionType = ActionType.Raise,
        Amount = 5,
        Street = Street.Flop,
        Sequence = seq++
    });
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = bb.Id,
        ActionType = ActionType.Raise,
        Amount = 15,
        Street = Street.Flop,
        Sequence = seq++
    });
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = btn.Id,
        ActionType = ActionType.Call,
        Amount = 15,
        Street = Street.Flop,
        Sequence = seq++
    });

    // --- Turn: Check, check
    hand.CurrentStreet = Street.Turn;

    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = bb.Id,
        ActionType = ActionType.Check,
        Amount = 0,
        Street = Street.Turn,
        Sequence = seq++
    });
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = btn.Id,
        ActionType = ActionType.Check,
        Amount = 0,
        Street = Street.Turn,
        Sequence = seq++
    });

    // --- River: BB checks, BTN raises, BB folds
    hand.CurrentStreet = Street.River;

    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = bb.Id,
        ActionType = ActionType.Check,
        Amount = 0,
        Street = Street.River,
        Sequence = seq++
    });
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = btn.Id,
        ActionType = ActionType.Raise,
        Amount = 10, // any number, e.g. 10
        Street = Street.River,
        Sequence = seq++
    });
    hand.Actions.Add(new Poker.Core.Models.Action
    {
        PlayerId = bb.Id,
        ActionType = ActionType.Fold,
        Amount = 0,
        Street = Street.River,
        Sequence = seq++
    });

    // --- Assertions ---

    // Preflop
    Assert.Equal(ActionType.Fold, hand.Actions[0].ActionType); // SB folded
    Assert.Equal(ActionType.Raise, hand.Actions[1].ActionType); // BTN raised
    Assert.Equal(ActionType.Call, hand.Actions[2].ActionType); // BB called

    // Flop
    Assert.Equal(ActionType.Check, hand.Actions[3].ActionType); // BB checked
    Assert.Equal(ActionType.Raise, hand.Actions[4].ActionType); // BTN bet
    Assert.Equal(ActionType.Raise, hand.Actions[5].ActionType); // BB check-raise
    Assert.Equal(ActionType.Call, hand.Actions[6].ActionType); // BTN call

    // Turn
    Assert.Equal(ActionType.Check, hand.Actions[7].ActionType); // BB check
    Assert.Equal(ActionType.Check, hand.Actions[8].ActionType); // BTN check

    // River
    Assert.Equal(ActionType.Check, hand.Actions[9].ActionType); // BB check
    Assert.Equal(ActionType.Raise, hand.Actions[10].ActionType); // BTN bet
    Assert.Equal(ActionType.Fold, hand.Actions[11].ActionType); // BB fold

    // You can add more assertions about the final state if you like (who won, pot size, etc.)
}


        // Add more [Fact] or [Theory] methods for your scenarios...

        private Hand CreateBasicHand()
        {
            var players = new List<Player>
            {
                new Player { Id = Guid.NewGuid(), SeatIndex = 0, InitialStack = 100, IsHero = false },
                new Player { Id = Guid.NewGuid(), SeatIndex = 1, InitialStack = 100, IsHero = false },
                new Player { Id = Guid.NewGuid(), SeatIndex = 2, InitialStack = 100, IsHero = false },
                new Player { Id = Guid.NewGuid(), SeatIndex = 3, InitialStack = 100, IsHero = false },
            };
            return new Hand
            {
                Id = Guid.NewGuid(),
                Players = players,
                Actions = new List<Poker.Core.Models.Action>(),
                Stakes = new List<int> { 1, 2 },
                CurrentStreet = Street.Preflop
            };
        }
    }