using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Threading;

namespace Bot.Commands.AudioCommands
{
    class MusicStop : BotCommand
    {

        MyBot myBot;

        public MusicStop(MyBot myBot) : base("music stop", "Stop music from playing with bot.", "music stop")
        {
            this.myBot = myBot;
        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            e.Channel.SendMessage("Stopping music...");
            await myBot.audioManager.stop();
            Thread.Sleep(1000); // Sleep for a sec so it can leave voice
            myBot.audioManager.leaveVoiceChannel();
        }
    }
}
