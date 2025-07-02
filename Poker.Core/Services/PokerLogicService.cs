using System;
using System.Collections.Generic;
using System.Linq;
using Poker.Core.Models;
namespace Poker.Core.Services;

public class PokerLogicService : IPokerLogicService
{
    public List<ActionType> GetAvailableActions(Hand hand, Player player)
    {
        var actions = new List<ActionType>();

        // If player has folded or is all-in, return no actions
        if (PlayerHasFolded(hand, player) || PlayerIsAllIn(hand, player))
            return actions;

        var currentStreet = hand.CurrentStreet;
        var streetActions = hand.Actions.Where(a => a.Street == currentStreet).ToList();

        var currentBet = GetCurrentBetThisStreet(streetActions, hand, currentStreet);
        var playerContribution = GetPlayerContributionThisStreet(streetActions, player.Id);
        var toCall = currentBet - playerContribution;
        var stack = player.InitialStack - GetTotalContribution(hand, player.Id);

        // If no bet in front (toCall == 0), player can Check or Raise (first bet of street)
        if (toCall == 0)
        {
            actions.Add(ActionType.Check);
            if (stack > 0)
                actions.Add(ActionType.Raise);
        }
        else
        {
            actions.Add(ActionType.Fold);

            // If player has enough to call
            if (stack >= toCall)
                actions.Add(ActionType.Call);
            else if (stack > 0)
                actions.Add(ActionType.Call); // This is technically a call-all-in

            // If player has enough to raise
            if (stack > toCall)
                actions.Add(ActionType.Raise);
        }

        return actions;
    }

    // Helper: Returns true if the player has folded already
    private bool PlayerHasFolded(Hand hand, Player player)
    {
        return hand.Actions.Any(a => a.PlayerId == player.Id && a.ActionType == ActionType.Fold);
    }

    // Helper: Returns true if player has no chips left to act
    private bool PlayerIsAllIn(Hand hand, Player player)
    {
        var totalContributed = GetTotalContribution(hand, player.Id);
        return (player.InitialStack - totalContributed) == 0;
    }

    // Helper: Get the current street (can be stored in Hand if you track it explicitly)
    public Street GetNextStreet(Street street)
    {
        switch (street)
        {
            case Street.Preflop: return Street.Flop;
            case Street.Flop: return Street.Turn;
            case Street.Turn: return Street.River;
            default: return Street.River; // Already at river
        }
    }


    // Helper: Get the highest bet placed so far on this street
    private int GetCurrentBetThisStreet(List<Poker.Core.Models.Action> streetActions, Hand hand, Street currentStreet)
    {
        // Find the largest contribution made in a single action this street
        return streetActions.Count > 0 ? streetActions.Max(a => a.Amount) : GetDefaultBet(hand, currentStreet);
    }

    // Helper: Get how much the player has put in on this street
    private int GetPlayerContributionThisStreet(List<Poker.Core.Models.Action> streetActions, Guid playerId)
    {
        return streetActions
            .Where(a => a.PlayerId == playerId && (a.ActionType == ActionType.Call || a.ActionType == ActionType.Raise))
            .Sum(a => a.Amount);
    }

    // Helper: Get how much the player has put in total (across all streets)
    private int GetTotalContribution(Hand hand, Guid playerId)
    {
        return hand.Actions
            .Where(a => a.PlayerId == playerId && (a.ActionType == ActionType.Call || a.ActionType == ActionType.Raise))
            .Sum(a => a.Amount);
    }

    // Helper: For first bet of each street (preflop is usually BB or straddle)
    private int GetDefaultBet(Hand hand, Street street)
    {
        // For Preflop, return Big Blind or Straddle
        if (street == Street.Preflop)
        {
            if (hand.IsStraddleOn == true && hand.StraddleAmount.HasValue)
                return hand.StraddleAmount.Value;
            else
                return hand.Stakes.Count > 1 ? hand.Stakes[1] : 0; // [small, big]
        }
        return 0;
    }

    public void AutoFillSkippedActions(Hand hand, Player justActedPlayer)
    {
        var street = hand.CurrentStreet;

            // Get the ordered list of players still in the hand (not folded/all-in)
            var eligiblePlayers = hand.Players
                .Where(p => !PlayerHasFolded(hand, p) && !PlayerIsAllIn(hand, p))
                .OrderBy(p => GetBettingOrderIndex(hand, p, street))
                .ToList();

            // Find action order for this street
            var actionOrder = eligiblePlayers;

            // Find indices
            int justActedIndex = actionOrder.FindIndex(p => p.Id == justActedPlayer.Id);

            // Find skipped players before justActedPlayer who haven't acted yet this street
            var skippedPlayers = new List<Player>();
            for (int i = 0; i < justActedIndex; i++)
            {
                var player = actionOrder[i];
                if (!HasActedThisStreet(hand, player))
                {
                    skippedPlayers.Add(player);
                }
            }

            // Determine if there has been a bet on this street already
            bool betOccurred = HasBetOccurredThisStreet(hand);

            // Insert auto-filled actions
            int nextSequence = GetNextSequence(hand);

        foreach (var skipped in skippedPlayers)
        {
            ActionType autoType;
            if (street == Street.Preflop)
            {
                autoType = ActionType.Fold;
            }
            else
            {
                autoType = betOccurred ? ActionType.Fold : ActionType.Check;
            }

            var autoAction = new Poker.Core.Models.Action
            {
                PlayerId = skipped.Id,
                ActionType = autoType,
                Amount = 0,
                Street = street,
                Sequence = nextSequence++,
                IsAutoFilled = true
            };

            // Insert before justActedPlayer's action if already added, else just add at end for now
            hand.Actions.Add(autoAction);
            }

            // NOTE: You'll want to ensure the actual "justActedPlayer" action is inserted after these (in the UI or VM logic)
        }

        // Helper: returns betting order index for a player, taking into account street rules
        private int GetBettingOrderIndex(Hand hand, Player player, Street street)
        {
            // Preflop: order starts after BB or Straddle
            // Postflop: order starts after Button
            // We'll keep it simple for now and just use SeatIndex
            // (Enhance this to handle actual betting order logic)
            return player.SeatIndex;
        }

        // Helper: Checks if player has acted this street
        private bool HasActedThisStreet(Hand hand, Player player)
        {
            return hand.Actions.Any(a => a.PlayerId == player.Id && a.Street == hand.CurrentStreet);
        }

        // Helper: Checks if anyone has bet/raised this street
        private bool HasBetOccurredThisStreet(Hand hand)
        {
            return hand.Actions.Any(a =>
                a.Street == hand.CurrentStreet &&
                (a.ActionType == ActionType.Raise)); // You might add .Bet if you add that as a separate type
        }

        // Helper: Get the next available sequence number
        private int GetNextSequence(Hand hand)
        {
            return hand.Actions.Count > 0
                ? hand.Actions.Max(a => a.Sequence) + 1
                : 1;
        }
    

    
    public Player? GetNextToAct(Hand hand)
    {
        // TODO: Implement logic
        return null;
    }



    public bool IsBettingRoundComplete(Hand hand, Street street)
    {
        // TODO: Implement logic
        return false;
    }



    public void UndoLastAction(Hand hand)
    {
        // TODO: Implement logic
    }

    public int GetPotSize(Hand hand, Street upToStreet)
    {
        // TODO: Implement logic
        return 0;
    }

    public int GetMinimumRaiseAmount(Hand hand, Player player)
    {
        // TODO: Implement logic
        return 0;
    }
}