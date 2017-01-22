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

        public Cat() : base("cat", "Get a random cat")
        {

        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            using (var client = new WebClient())
            {
                e.Channel.SendMessage("Finding random cat image...");
                client.DownloadFile("http://thecatapi.com/api/images/get?format=src&type=png", @"C:\Users\Kristian\Source\Repos\DiscordBot\cat.png");
                await e.Channel.SendFile(@"C:\Users\Kristian\Source\Repos\DiscordBot\cat.png");
                if (File.Exists(@"C:\Users\Kristian\Source\Repos\DiscordBot\cat.png"))
                {
                    File.Delete(@"C:\Users\Kristian\Source\Repos\DiscordBot\cat.png");
                }
            }
        }
    }
}
