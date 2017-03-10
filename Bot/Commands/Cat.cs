using System;
using Discord;
using Discord.Commands;
using System.Net;
using System.IO;

namespace Bot.Commands
{
    class Cat : BotCommand
    {

        public MyBot myBot;

        public Cat(MyBot myBot) : base("cat", "Get a random cat", "!cat")
        {
            this.myBot = myBot;
        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            if (myBot.getCooldownManager().hasCooldown(e.User.Name + "catCooldown")) return;
            myBot.getCooldownManager().addCooldown(e.User.Name + "catCooldown", 3);

            using (var client = new WebClient())
            {
                await e.Channel.SendMessage("Finding random cat image...");
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
