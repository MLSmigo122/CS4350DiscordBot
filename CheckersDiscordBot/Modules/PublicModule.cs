using CheckersDiscordBot.Services;
using Discord;
using Discord.Commands;

namespace CheckersDiscordBot.Modules
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        //Dependency injecetion
        public PictureService PictureService { get; set; }
        public TCPHandler tcphandler { get; set; }

        bool playing = Program.isPlaying();
        IUser player1 = Program.getPlayer1();
        IUser player2 = Program.getPlayer2();

        [Command("play")]
        public async Task StartGame(IUser user)
        {
            if (!playing && !user.IsBot)
            {
                Program.setPlaying(true);
                Program.setPlayers(Context.User, user);
                //tcphandler.runListener();
                Console.WriteLine("Game started");
                Console.WriteLine("Player1: {0}", player1);
            }
            else
            {
                ReplyAsync("Game already running!");
            }
        }

        [Command("move")]
        public async Task Move(string from, string to)
        {
            Console.WriteLine("Player1: {0}", player1);
            if (playing)
            {
                bool team;
                if (Context.User == player1)
                    team = false;
                else if (Context.User == player2)
                    team = true;
                else
                {
                    ReplyAsync("You're not playing");
                    return;
                }
                Console.WriteLine("Hello");
                tcphandler.SendMove(team, from, to);
            }
            else
                ReplyAsync("No game is being played");
        }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task async() => ReplyAsync("pong!");


        [Command("cat")]
        public async Task CatAsync()
        {
            //Get a stream containing cat picture
            var stream = await PictureService.GetCatPictureAsync();

            //Stream must be seeked to the beginning before upload
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }


        //Get info on user
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;

            await ReplyAsync(user.ToString());
        }


        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);


        // 'params' will parse space-separated elements into a list
        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));


        // Setting a custom ErrorMessage property will help clarify the precondition error
        [Command("guild_only")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public Task GuildOnlyCommand()
            => ReplyAsync("Nothing to see here!");
    }
}
