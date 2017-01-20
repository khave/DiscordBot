using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot
{

    
    class MyBot
    {
        DiscordClient discord;

        public MyBot()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            var commands = discord.GetService<CommandService>();
            commands.CreateCommand("hello").Do(async (e) =>
            {
                e.Channel.SendMessage("World!");
            });

            discord.ExecuteAndWait(async () =>
           {
               await discord.Connect("MjcxOTg3MDQ1MTAwOTQ1NDA4.C2Oc9A.Y5ng8OYKAWbV9DRBNAl7L5mHsuI", TokenType.Bot);
           });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
