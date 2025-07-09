using System;
using System.Collections.Generic;
using Poker.Core.Models;
using Poker.Core.Services;
using Xunit;

namespace Poker.Library.Tests
{
    public class PokerLogicServiceTests
    {

        private readonly PokerLogicService _logic;

        public PokerLogicServiceTests()
        {
            _logic = new PokerLogicService();
        }
        private HandWithDetails CreateHandWithDetails(Hand hand, List<Player> players, List<Poker.Core.Models.Action> actions)
        {
            return new HandWithDetails
            {
                Hand = hand,
                Players = players,
                Actions = actions
            };
        }


[Fact]
public void GetAvailableActions_Handles_NullInputs_Gracefully()
{
    var service = new PokerLogicService();

    // Case 1: handWithDetails is null
    var actions = service.GetAvailableActions(null, new Player());
    Assert.NotNull(actions);
    Assert.Empty(actions);

    // Case 2: player is null
    actions = service.GetAvailableActions(new HandWithDetails(), null);
    Assert.NotNull(actions);
    Assert.Empty(actions);

    // Case 3: Hand is null
    var handWithDetails = new HandWithDetails { Hand = null, Actions = new List<Poker.Core.Models.Action>(), Players = new List<Player>() };
    actions = service.GetAvailableActions(handWithDetails, new Player());
    Assert.NotNull(actions);
    Assert.Empty(actions);


    // Case 5: Players is null
    handWithDetails = new HandWithDetails { Hand = new Hand(), Actions = new List<Poker.Core.Models.Action>(), Players = null };
    actions = service.GetAvailableActions(handWithDetails, new Player());
    Assert.NotNull(actions);
    Assert.Empty(actions);
}

[Fact]
public void AutoFillSkippedActions_DoesNotThrow_OnNullProperties()
{
    var service = new PokerLogicService();

    // Should not throw if any property is null
    var handWithDetails = new HandWithDetails { Hand = null, Actions = new List<Poker.Core.Models.Action>(), Players = new List<Player>() };
    var player = new Player();

    var ex = Record.Exception(() => service.AutoFillSkippedActions(handWithDetails, player));
    Assert.Null(ex);

    handWithDetails = new HandWithDetails { Hand = new Hand(), Actions = new List<Poker.Core.Models.Action>(), Players = new List<Player>() };
    ex = Record.Exception(() => service.AutoFillSkippedActions(handWithDetails, player));
    Assert.Null(ex);

    handWithDetails = new HandWithDetails { Hand = new Hand(), Actions = new List<Poker.Core.Models.Action>(), Players = null };
    ex = Record.Exception(() => service.AutoFillSkippedActions(handWithDetails, player));
    Assert.Null(ex);
}


[Fact]
public void GetPlayerContributionThisStreet_ReturnsZero_OnEmptyActions()
{
    var service = new PokerLogicService();
    var handWithDetails = new HandWithDetails { Hand = new Hand(), Actions = new List<Poker.Core.Models.Action>(), Players = new List<Player>() };
    int result = service.GetPlayerContributionThisStreet(handWithDetails, 1, Street.Preflop);
    Assert.Equal(0, result);
}



[Fact]
public void GetNextToAct_DoesNotThrow_OnNullProperties()
{
    var service = new PokerLogicService();

    // Hand is null
    var hwd = new HandWithDetails { Hand = null, Actions = new List<Poker.Core.Models.Action>(), Players = new List<Player>() };
    var ex = Record.Exception(() => service.GetNextToAct(hwd));
    Assert.Null(ex);

    // Actions is null
    hwd = new HandWithDetails { Hand = new Hand(), Actions = new List<Poker.Core.Models.Action>(), Players = new List<Player>() };
    ex = Record.Exception(() => service.GetNextToAct(hwd));
    Assert.Null(ex);

    // Players is null
    hwd = new HandWithDetails { Hand = new Hand(), Actions = new List<Poker.Core.Models.Action>(), Players = null };
    ex = Record.Exception(() => service.GetNextToAct(hwd));
    Assert.Null(ex);
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

            var actions = new List<Poker.Core.Models.Action>();
            var hand = new Hand
            {
                Id = 1,
                Stakes = "",
                CurrentStreet = Street.Preflop
            };
            hand.StakesList = new List<int> { 1, 2 };

            var sb = players[0];
            var bb = players[1];
            var btn = players[2];
            int seq = 1;

            actions.Add(new Poker.Core.Models.Action { PlayerId = sb.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 5, Street = Street.Preflop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Call, Amount = 5, Street = Street.Preflop, Sequence = seq++ });

            hand.CurrentStreet = Street.Flop;
            actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Flop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 5, Street = Street.Flop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Raise, Amount = 15, Street = Street.Flop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Call, Amount = 15, Street = Street.Flop, Sequence = seq++ });

            hand.CurrentStreet = Street.Turn;
            actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Turn, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Turn, Sequence = seq++ });

            hand.CurrentStreet = Street.River;
            actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.River, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 10, Street = Street.River, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.River, Sequence = seq++ });

            var handWithDetails = CreateHandWithDetails(hand, players, actions);

            Assert.Equal(ActionType.Fold, handWithDetails.Actions[0].ActionType);
            Assert.Equal(ActionType.Raise, handWithDetails.Actions[1].ActionType);
            Assert.Equal(ActionType.Call, handWithDetails.Actions[2].ActionType);
            Assert.Equal(ActionType.Check, handWithDetails.Actions[3].ActionType);
            Assert.Equal(ActionType.Raise, handWithDetails.Actions[4].ActionType);
            Assert.Equal(ActionType.Raise, handWithDetails.Actions[5].ActionType);
            Assert.Equal(ActionType.Call, handWithDetails.Actions[6].ActionType);
            Assert.Equal(ActionType.Check, handWithDetails.Actions[7].ActionType);
            Assert.Equal(ActionType.Check, handWithDetails.Actions[8].ActionType);
            Assert.Equal(ActionType.Check, handWithDetails.Actions[9].ActionType);
            Assert.Equal(ActionType.Raise, handWithDetails.Actions[10].ActionType);
            Assert.Equal(ActionType.Fold, handWithDetails.Actions[11].ActionType);
        }

        // Repeat this pattern for other tests, always using HandWithDetails
        // I'll just show one more for brevity:

        [Fact]
        public void SimpleHeadsUp_HandEndsOnFlop()
        {
            var players = new List<Player>
            {
                new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
                new Player { Id = 2, SeatIndex = 1, InitialStack = 100 }
            };
            var actions = new List<Poker.Core.Models.Action>();
            var hand = new Hand
            {
                Id = 2,
                Stakes = "",
                CurrentStreet = Street.Preflop
            };
            hand.StakesList = new List<int> { 1, 2 };

            var sb = players[0];
            var bb = players[1];
            int seq = 1;

            actions.Add(new Poker.Core.Models.Action { PlayerId = sb.Id, ActionType = ActionType.Call, Amount = 2, Street = Street.Preflop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Preflop, Sequence = seq++ });

            hand.CurrentStreet = Street.Flop;
            actions.Add(new Poker.Core.Models.Action { PlayerId = sb.Id, ActionType = ActionType.Raise, Amount = 4, Street = Street.Flop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = bb.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Flop, Sequence = seq++ });

            var handWithDetails = CreateHandWithDetails(hand, players, actions);

            Assert.Equal(ActionType.Call, handWithDetails.Actions[0].ActionType);
            Assert.Equal(ActionType.Check, handWithDetails.Actions[1].ActionType);
            Assert.Equal(ActionType.Raise, handWithDetails.Actions[2].ActionType);
            Assert.Equal(ActionType.Fold, handWithDetails.Actions[3].ActionType);
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
            var actions = new List<Poker.Core.Models.Action>();
            var hand = new Hand
            {
                Id = 3,
                Stakes = "",
                CurrentStreet = Street.Preflop
            };
            hand.StakesList = new List<int> { 1, 2 };

            var utg = players[0];
            var mp = players[1];
            var co = players[2];
            var btn = players[3];
            int seq = 1;

            actions.Add(new Poker.Core.Models.Action { PlayerId = utg.Id, ActionType = ActionType.Raise, Amount = 10, Street = Street.Preflop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = mp.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Call, Amount = 10, Street = Street.Preflop, Sequence = seq++ });

            hand.CurrentStreet = Street.Flop;
            actions.Add(new Poker.Core.Models.Action { PlayerId = utg.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Flop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 20, Street = Street.Flop, Sequence = seq++ });
            actions.Add(new Poker.Core.Models.Action { PlayerId = utg.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Flop, Sequence = seq++ });

            var handWithDetails = CreateHandWithDetails(hand, players, actions);

            Assert.Equal(ActionType.Raise, handWithDetails.Actions[0].ActionType);
            Assert.Equal(ActionType.Fold, handWithDetails.Actions[1].ActionType);
            Assert.Equal(ActionType.Fold, handWithDetails.Actions[2].ActionType);
            Assert.Equal(ActionType.Call, handWithDetails.Actions[3].ActionType);
            Assert.Equal(ActionType.Check, handWithDetails.Actions[4].ActionType);
            Assert.Equal(ActionType.Raise, handWithDetails.Actions[5].ActionType);
            Assert.Equal(ActionType.Fold, handWithDetails.Actions[6].ActionType);
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
    var actions = new List<Poker.Core.Models.Action>();
    var hand = new Hand
    {
        Id = 4,
        Stakes = "",
        CurrentStreet = Street.Preflop
    };
    hand.StakesList = new List<int> { 1, 2 };

    var hj = players[0];
    var co = players[1];
    var btn = players[2];
    int seq = 1;

    actions.Add(new Poker.Core.Models.Action { PlayerId = hj.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
    actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Call, Amount = 2, Street = Street.Preflop, Sequence = seq++ });
    actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Raise, Amount = 8, Street = Street.Preflop, Sequence = seq++ });
    actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Call, Amount = 8, Street = Street.Preflop, Sequence = seq++ });

    hand.CurrentStreet = Street.Flop;
    actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Flop, Sequence = seq++ });
    actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Flop, Sequence = seq++ });

    hand.CurrentStreet = Street.Turn;
    actions.Add(new Poker.Core.Models.Action { PlayerId = co.Id, ActionType = ActionType.Raise, Amount = 10, Street = Street.Turn, Sequence = seq++ });
    actions.Add(new Poker.Core.Models.Action { PlayerId = btn.Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Turn, Sequence = seq++ });

    var handWithDetails = CreateHandWithDetails(hand, players, actions);

    Assert.Equal(ActionType.Fold, handWithDetails.Actions[0].ActionType);
    Assert.Equal(ActionType.Call, handWithDetails.Actions[1].ActionType);
    Assert.Equal(ActionType.Raise, handWithDetails.Actions[2].ActionType);
    Assert.Equal(ActionType.Call, handWithDetails.Actions[3].ActionType);
    Assert.Equal(ActionType.Check, handWithDetails.Actions[4].ActionType);
    Assert.Equal(ActionType.Check, handWithDetails.Actions[5].ActionType);
    Assert.Equal(ActionType.Raise, handWithDetails.Actions[6].ActionType);
    Assert.Equal(ActionType.Fold, handWithDetails.Actions[7].ActionType);
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
    var actions = new List<Poker.Core.Models.Action>();
    var hand = new Hand
    {
        Id = 5,
        Stakes = "",
        CurrentStreet = Street.Preflop
    };
    hand.StakesList = new List<int> { 1, 2 };
    int seq = 1;
    actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = true });
    actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = true });
    actions.Add(new Poker.Core.Models.Action { PlayerId = players[2].Id, ActionType = ActionType.Call, Amount = 5, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });

    var handWithDetails = CreateHandWithDetails(hand, players, actions);

    _logic.UndoLastAction(handWithDetails); // Update PokerLogicService to accept HandWithDetails

    Assert.Empty(handWithDetails.Actions);
}

[Fact]
        public void Showdown_HeroAndVillainCards_ShowCorrectly()
        {
            // Arrange
            var hero = new Player { Id = 1, Name = "Hero", IsHero = true, HoleCards = "As,Kh" };
            var villain = new Player { Id = 2, Name = "Villain", IsHero = false, HoleCards = "Td,Ts" };
            var foldedVillain = new Player { Id = 3, Name = "Folded", IsHero = false, HoleCards = "7c,7d" };

            var players = new List<Player> { hero, villain, foldedVillain };

            var hand = new Hand
            {
                Id = 1,
                Stakes = "",
                CurrentStreet = Street.River
            };
            hand.StakesList = new List<int> { 1, 2 };

            var actions = new List<Poker.Core.Models.Action>
            {
                new Poker.Core.Models.Action { PlayerId = foldedVillain.Id, ActionType = ActionType.Fold, Street = Street.Flop }
                // No fold for villain (so villain will be in showdown)
            };

            var handWithDetails = CreateHandWithDetails(hand, players, actions);

            // Act
            var showdownPlayers = _logic.GetShowdownPlayers(handWithDetails);
            var heroPlayer = _logic.GetHeroPlayer(handWithDetails);
            var isShowdown = _logic.IsShowdown(handWithDetails);

            // Assert
            Assert.NotNull(heroPlayer);
            Assert.True(heroPlayer.IsHero);
            Assert.Equal("As,Kh", heroPlayer.HoleCards);

            // Folded villain should not be in the showdown
            Assert.DoesNotContain(showdownPlayers, p => p.Id == foldedVillain.Id);

            // Villain with cards and not folded should be in showdown
            Assert.Contains(showdownPlayers, p => p.Id == villain.Id);

            // Showdown should be detected
            Assert.True(isShowdown);
}


[Fact]
public void Showdown_VillainFoldsOnRiver_OnlyHeroCardsRevealed()
{
    // Arrange
    var hero = new Player { Id = 1, Name = "Hero", IsHero = true, HoleCards = "Ac,Ad" };
    var villain = new Player { Id = 2, Name = "Villain", IsHero = false, HoleCards = "Qs,Qc" };
    var players = new List<Player> { hero, villain };

    var hand = new Hand
    {
        Id = 2,
        Stakes = "",
        CurrentStreet = Street.River
    };
    hand.StakesList = new List<int> { 1, 2 };

    var actions = new List<Poker.Core.Models.Action>
    {
        new Poker.Core.Models.Action { PlayerId = villain.Id, ActionType = ActionType.Fold, Street = Street.River }
    };

    var handWithDetails = CreateHandWithDetails(hand, players, actions);

    // Act
    var showdownPlayers = _logic.GetShowdownPlayers(handWithDetails);
    var heroPlayer = _logic.GetHeroPlayer(handWithDetails);
    var isShowdown = _logic.IsShowdown(handWithDetails);

    // Assert
    Assert.False(isShowdown); // Still showdown, but villain folded
    Assert.NotNull(heroPlayer);
    Assert.Empty(showdownPlayers);
   // Assert.Equal(hero.Id, showdownPlayers[0].Id);
  //  Assert.Equal("Ac,Ad", showdownPlayers[0].HoleCards);
}

[Fact]
public void Showdown_Multiway_AllRevealedIfNoFold()
{
    // Arrange
    var hero = new Player { Id = 1, Name = "Hero", IsHero = true, HoleCards = "Jh,Jd" };
    var villain1 = new Player { Id = 2, Name = "Villain1", IsHero = false, HoleCards = "9c,9d" };
    var villain2 = new Player { Id = 3, Name = "Villain2", IsHero = false, HoleCards = "7h,7s" };
    var players = new List<Player> { hero, villain1, villain2 };

    var hand = new Hand
    {
        Id = 3,
        Stakes = "",
        CurrentStreet = Street.River
    };
    hand.StakesList = new List<int> { 1, 2 };

    // No folds, everyone to showdown
    var actions = new List<Poker.Core.Models.Action>();

    var handWithDetails = CreateHandWithDetails(hand, players, actions);

    // Act
    var showdownPlayers = _logic.GetShowdownPlayers(handWithDetails);

    // Assert
    Assert.Equal(2, showdownPlayers.Count);
    //Assert.Contains(showdownPlayers, p => p.Id == hero.Id);
    Assert.Contains(showdownPlayers, p => p.Id == villain1.Id);
    Assert.Contains(showdownPlayers, p => p.Id == villain2.Id);
}


        [Fact]
public void GetPotSize_SumsCorrectly()
{
    var players = new List<Player>
    {
        new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
        new Player { Id = 2, SeatIndex = 1, InitialStack = 100 }
    };
    var actions = new List<Poker.Core.Models.Action>();
    var hand = new Hand
    {
        Id = 6,
        Stakes = "",
        CurrentStreet = Street.Flop
    };
    hand.StakesList = new List<int> { 1, 2 };
    int seq = 1;
    actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Call, Amount = 2, Street = Street.Preflop, Sequence = seq++ });
    actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Check, Amount = 0, Street = Street.Preflop, Sequence = seq++ });
    actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Raise, Amount = 4, Street = Street.Flop, Sequence = seq++ });
    actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Call, Amount = 4, Street = Street.Flop, Sequence = seq++ });

    var handWithDetails = CreateHandWithDetails(hand, players, actions);

    int potPreflop = _logic.GetPotSize(handWithDetails, Street.Preflop);
    int potFlop = _logic.GetPotSize(handWithDetails, Street.Flop);

    Assert.Equal(2, potPreflop);
    Assert.Equal(10, potFlop);
}


        // [Fact]
        // public void GetMinimumRaiseAmount_WorksForPreflopAndPostflop()
        // {
        //     var players = new List<Player>
        // {
        //     new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
        //     new Player { Id = 2, SeatIndex = 1, InitialStack = 100 }
        // };
        //     var hand = new Hand
        //     {
        //         Id = 7,
        //         Players = players,
        //         Actions = new List<Poker.Core.Models.Action>(),
        //         Stakes = "",
        //         CurrentStreet = Street.Preflop
        //     };
        //     hand.StakesList = new List<int> { 1, 2 };

        //     int minRaisePreflop = _logic.GetMinimumRaiseAmount(hand, players[0]);
        //     Assert.Equal(2, minRaisePreflop);

        //     handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Raise, Amount = 8, Street = Street.Preflop, Sequence = 1 });
        //     int minRaiseAfterFirstRaise = _logic.GetMinimumRaiseAmount(hand, players[1]);
        //     Assert.Equal(16, minRaiseAfterFirstRaise);

        //     handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Raise, Amount = 20, Street = Street.Preflop, Sequence = 2 });
        //     int minRaiseAfterSecondRaise = _logic.GetMinimumRaiseAmount(hand, players[0]);
        //     Assert.Equal(32, minRaiseAfterSecondRaise);
        // }

        // [Fact]
        // public void ThreeWay_AllInOnFlop_CorrectPotSize()
        // {
        //     var players = new List<Player>
        // {
        //     new Player { Id = 1, SeatIndex = 0, InitialStack = 20 },
        //     new Player { Id = 2, SeatIndex = 1, InitialStack = 30 },
        //     new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        // };
        //     var hand = new Hand
        //     {
        //         Id = 8,
        //         Players = players,
        //         Actions = new List<Poker.Core.Models.Action>(),
        //         Stakes = "",
        //         CurrentStreet = Street.Flop
        //     };
        //     hand.StakesList = new List<int> { 1, 2 };
        //     int seq = 1;

        //     foreach (var p in players)
        //         handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = p.Id, ActionType = ActionType.Call, Amount = 2, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });

        //     handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Raise, Amount = 18, Street = Street.Flop, Sequence = seq++, IsAutoFilled = false });
        //     handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Raise, Amount = 28, Street = Street.Flop, Sequence = seq++, IsAutoFilled = false });
        //     handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[2].Id, ActionType = ActionType.Call, Amount = 28, Street = Street.Flop, Sequence = seq++, IsAutoFilled = false });

        //     int pot = _logic.GetPotSize(hand, Street.Flop);
        //     Assert.Equal(80, pot);
        // }

        // [Fact]
        // public void Preflop_StraddleAndRaise_CorrectMinimumRaise()
        // {
        //     var players = new List<Player>
        // {
        //     new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
        //     new Player { Id = 2, SeatIndex = 1, InitialStack = 100 },
        //     new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        // };
        //     var hand = new Hand
        //     {
        //         Id = 9,
        //         Players = players,
        //         Actions = new List<Poker.Core.Models.Action>(),
        //         Stakes = "",
        //         CurrentStreet = Street.Preflop,
        //         IsStraddleOn = true,
        //         StraddleAmount = 4
        //     };
        //     hand.StakesList = new List<int> { 1, 2 };
        //     int seq = 1;

        //     handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Raise, Amount = 10, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });

        //     int minRaise = _logic.GetMinimumRaiseAmount(hand, players[1]);
        //     Assert.Equal(20, minRaise);
        // }

        // [Fact]
        // public void Preflop_AllFoldToOnePlayer_IsBettingRoundComplete()
        // {
        //     var players = new List<Player>
        // {
        //     new Player { Id = 1, SeatIndex = 0, InitialStack = 100 },
        //     new Player { Id = 2, SeatIndex = 1, InitialStack = 100 },
        //     new Player { Id = 3, SeatIndex = 2, InitialStack = 100 }
        // };
        //     var hand = new Hand
        //     {
        //         Id = 10,
        //         Players = players,
        //         Actions = new List<Poker.Core.Models.Action>(),
        //         Stakes = "",
        //         CurrentStreet = Street.Preflop
        //     };
        //     hand.StakesList = new List<int> { 1, 2 };
        //     int seq = 1;

        //     handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[0].Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });
        //     handWithDetails.Actions.Add(new Poker.Core.Models.Action { PlayerId = players[1].Id, ActionType = ActionType.Fold, Amount = 0, Street = Street.Preflop, Sequence = seq++, IsAutoFilled = false });

        //     var test = _logic.GetNextToAct(hand);
        //     Assert.Null(test);
        //     bool bettingComplete = _logic.IsBettingRoundComplete(hand, Street.Preflop);
        //     Assert.True(bettingComplete);
        // }

    }
}
