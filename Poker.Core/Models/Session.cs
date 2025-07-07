using System;
using SQLite;

namespace Poker.Core.Models
{
    [Table("Session")]
    public class Session
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Guid SyncId { get; set; }

        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Stakes { get; set; } = string.Empty; // e.g., "1,2"
        public string GameType { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        public Session()
        {
            Date = DateTime.Now;
            SyncId  = Guid.NewGuid();
        }
    }
}
