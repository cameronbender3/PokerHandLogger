using System;
using System.Collections.Generic;

namespace Poker.Core.Models;

// Represents a poker session. Each session can have default stakes and game type, which individual hands may override.
public class Session
{
    /// Unique identifier for the session.
    public Guid Id { get; set; }

    /// Date and time the session was played.
    public DateTime Date { get; set; }

    /// Location or venue where the session took place.
    public string Location { get; set; }

    /// Default stakes for the session (e.g., "1/2"). Individual hands can override.
    public List<int>? Stakes { get; set; }

    /// Default game type for the session (e.g., "NLHE"). Individual hands can override.
    public string? GameType { get; set; }

    /// Any notes or remarks about the session.
    public string? Note { get; set; }

    /// List of hands played in this session (optional, may be null or empty).
    public List<Hand>? Hands { get; set; }

    /// Default constructor initializes with new Guid and current date.
    public Session()
    {
        Id = Guid.NewGuid();
        Date = DateTime.Now;
        Location = string.Empty;
        Hands = new List<Hand>();
    }
}
