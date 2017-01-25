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

        public Help(MyBot myBot) : base("help", "Shows you a list of commands and how to use them", "help")
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            string complete = "```";
            foreach(BotCommand cmd in myBot.commands)
            {
                complete += "\n" + cmd.getCommand() + "      -       " + cmd.getUsage();
            }
            e.Channel.SendMessage(complete + "```");
            myBot.discord.GetService<CommandService>().ShowGeneralHelp(e.User, e.Channel);
        }
    }
}
