using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands.AudioCommands
{
    class Music : BotCommand
    {
        MyBot myBot;

        public Music(MyBot myBot) : base("music", "Basic music command!")
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            e.Channel.SendMessage("work in progress");
            return;
            var commands = discord.GetService<CommandService>();
            foreach (AudioCommand cmd in myBot.audioCommands)
            {
                string messagecmd = e.Message.Text.Replace("music", "").Replace("@" + myBot.discord.CurrentUser.Name + " ", "").Replace("!", "");
                e.Channel.SendMessage(messagecmd + " == " + cmd.getCommand());
                if (messagecmd == cmd.getCommand()) {
                    args = messagecmd.Split(null);
                    cmd.onCommand(e, discord, args);
                }
            }
        }
    }
}
