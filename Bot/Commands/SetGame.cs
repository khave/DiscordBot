using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class SetGame : BotCommand
    {

        public MyBot myBot;

        public SetGame(MyBot myBot) : base("setgame", "Set the game the bot is playing", "!setgame <text>")
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            if(args[0] == "")
            {
                e.Channel.SendMessage("Please input the game text!");
                return;
            }
            string gameText = string.Join(" ", args);
            myBot.discord.SetGame(gameText);
        }
    }
}
