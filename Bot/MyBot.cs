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

                if (cmd.getAliases() == null)
                {
                    commands.CreateCommand(cmd.getCommand()).Description(cmd.getUsage()).Parameter("arg", ParameterType.Unparsed).Do(async (e) =>
                    {
                        if (cmd.requiresAdmin() && !hasAdmin(e))
                        {
                            await e.Channel.SendMessage("That is an admin command, and you are not admin!");
                            return;
                        }

                        string[] args = e.GetArg("arg")?.Replace(cmd.getCommand(), "").Split(null);
                        cmd.onCommand(e, discord, args);
                    });
                }
                else {
                    commands.CreateCommand(cmd.getCommand()).Description(cmd.getUsage()).Alias(cmd.getAliases()).Parameter("arg", ParameterType.Unparsed).Do(async (e) =>
                    {
                        if (cmd.requiresAdmin() && !hasAdmin(e))
                        {
                            await e.Channel.SendMessage("That is an admin command, and you are not admin!");
                            return;
                        }

                        string[] args = e.GetArg("arg")?.Replace(cmd.getCommand(), "").Split(null);
                        cmd.onCommand(e, discord, args);
                    });
                }
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

            //Register scripts
            CleverBotScript cleverBotScript = new CleverBotScript(this);
            MessageReceived messageReceived = new MessageReceived(this);


            //discord.SetGame(new Game("use !help for commands"));

            discord.ExecuteAndWait(async () =>
           {
               await discord.Connect("MjcxOTg3MDQ1MTAwOTQ1NDA4.C21F9Q.9OC5Lt1uMKyLVPoqhL-Rzp10mOo", TokenType.Bot);
               discord.SetGame("Use !help for commands");
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
            commands.Add(new MusicPlay(this));
            commands.Add(new Roll(this));
            commands.Add(new Volume(this));
            commands.Add(new Skip(this));
            commands.Add(new Anime());
            commands.Add(new GoogleSearch());
            //new MusicPlay(this);
            new Test(this);
            audioCommands.Add(new Commands.AudioCommands.Hello());

            //Responses
            //TODO: Load responses from either txt-file or database
            responses.Add(new Response("khave", "test", 100, "Test response!"));
            responses.Add(new Response("Mooshii", "porn", 100, "Where?!"));
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

        public bool hasAdmin(CommandEventArgs e)
        {
            return e.User.HasRole(e.Server.FindRoles("BOT MASTERS").First()) || e.User.HasRole(e.Server.FindRoles("Generals").First()) || e.User.Name == "khave";
        }
    }
}
