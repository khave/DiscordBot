using Discord;
using Discord.Commands;
using System.Threading;

namespace Bot.Commands.AudioCommands
{
    class MusicStop : BotCommand
    {

        MyBot myBot;

        public MusicStop(MyBot myBot) : base("music stop", "Stop music from playing with bot.", "!music stop", true)
        {
            this.myBot = myBot;
        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            await e.Channel.SendMessage("Stopping music...");
            myBot.audioManager.stop();
            Thread.Sleep(1000); // Sleep for a sec so it can leave voice
            myBot.audioManager.leaveVoiceChannel();
        }
    }
}
