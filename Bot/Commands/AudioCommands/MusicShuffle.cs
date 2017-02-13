using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands.AudioCommands
{
    class MusicShuffle : BotCommand
    {

        MyBot myBot;
        private Random rng = new Random();


        public MusicShuffle(MyBot myBot) : base("music shuffle", "Shuffle the music queue", "!music shuffle", true)
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            var list = myBot.audioManager.queue.ToList();
            list.OrderBy(item => rng.Next());
            myBot.audioManager.queue = new Queue<YouTubeVideo>(list);
            e.Channel.SendMessage("Shuffled the queue!");
        }
    }
}
    
