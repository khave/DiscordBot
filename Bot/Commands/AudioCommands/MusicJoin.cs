using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands.AudioCommands
{
    class MusicJoin : BotCommand
    {

        MyBot myBot;

        public MusicJoin(MyBot myBot) : base("music join", "Be in a voice channel, and the bot will join you", "music join")
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            e.Channel.SendMessage("Connecting to YouTubeExtractor...");
            //myBot.audioManager.joinVoiceChannel(e, e.User);
        }
    }
}
