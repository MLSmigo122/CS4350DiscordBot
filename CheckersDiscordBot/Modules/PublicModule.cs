using CheckersDiscordBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

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
            Program.setChannel(Context.Channel);
            if (user == null)
            {
                ReplyAsync("Please mention a user to play against");
                return;
            }
            if (!playing && !user.IsBot)
            {
                tcphandler.SendReset();
                Program.setPlaying(true);
                Program.setPlayers(Context.User, user);
                Console.WriteLine("Game started");
                ReplyAsync("Game started");
            }
            else
            {
                ReplyAsync("Game unable to start");
            }
        }

        [Command("forfeit")]
        public async Task ForfeitGame()
        {
            if (playing)
            {
                IUser forfeitee = Context.User;
                ReplyAsync(forfeitee + " forfeit the game");
                Program.setPlaying(false);
            }
        }

        [Command("move")]
        [Alias("Move")]
        public async Task Move(string from, string to)
        {
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
                tcphandler.SendMove(team, from, to);
            }
            else
                ReplyAsync("No game is being played");
        }

        [Command("jump")]
        [Alias("Jump")]
        public async Task Jump(string from, params string[] jumps)
        {
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
                tcphandler.SendJump(team, from, jumps);
            }
            else
                ReplyAsync("No game is being played");
        }

        [Command("cat")]
        public async Task CatAsync()
        {
            //Get a stream containing cat picture
            var stream = await PictureService.GetCatPictureAsync();

            //Stream must be seeked to the beginning before upload
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }
    }
}
