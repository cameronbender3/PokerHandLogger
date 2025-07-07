using Microsoft.Extensions.Logging;
using System.IO;
namespace MauiApp1;

using Poker.Core.Services;
using Poker.Data.Services;
using Poker.Library.Services;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
        {
            Console.WriteLine("Starting MauiApp creation...");
            
            Console.WriteLine("Initializing SQLite...");
            SQLitePCL.Batteries_V2.Init();
            Console.WriteLine("SQLite initialized successfully");
            
            var builder = MauiApp.CreateBuilder();
            Console.WriteLine("Builder created");
            
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            Console.WriteLine("Basic configuration done");

            Console.WriteLine("Adding services...");
            builder.Services.AddSingleton<IPokerLogicService, PokerLogicService>();
            
            Console.WriteLine("Adding database service...");
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "poker.db3");
            Console.WriteLine($"Database path: {dbPath}");
            builder.Services.AddSingleton<IPokerDataService>(provider =>
                new PokerDataService(dbPath));
            Console.WriteLine("Database service added");

#if DEBUG
            builder.Logging.AddDebug();
#endif

            Console.WriteLine("Building app...");
            var app = builder.Build();
            Console.WriteLine("App built successfully");
            
            return app;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateMauiApp: {ex}");
            throw;
        }
    }
}