using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Bot.Commands;

namespace Bot
{

    
    class MyBot
    {
        DiscordClient discord;
        List<BotCommand> commands = new List<BotCommand>();

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

            loadCommands();

            var commands = discord.GetService<CommandService>();
            foreach (BotCommand cmd in this.commands)
            {
                commands.CreateCommand(cmd.getCommand()).Description(cmd.getUsage()).Do(async (e) =>
                {
                    string[] args = e.Message.Text.Split(null);
                    cmd.onCommand(e, discord, args);
                    await e.Channel.SendMessage("");
                });
            }


            //Custom command system
            /*
            discord.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    string[] args = e.Message.Text.Split(null);
                    string msg = e.Message.Text.Replace("@" + discord.CurrentUser.Name + " ", "");
                    foreach (BotCommand cmd in this.commands)
                    {
                        await e.Channel.SendIsTyping();
                        if (msg == cmd.getCommand())
                        {
                            cmd.onCommand(e, discord, args);
                        }
                    }

                }
            };
             */


            discord.ExecuteAndWait(async () =>
           {
               await discord.Connect("MjcxOTg3MDQ1MTAwOTQ1NDA4.C2Oc9A.Y5ng8OYKAWbV9DRBNAl7L5mHsuI", TokenType.Bot);
           });
        }

        private void loadCommands()
        {
            commands.Add(new Hello());
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
