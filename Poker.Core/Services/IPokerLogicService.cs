using System;
using System.Collections.Generic;
namespace Poker.Core.Services;

using Poker.Core.Models;

public interface IPokerLogicService
{
        List<ActionType> GetAvailableActions(Hand hand, Player player);
        Player? GetNextToAct(Hand hand);
        void AutoFillSkippedActions(Hand hand, Player justActedPlayer);
        bool IsBettingRoundComplete(Hand hand, Street street);
        Street GetNextStreet(Street street);
        void UndoLastAction(Hand hand);
        int GetPotSize(Hand hand, Street upToStreet);
        int GetMinimumRaiseAmount(Hand hand, Player player);
}
