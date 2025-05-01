using CheckersDiscordBot.Modules;
using CheckersDiscordBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace CheckersDiscordBot
{
    public class Program
    {
        static bool playing = false;
        static ISocketMessageChannel channel;
        static IUser player1;
        static IUser player2;

        public static async Task Main(string[] args)
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var listener = services.GetRequiredService<TCPHandler>();

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
                    GatewayIntents = GatewayIntents.All//GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                })
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .AddSingleton<TCPHandler>()
                .AddSingleton<PublicModule>()
                .BuildServiceProvider();
        }

        public static void setPlaying(bool p) { playing = p; }
        public static bool isPlaying() { return playing; }
        public static void setChannel(ISocketMessageChannel ch) { channel = ch; }
        public static void setPlayers(IUser user1, IUser user2) {player1 = user1; player2 = user2; }
        public static IUser getPlayer1() { return player1; }
        public static IUser getPlayer2() { return player2; }
        public static void sendMsg(string msg) { channel.SendMessageAsync(msg); }
    }
}