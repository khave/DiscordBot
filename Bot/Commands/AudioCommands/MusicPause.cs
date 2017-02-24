using Discord;
using Discord.Commands;

namespace Bot.Commands.AudioCommands
{
    class MusicPause : BotCommand
    {

        MyBot myBot;

        public MusicPause(MyBot myBot) : base("music pause", "Pause music", "music pause")
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            e.Channel.SendMessage("Pausing");
            //myBot.audioManager.pause();
        }
    }
}
