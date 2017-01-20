using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class Help : BotCommand
    {

        MyBot myBot;

        public Help(MyBot myBot) : base("help", "Shows you a list of commands and how to use them")
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            foreach(BotCommand cmd in myBot.commands)
            {
                e.Channel.SendMessage("```" + cmd.getCommand() + "      -       " + cmd.getUsage() + "```");
            }
        }
    }
}
