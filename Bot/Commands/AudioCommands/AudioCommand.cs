using System;
using Discord;
using Discord.Commands;

namespace Bot.Commands.AudioCommands
{
    abstract class AudioCommand
    {

        private string command, usage;
        private string[] aliases;


        //Constructor
        public AudioCommand(string command, string usage)
        {
            this.command = command;
            this.usage = usage;
        }

        //Constructor
        public AudioCommand(string command, string usage, string[] aliases) : this(command, usage)
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
