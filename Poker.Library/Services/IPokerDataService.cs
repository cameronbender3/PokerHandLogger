using System.Collections.Generic;
using System.Threading.Tasks;
using Poker.Core.Models;

namespace Poker.Library.Services
{
    public interface IPokerDataService
    {
        // HANDS
        Task<List<Hand>> GetAllHandsAsync();
        Task<Hand?> GetHandByIdAsync(int handId);
        Task SaveHandAsync(Hand hand);
        Task DeleteHandAsync(int handId);

        // SESSIONS
        Task<List<Session>> GetAllSessionsAsync();
        Task<Session?> GetSessionByIdAsync(int sessionId);
        Task SaveSessionAsync(Session session);
        Task DeleteSessionAsync(int sessionId);

    }
}
