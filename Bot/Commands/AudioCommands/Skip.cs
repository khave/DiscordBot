using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class Skip : BotCommand
    {

        MyBot myBot;

        public Skip(MyBot myBot) : base("music skip", "Vote skip the current song", "music skip", new string[] { "skip" })
        {
            this.myBot = myBot;
        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            if (!myBot.audioManager.isMusicChannel(e)) return;

            myBot.audioManager.skip(e);
        }
    }
}
