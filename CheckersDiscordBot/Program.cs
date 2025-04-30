using CheckersDiscordBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace CheckersDiscordBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var listener = services.GetRequiredService<MyTCPListener>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            // Tokens should be considered secret data and never hard-coded.
            // We can read from the environment variable to avoid hard coding.
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            listener.runListener();

            await Task.Delay(Timeout.Infinite);
        }

        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                })
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .AddSingleton<MyTCPListener>()
                .BuildServiceProvider();
        }
    }
}