using System;
using System.Collections.Generic;
namespace Poker.Core.Models;

    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int InitialStack { get; set; }
        public bool IsHero { get; set; }
        public int SeatIndex { get; set; }
        public List<Card> HoleCards { get; set; }
        public PlayerType Type { get; set; }


        public enum PlayerType
    {
        Unknown,
        Fish,
        Whale,
        Shark,
        Nit,
        Reg
    }

    public Player(int holeCardCount = 2)
    {
        Id = Guid.NewGuid();
        Name = string.Empty;
        InitialStack = 0;
        IsHero = false;
        SeatIndex = 0;
        HoleCards = new List<Card>(holeCardCount); // Will be filled as cards are entered
        Type = PlayerType.Unknown;
    }
}