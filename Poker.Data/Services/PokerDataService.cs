using Poker.Core.Models;
using Poker.Library.Services;
using SQLite;
using System.Threading.Tasks;
using System.Linq;

namespace Poker.Data.Services
{
    public class PokerDataService : IPokerDataService
    {
        private readonly SQLiteAsyncConnection _db;

        public PokerDataService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<Session>().Wait();
            _db.CreateTableAsync<Hand>().Wait();
        }

        // HANDS

        public async Task<List<Hand>> GetAllHandsAsync()
        {
            return await _db.Table<Hand>().ToListAsync();
        }

        public async Task<Hand?> GetHandByIdAsync(int handId)
        {
            return await _db.Table<Hand>().Where(h => h.Id == handId).FirstOrDefaultAsync();
        }

        public async Task SaveHandAsync(Hand hand)
        {
            if (hand.Id != 0)
                await _db.UpdateAsync(hand);
            else
                await _db.InsertAsync(hand);
        }

        public async Task DeleteHandAsync(int handId)
        {
            var hand = await GetHandByIdAsync(handId);
            if (hand != null)
                await _db.DeleteAsync(hand);
        }

        // SESSIONS

        public async Task<List<Session>> GetAllSessionsAsync()
        {
            return await _db.Table<Session>().ToListAsync();
        }

        public async Task<Session?> GetSessionByIdAsync(int sessionId)
        {
            return await _db.Table<Session>().Where(s => s.Id == sessionId).FirstOrDefaultAsync();
        }

        public async Task SaveSessionAsync(Session session)
        {
            if (session.Id != 0)
                await _db.UpdateAsync(session);
            else
                await _db.InsertAsync(session);
        }

        public async Task DeleteSessionAsync(int sessionId)
        {
            var session = await GetSessionByIdAsync(sessionId);
            if (session != null)
                await _db.DeleteAsync(session);
        }
    }
}
