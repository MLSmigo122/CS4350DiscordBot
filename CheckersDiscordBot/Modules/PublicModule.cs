using CheckersDiscordBot.Services;
using Discord;
using Discord.Commands;

namespace CheckersDiscordBot.Modules
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        //Dependency injecetion
        public PictureService PictureService { get; set; }


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
