using Discord;
using Discord.Commands;
using System;

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
            int queueDuration = 0;

            foreach (YouTubeVideo video in myBot.audioManager.queue)
            {
                complete += "\n" + video.title + "[" + video.duration + "]" + " requested by " + video.requester;
                queueDuration += video.rawDuration;
            }
            TimeSpan t = TimeSpan.FromSeconds(queueDuration);
            string answer = string.Format("{1:D2}:{2:D2}",
            t.Hours,
            t.Minutes,
            t.Seconds,
            t.Milliseconds);
            e.Channel.SendMessage(complete + "```"+
                "\nTotal Queue Duration: " + answer);
        }
    }
}
