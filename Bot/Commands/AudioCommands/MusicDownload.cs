using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands.AudioCommands
{
    class MusicDownload : BotCommand
    {

        public MusicDownload() : base("music download", "Download music", "music download <url>", new string[] { "m d", "md"})
        {

        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {

            if (args == null || args.Length == 0) 
            {
                e.Channel.SendMessage("Missing args :(");
                return;
            }

            foreach(string s in args)
            {
                if(s == "")
                {
                    continue;
                }
                e.Channel.SendMessage(s);
            }
        }
    }
}
