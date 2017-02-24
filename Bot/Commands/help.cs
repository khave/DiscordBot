using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    class Help : BotCommand
    {

        MyBot myBot;

        public Help(MyBot myBot) : base("help", "Shows you a list of commands and how to use them", "help")
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            string complete = "```";
            foreach(BotCommand cmd in myBot.commands)
            {
                complete += "\n" + calculateSpaces(cmd);
            }
            e.Channel.SendMessage(complete + "```");
        }


        public string calculateSpaces(BotCommand cmd)
        {
            int cmdLength = cmd.getCommand().Length;
            int usageLength = cmd.getUsage().Length;
            int spaceforcmdLength = 30 - cmdLength;
            int spaceforusageLength = 30 - usageLength;
            string spaces = cmd.getCommand() + "";
            for(int i = 1; i < spaceforcmdLength; i++)
            {
                spaces += " ";
            }

            spaces += " - ";

            for (int i = 1; i < spaceforusageLength; i++)
            {
                spaces += " ";
            }
            spaces += cmd.getUsage();
            return spaces;
        }
    }
}
