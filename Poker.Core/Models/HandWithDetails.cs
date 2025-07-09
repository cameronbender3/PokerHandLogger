using System;

namespace Poker.Core.Models;

public class HandWithDetails
{
    public Hand? Hand { get; set; }
    public List<Player>? Players { get; set; } 
    public List<Action> Actions { get; set; } = new List<Action>();
    public List<Card>? Cards { get; set; }
}

