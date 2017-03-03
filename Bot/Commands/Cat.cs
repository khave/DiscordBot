using System;
using Discord;
using Discord.Commands;
using System.Net;
using System.IO;

namespace Bot.Commands
{
    class Cat : BotCommand
    {

        Loader loader;

        public Cat() : base("cat", "Get a random cat", "!cat")
        {

        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            using (var client = new WebClient())
            {
                Message message = await e.Channel.SendMessage("Finding random cat image...");
                loader = new Loader(message);
                try
                {
                    client.DownloadFile("http://thecatapi.com/api/images/get?format=src&type=png", @".\cat.png");
                    await e.Channel.SendFile(@".\cat.png");
                    if (File.Exists(@".\cat.png"))
                    {
                        File.Delete(@".\cat.png");
                    }
                }
                catch (Exception ex)
                {
                    await e.Channel.SendMessage("An error occured :(");
                }
            }
        }
    }
}
