using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class Hello : BotCommand
    {

        public Hello() : base("hello", "Hello World Command!", "hello", true)
        {
        }
            

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            e.Channel.SendMessage("Hello World!");
        }
    }
}
