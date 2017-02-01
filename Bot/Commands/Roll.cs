﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Bot.Commands
{
    class Roll : BotCommand
    {

        MyBot myBot;
        Random r = new Random();

        public Roll(MyBot myBot) : base("roll", "Roll a dice!", "roll <type> <times>")
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            int diceNum;
            try
            {
                diceNum = Convert.ToInt32(args[0]);
                int rInt = r.Next(1, diceNum + 1); //for ints
                if(args.Length > 1) //test
                {
                    int times = Convert.ToInt32(args[1]);
                    int result = rInt;
                    string output = " Rolled D" + diceNum + ": " + rInt;
                    for (int i = 1; i < times; i++)
                    {
                        rInt = r.Next(1, diceNum + 1); //for ints
                        result += rInt;
                        output += " + " + rInt;
                    }
                    e.Channel.SendMessage(e.User.Mention + output + " = " + result);
                    return;
                }
                 e.Channel.SendMessage(e.User.Mention + " Rolled D" + diceNum + ": " + rInt);
            }
            catch (FormatException ex)
            {
                e.Channel.SendMessage("That's not a number!");
            }
        }
    }
}
