using System;
using System;
using System.IO;
using System.Threading.Tasks;
using Poker.Core.Models;
using Poker.Library.Services;
using Poker.Data.Services; // The implementation lives in Poker.Maui
using Xunit;
namespace Poker.Library.Tests;

public class PokerDataServiceTests
{
    private static string TestDbPath => Path.Combine(Path.GetTempPath(), $"test_poker_{Guid.NewGuid()}.db3");

    [Fact]
    public async Task CanInsertAndRetrieveSession()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var session = new Session
        {
            Date = DateTime.Now,
            Location = "Test Casino",
            Stakes = "1,2",
            GameType = "NLHE",
            Note = "Unit test session"
        };

        await dataService.SaveSessionAsync(session);

        var sessions = await dataService.GetAllSessionsAsync();
        Assert.Single(sessions);
        Assert.Equal("Test Casino", sessions[0].Location);

        // Clean up
        File.Delete(dbPath);
    }

    [Fact]
    public async Task CanInsertAndRetrieveHand()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var hand = new Hand
        {
            Timestamp = DateTime.Now,
            Stakes = "1,2",
            GameType = "NLHE",
            ProfitLoss = 10,
            CurrentStreet = Street.Flop
        };

        await dataService.SaveHandAsync(hand);

        var hands = await dataService.GetAllHandsAsync();
        Assert.Single(hands);
        Assert.Equal(10, hands[0].ProfitLoss);

        // Clean up
        File.Delete(dbPath);
    }

    [Fact]
    public async Task CanUpdateSession()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var session = new Session { Location = "Original", Stakes = "1,2" };
        await dataService.SaveSessionAsync(session);

        var sessions = await dataService.GetAllSessionsAsync();
        var s = sessions[0];
        s.Location = "Updated";
        await dataService.SaveSessionAsync(s);

        var updated = (await dataService.GetAllSessionsAsync())[0];
        Assert.Equal("Updated", updated.Location);

        File.Delete(dbPath);
    }

    [Fact]
    public async Task CanUpdateHand()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var hand = new Hand { Stakes = "1,2", ProfitLoss = 10 };
        await dataService.SaveHandAsync(hand);

        var hands = await dataService.GetAllHandsAsync();
        var h = hands[0];
        h.ProfitLoss = 99;
        await dataService.SaveHandAsync(h);

        var updated = (await dataService.GetAllHandsAsync())[0];
        Assert.Equal(99, updated.ProfitLoss);

        File.Delete(dbPath);
    }

    [Fact]
    public async Task CanDeleteSession()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var session = new Session { Location = "ForDelete" };
        await dataService.SaveSessionAsync(session);

        var sessions = await dataService.GetAllSessionsAsync();
        Assert.Single(sessions);

        await dataService.DeleteSessionAsync(sessions[0].Id);

        var afterDelete = await dataService.GetAllSessionsAsync();
        Assert.Empty(afterDelete);

        File.Delete(dbPath);
    }

    [Fact]
    public async Task CanDeleteHand()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var hand = new Hand { Stakes = "1,2" };
        await dataService.SaveHandAsync(hand);

        var hands = await dataService.GetAllHandsAsync();
        Assert.Single(hands);

        await dataService.DeleteHandAsync(hands[0].Id);

        var afterDelete = await dataService.GetAllHandsAsync();
        Assert.Empty(afterDelete);

        File.Delete(dbPath);
    }

    [Fact]
    public async Task CanGetSessionById()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var session = new Session { Location = "FindMe" };
        await dataService.SaveSessionAsync(session);

        var sessions = await dataService.GetAllSessionsAsync();
        var found = await dataService.GetSessionByIdAsync(sessions[0].Id);

        Assert.Equal("FindMe", found.Location);
        File.Delete(dbPath);
    }

    [Fact]
    public async Task CanLinkHandToSession()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var session = new Session { Location = "ParentSession" };
        await dataService.SaveSessionAsync(session);

        var sessions = await dataService.GetAllSessionsAsync();
        var sessionId = sessions[0].Id;

        var hand = new Hand { SessionId = sessionId, Stakes = "1,2" };
        await dataService.SaveHandAsync(hand);

        var hands = await dataService.GetAllHandsAsync();
        Assert.Equal(sessionId, hands[0].SessionId);

        File.Delete(dbPath);
    }

    [Fact]
    public async Task SavingDuplicateSessionDoesNotError()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        var session = new Session { Location = "Dupe" };
        await dataService.SaveSessionAsync(session);
        await dataService.SaveSessionAsync(session); // Should not throw

        var sessions = await dataService.GetAllSessionsAsync();
        Assert.Single(sessions);

        File.Delete(dbPath);
    }

    [Fact]
    public async Task EmptyDatabaseReturnsEmptyLists()
    {
        var dbPath = TestDbPath;
        var dataService = new PokerDataService(dbPath);

        Assert.Empty(await dataService.GetAllSessionsAsync());
        Assert.Empty(await dataService.GetAllHandsAsync());

        File.Delete(dbPath);
    }



}
