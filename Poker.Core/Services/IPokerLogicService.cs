using System;
using System.Collections.Generic;
namespace Poker.Core.Services;

using Poker.Core.Models;

public interface IPokerLogicService
{
        public List<ActionType> GetAvailableActions(HandWithDetails handWithDetails, Player player);
        Player? GetNextToAct(HandWithDetails handWithDetails);
        void AutoFillSkippedActions(HandWithDetails handWithDetails, Player justActedPlayer);
        bool IsBettingRoundComplete(HandWithDetails handWithDetails, Street street);
        Street GetNextStreet(Street street);
        void UndoLastAction(HandWithDetails handWithDetails);
        int GetPotSize(HandWithDetails handWithDetails, Street upToStreet);
        int GetMinimumRaiseAmount(HandWithDetails handWithDetails, Player player);
}
