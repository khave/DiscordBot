using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    class Test
    {
        MyBot myBot;

        public Test(MyBot myBot)
        {
            this.myBot = myBot;
            onCommand();
        }

        public void onCommand()
        {
            myBot.discord.GetService<CommandService>().CreateCommand("test").Alias(new string[] { "t" })
                .Parameter("test", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Args: " + e.GetArg("test"));
                });
        }
    }
}
