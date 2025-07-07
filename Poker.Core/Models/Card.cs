using System;
using SQLite;

namespace Poker.Core.Models
{
    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    [Table("Card")]
    public class Card
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int HandId { get; set; }           // FK to Hand (required)
        public int? PlayerId { get; set; }        // FK to Player, null if board card
        public bool IsBoardCard { get; set; }     // True if board card, false if hole card

        public Rank Rank { get; set; }
        public Suit Suit { get; set; }

        public Card() { }

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public override string ToString()
        {
            // Returns something like "A♠" or "10♦"
            string rankStr = Rank switch
            {
                Rank.Ace => "A",
                Rank.King => "K",
                Rank.Queen => "Q",
                Rank.Jack => "J",
                Rank.Ten => "10",
                _ => ((int)Rank).ToString()
            };
            string suitStr = Suit switch
            {
                Suit.Clubs => "♣",
                Suit.Diamonds => "♦",
                Suit.Hearts => "♥",
                Suit.Spades => "♠",
                _ => ""
            };
            return $"{rankStr}{suitStr}";
        }
    }
}
