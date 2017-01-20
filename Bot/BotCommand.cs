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

        private string command, usage;
        private string[] aliases;


        //Constructor
        public BotCommand(string command, string usage)
        {
            this.command = command;
            this.usage = usage;
        }

        //Constructor
        public BotCommand(string command, string usage, string[] aliases) : this(command, usage)
        {
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

        public string[] getAliases()
        {
            return aliases;
        }
    }
}
