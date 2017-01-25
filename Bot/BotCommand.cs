using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot
{
    abstract class BotCommand
    {

        private string command, description, usage;
        private string[] aliases;


        //Constructor
        public BotCommand(string command, string description, string usage)
        {
            this.command = command;
            this.description = description;
            this.usage = usage;
        }

        //Constructor
        public BotCommand(string command, string description, string usage, string[] aliases)
        {
            this.command = command;
            this.description = description;
            this.usage = usage;
            this.aliases = aliases;
        }

   
        public abstract void onCommand(CommandEventArgs e, DiscordClient discord, String[] args);

        public string getCommand()
        {
            return command;
        }

        public string getUsage()
        {
            return usage;
        }

        public string getDescription()
        {
            return description;
        }

        public string[] getAliases()
        {
            return aliases;
        }
    }
}
