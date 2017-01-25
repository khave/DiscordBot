using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Bot.Commands;
using Bot.Responses;
using Bot.Commands.AudioCommands;
using Bot.Audio;
using Bot.Events;

namespace Bot
{

    
    class MyBot
    {
        public DiscordClient discord;
        public List<BotCommand> commands = new List<BotCommand>();
        public List<AudioCommand> audioCommands = new List<AudioCommand>();
        public List<Response> responses = new List<Response>();
        public AudioManager audioManager;

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
              

            load();

          audioManager = new AudioManager(this);

          
          var commands = discord.GetService<CommandService>();
          foreach (BotCommand cmd in this.commands)
          {
              commands.CreateCommand(cmd.getCommand()).Description(cmd.getUsage()).Do(async (e) =>
              {
                  string[] args = e.Message.Text.Replace(cmd.getCommand(), "").Split(null);
                  cmd.onCommand(e, discord, args);
                  await e.Channel.SendMessage("");
              });
          }
            new MusicPlay(this);





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

            //Register scripts
            CleverBotScript cleverBotScript = new CleverBotScript(this);
            //MessageReceived messageReceived = new MessageReceived(this);

            discord.ExecuteAndWait(async () =>
           {
               await discord.Connect("MjcxOTg3MDQ1MTAwOTQ1NDA4.C2Oc9A.Y5ng8OYKAWbV9DRBNAl7L5mHsuI", TokenType.Bot);
           });
        }

        private void load()
        {
            //Commands
            commands.Add(new Commands.Hello());
            commands.Add(new Help(this));
            commands.Add(new MusicJoin(this));
            commands.Add(new Cat());
            commands.Add(new MusicDownload());
            commands.Add(new MusicPause(this));
            commands.Add(new MusicStop(this));
            audioCommands.Add(new Commands.AudioCommands.Hello());

            //Responses
            //TODO: Load responses from either txt-file or database
        }

        private void save()
        {
            //TODO: Save responses
        }

        public List<Response> getResponses()
        {
            return responses;
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
