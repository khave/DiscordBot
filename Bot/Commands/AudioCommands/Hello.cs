﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands.AudioCommands
{
    class Hello : AudioCommand
    {

        public Hello() : base("hello", "Hello World Command!")
        {
            //some code
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            e.Channel.SendMessage("Hello World! arg 0 = " + args[0]);
        }
    }
}