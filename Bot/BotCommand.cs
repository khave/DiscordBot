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
        private string[] aliases, parameters;
        private bool adminCommand = false;


        //Constructor
        public BotCommand(string command, string description, string usage)
        {
            this.command = command;
            this.description = description;
            this.usage = usage;
        }

        //Constructor
        public BotCommand(string command, string description, string usage, bool adminCommand)
        {
            this.command = command;
            this.description = description;
            this.usage = usage;
            this.adminCommand = adminCommand;
        }

        //Constructor
        public BotCommand(string command, string description, string usage, string[] aliases)
        {
            this.command = command;
            this.description = description;
            this.usage = usage;
            this.aliases = aliases;
        }


        //Constructor
        public BotCommand(string command, string description, string usage, bool adminCommand, string[] aliases)
        {
            this.command = command;
            this.description = description;
            this.usage = usage;
            this.adminCommand = adminCommand;
            this.aliases = aliases;
        }


        public abstract void onCommand(CommandEventArgs e, DiscordClient discord, string[] args);

        public bool requiresAdmin()
        {
            return adminCommand;
        }

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
