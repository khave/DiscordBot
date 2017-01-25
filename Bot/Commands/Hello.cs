using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class Hello : BotCommand
    {

        public Hello() : base("hello", "Hello World Command!", "hello")
        {
            //some code
        }
            

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            e.Channel.SendMessage("Hello World!");
        }
    }
}
