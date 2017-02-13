using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class MusicList : BotCommand
    {

        MyBot myBot;
        
        public MusicList(MyBot myBot) : base("music list", "List the queue of music", "!music list", new string[] { "list" })
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            string complete = "`Currently Playing " + myBot.audioManager.currentSong.title + " [" + myBot.audioManager.currentSong.duration + "]" + " requested by " + myBot.audioManager.currentSong.requester + "`\n```";


            foreach (YouTubeVideo video in myBot.audioManager.queue)
            {
                complete += "\n" + video.title + "[" + video.duration + "]" + " requested by " + video.requester;

            }
            e.Channel.SendMessage(complete + "```");
        }
    }
}
