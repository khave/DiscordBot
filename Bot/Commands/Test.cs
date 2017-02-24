using Discord.Commands;

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
