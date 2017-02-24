using System;
using System.Linq;
using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class DeleteMessages : BotCommand
    {

        public DeleteMessages() : base("clear", "Clear all recent messages", "!clear <number of messages>", true, new string [] { "delmessages", "clearmessages", "deletemessages", "delete" })
        {

        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            int count;
            try
            {
                count = Convert.ToInt32(args[0])+1;
                int msgCount = e.Channel.Messages.Count();
                Message[] messages = e.Channel.Messages.Skip(msgCount - count).ToArray();
                await e.Channel.DeleteMessages(messages);
            }
            catch (FormatException ex)
            {
                await e.Channel.SendMessage("That's not a number!");
            }
        }
    }
}
