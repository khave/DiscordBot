using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Net;
using System.IO;

namespace Bot.Commands
{
    class Cat : BotCommand
    {

        public Cat() : base("cat", "Get a random cat", "cat")
        {

        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            using (var client = new WebClient())
            {
                e.Channel.SendMessage("Finding random cat image...");
                client.DownloadFile("http://thecatapi.com/api/images/get?format=src&type=png", @".\cat.png");
                await e.Channel.SendFile(@".\cat.png");
                if (File.Exists(@".\cat.png"))
                {
                    File.Delete(@".\cat.png");
                }
            }
        }
    }
}
