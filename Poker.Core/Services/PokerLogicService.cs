using System;
using System.Collections.Generic;
using System.Linq;
using Poker.Core.Models;
namespace Poker.Core.Services;

public class PokerLogicService : IPokerLogicService
{
    public List<ActionType> GetAvailableActions(HandWithDetails handWithDetails, Player player)
    {
        var actions = new List<ActionType>();

        // If player has folded or is all-in, return no actions
        if (PlayerHasFolded(handWithDetails, player) || PlayerIsAllIn(handWithDetails, player))
            return actions;

        var currentStreet = handWithDetails.Hand.CurrentStreet;
        var streetActions = handWithDetails.Actions.Where(a => a.Street == currentStreet).ToList();

        var currentBet = GetCurrentBetThisStreet(streetActions, handWithDetails);
        var playerContribution = GetPlayerContributionThisStreet(handWithDetails, player.Id, currentStreet);

        // Find the full player object (in case you need up-to-date stack etc.)
        var fullPlayer = handWithDetails.Players.FirstOrDefault(p => p.Id == player.Id);
        int stack = (fullPlayer?.InitialStack ?? 0) - GetTotalContribution(handWithDetails, player.Id);

        var toCall = currentBet - playerContribution;

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
    public bool PlayerHasFolded(HandWithDetails handWithDetails, Player player)
    {
        if (handWithDetails?.Actions == null || player == null)
            return false;

        return handWithDetails.Actions.Any(a =>
            a.PlayerId == player.Id &&
            a.ActionType == ActionType.Fold
        );
    }



    // Helper: Returns true if player has no chips left to act
    private bool PlayerIsAllIn(HandWithDetails handWithDetails, Player player)
    {
        var totalContributed = GetTotalContribution(handWithDetails, player.Id);
        // Get the "real" player from the list, to ensure up-to-date stack
        var fullPlayer = handWithDetails.Players.FirstOrDefault(p => p.Id == player.Id);
        return ((fullPlayer?.InitialStack ?? 0) - totalContributed) == 0;
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
    private int GetCurrentBetThisStreet(List<Poker.Core.Models.Action> streetActions, HandWithDetails handWithDetails)
    {
        var currentStreet = handWithDetails.Hand.CurrentStreet;
        return streetActions.Count > 0
            ? streetActions.Max(a => a.Amount)
            : GetDefaultBet(handWithDetails.Hand, currentStreet);
    }


    // Helper: Get how much the player has put in on this street
    private int GetPlayerContributionThisStreet(HandWithDetails handWithDetails, int playerId, Street street)
    {
        return handWithDetails.Actions
            .Where(a => a.Street == street && a.PlayerId == playerId && (a.ActionType == ActionType.Call || a.ActionType == ActionType.Raise))
            .Sum(a => a.Amount);
    }


    // Helper: Get how much the player has put in total (across all streets)
    private int GetTotalContribution(HandWithDetails handWithDetails, int playerId)
    {
        return handWithDetails.Actions
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
                return hand.StakesList.Count > 1 ? hand.StakesList[1] : 0; // [small, big]
        }
        return 0;
    }

    public void AutoFillSkippedActions(HandWithDetails handWithDetails, Player justActedPlayer)
    {
        var street = handWithDetails.Hand.CurrentStreet;

        // Only consider players still in the hand (not folded/all-in)
        var eligiblePlayers = handWithDetails.Players
            .Where(p => !PlayerHasFolded(handWithDetails, p) && !PlayerIsAllIn(handWithDetails, p))
            .OrderBy(p => GetBettingOrderIndex(handWithDetails.Hand, p, street))
            .ToList();

        // Betting order on this street
        var actionOrder = eligiblePlayers;

        int justActedIndex = actionOrder.FindIndex(p => p.Id == justActedPlayer.Id);

        // Find skipped players (before justActedPlayer) who haven't acted this street
        var skippedPlayers = new List<Player>();
        for (int i = 0; i < justActedIndex; i++)
        {
            var player = actionOrder[i];
            if (!HasActedThisStreet(handWithDetails, player))
            {
                skippedPlayers.Add(player);
            }
        }

        bool betOccurred = HasBetOccurredThisStreet(handWithDetails);

        int nextSequence = GetNextSequence(handWithDetails);

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
                HandId = handWithDetails.Hand.Id,
                ActionType = autoType,
                Amount = 0,
                Street = street,
                Sequence = nextSequence++,
                IsAutoFilled = true
            };

            handWithDetails.Actions.Add(autoAction);
        }

        // The actual user action should be added after these in your UI/VM logic
    }


    // Helper: returns betting order index for a player, taking into account street rules
    private int GetBettingOrderIndex(Hand hand, Player player, Street street)
    {
        // For now, just SeatIndex (can later improve for more poker-specific logic)
        return player.SeatIndex;
    }


    // Helper: Checks if player has acted this street
    private bool HasActedThisStreet(HandWithDetails handWithDetails, Player player)
    {
        var street = handWithDetails.Hand.CurrentStreet;
        return handWithDetails.Actions.Any(a => a.PlayerId == player.Id && a.Street == street);
    }



    // Helper: Checks if anyone has bet/raised this street
    private bool HasBetOccurredThisStreet(HandWithDetails handWithDetails)
    {
        var street = handWithDetails.Hand.CurrentStreet;
        return handWithDetails.Actions.Any(a =>
            a.Street == street &&
            (a.ActionType == ActionType.Raise));
    }

    // Helper: Get the next available sequence number
    private int GetNextSequence(HandWithDetails handWithDetails)
    {
        return handWithDetails.Actions.Count > 0
            ? handWithDetails.Actions.Max(a => a.Sequence) + 1
            : 1;
    }




    public Player? GetNextToAct(HandWithDetails handWithDetails)
    {
        var hand = handWithDetails.Hand;
        var players = handWithDetails.Players;
        var actions = handWithDetails.Actions;

        if (hand == null || players == null || players.Count < 2)
            return null;

        var currentStreet = hand.CurrentStreet;
        var actionsThisStreet = actions
            .Where(a => a.Street == currentStreet)
            .OrderBy(a => a.Sequence)
            .ToList();

        // Only consider players who have not folded and are not all-in
        var eligiblePlayers = players
            .Where(p => !PlayerHasFolded(handWithDetails, p) && !PlayerIsAllIn(handWithDetails, p))
            .OrderBy(p => p.SeatIndex)
            .ToList();

        if (eligiblePlayers.Count == 1)
            return null;

        // Find the last player who acted this street
        Player? lastActor = null;
        if (actionsThisStreet.Count > 0)
        {
            var lastAction = actionsThisStreet.Last();
            lastActor = eligiblePlayers.FirstOrDefault(p => p.Id == lastAction.PlayerId);
        }

        int startIdx = 0;
        if (lastActor != null)
        {
            int idx = eligiblePlayers.IndexOf(lastActor);
            startIdx = (idx + 1) % eligiblePlayers.Count;
        }

        // Loop through eligible players in table order, starting after last actor
        for (int i = 0; i < eligiblePlayers.Count; i++)
        {
            int idx = (startIdx + i) % eligiblePlayers.Count;
            var player = eligiblePlayers[idx];

            bool hasActed = actions.Any(a => a.Street == currentStreet && a.PlayerId == player.Id);
            if (!hasActed)
                return player;
        }

        return null; // All eligible players have acted
    }




    public bool IsBettingRoundComplete(HandWithDetails handWithDetails, Street street)
    {
        // (Optional: if you want to support passing a specific street, you may)
        // Or just always check for CurrentStreet.
        return GetNextToAct(handWithDetails) == null;
    }



    public void UndoLastAction(HandWithDetails handWithDetails)
    {
        var actions = handWithDetails.Actions;
        if (actions == null || actions.Count == 0)
            return;

        // Remove the last action (should always be user)
        var lastAction = actions.OrderByDescending(a => a.Sequence).First();
        actions.Remove(lastAction);

        // Remove preceding auto-filled actions, until next user action or empty
        while (actions.Count > 0)
        {
            var prevAction = actions.OrderByDescending(a => a.Sequence).First();
            if (prevAction.IsAutoFilled)
                actions.Remove(prevAction);
            else
                break;
        }
    }


    public int GetPotSize(HandWithDetails handWithDetails, Street upToStreet)
    {
        int pot = 0;
        var actions = handWithDetails.Actions;
        if (actions == null) return 0;

        foreach (var action in actions)
        {
            if ((action.Street <= upToStreet) &&
                (action.ActionType == ActionType.Raise || action.ActionType == ActionType.Call))
            {
                pot += action.Amount;
            }
        }
        return pot;
    }


    public int GetMinimumRaiseAmount(HandWithDetails handWithDetails, Player player)
    {
        var hand = handWithDetails.Hand;
        var street = hand.CurrentStreet;
        var actionsThisStreet = handWithDetails.Actions
            .Where(a => a.Street == street)
            .OrderBy(a => a.Sequence)
            .ToList();

        // Find all unique raises (and optionally bets)
        var raises = actionsThisStreet
            .Where(a => a.ActionType == ActionType.Raise)
            .ToList();

        int minRaise = 0;

        if (raises.Count == 0)
        {
            // No raise yet. Use BB for preflop, otherwise 0 or a default for postflop
            minRaise = (street == Street.Preflop && hand.StakesList.Count > 1) ? hand.StakesList[1] : 0;
        }
        else if (raises.Count == 1)
        {
            // First raise: min raise = 2x BB or 2x first bet
            minRaise = raises[0].Amount * 2;
        }
        else
        {
            // Find the last two raises to get difference
            var lastRaise = raises.Last();
            var prevRaise = raises[raises.Count - 2];
            var raiseDiff = lastRaise.Amount - prevRaise.Amount;
            minRaise = lastRaise.Amount + raiseDiff;
        }

        return minRaise;
    }

    public Player? GetHeroPlayer(HandWithDetails handWithDetails)
    {
        if (handWithDetails?.Players == null)
            return null;

        return handWithDetails.Players.FirstOrDefault(p => p.IsHero);
    }

    public List<Player> GetShowdownPlayers(HandWithDetails handWithDetails)
    {
        if (handWithDetails?.Players == null)
            return new List<Player>();

        // Find all non-hero players still in at showdown and with known hole cards.
        return handWithDetails.Players
            .Where(p =>
                !p.IsHero &&
                !PlayerHasFolded(handWithDetails, p) &&
                !string.IsNullOrWhiteSpace(p.HoleCards)
            )
            .ToList();
    }

public bool IsShowdown(HandWithDetails handWithDetails)
{
    if (handWithDetails?.Hand == null || handWithDetails.Players == null)
        return false;

    // Showdown occurs if there are 2 or more players who have NOT folded at the end (river)
    var playersInHand = handWithDetails.Players
        .Where(p => !PlayerHasFolded(handWithDetails, p))
        .ToList();

    return handWithDetails.Hand.CurrentStreet == Street.River && playersInHand.Count >= 2;
}


}