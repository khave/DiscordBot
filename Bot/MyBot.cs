using System;
using System.Collections.Generic;
using System.Linq;
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
        public CooldownManager cooldownManager;
        public bool isTesting = false;
        public ulong serverId = 217373774821982210;
        
        /*


            This bot is made for the server "The Soviet Gamers"
            If this bot is used on any other channels, some functions will not work.


        */

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
            cooldownManager = new CooldownManager();

          
          var commands = discord.GetService<CommandService>();
          foreach (BotCommand cmd in this.commands)
          {

                if (cmd.getAliases() == null)
                {
                    commands.CreateCommand(cmd.getCommand()).Description(cmd.getUsage()).Parameter("arg", ParameterType.Unparsed).Do(async (e) =>
                    {
                        if (isInTestMode(e))
                        {
                            await e.Channel.SendMessage("The bot is currently undergoing serious changes! Please try again later");
                            return;
                        }

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
                        if (isInTestMode(e))
                        {
                            await e.Channel.SendMessage("The bot is currently undergoing serious changes! Please try again later");
                            return;
                        }

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

            //Register scripts
            CleverBotScript cleverBotScript = new CleverBotScript(this);
            MessageReceived messageReceived = new MessageReceived(this);


            //discord.SetGame(new Game("use !help for commands"));

            discord.ExecuteAndWait(async () =>
           {
               await discord.Connect("MjcxOTg3MDQ1MTAwOTQ1NDA4.C7wF8Q.iA8cSZ4oFcHjGCxxpu69gwpnBuQ", TokenType.Bot);
               discord.SetGame("DM to contact mods");
           });
        }

        private void load()
        {
            //Commands
            commands.Add(new Commands.Hello());
            commands.Add(new Help(this));
            //commands.Add(new MusicJoin(this));
            commands.Add(new Cat(this));
            //commands.Add(new MusicDownload());
            //commands.Add(new MusicPause(this));
            commands.Add(new Roll(this));
            commands.Add(new Skip(this));
            commands.Add(new Anime());
            commands.Add(new GoogleSearch());
            commands.Add(new nsfw());
            commands.Add(new Info());
            commands.Add(new DeleteMessages());
            commands.Add(new Rule34());
            commands.Add(new Yandere());
            commands.Add(new MusicStop(this));
            commands.Add(new MusicPlay(this));
            commands.Add(new Volume(this));
            commands.Add(new MusicList(this));
            commands.Add(new MusicShuffle(this));
            commands.Add(new SavePlaylist(this));
            commands.Add(new LoadPlaylist(this));
            commands.Add(new PlaylistList(this));
            commands.Add(new DeletePlaylist());
            commands.Add(new SetGame(this));
            //new MusicPlay(this);
            new Test(this);
            audioCommands.Add(new Commands.AudioCommands.Hello());

            //Responses
            //TODO: Load responses from either txt-file or database
            responses.Add(new Response("Mooshii", "porn", 100, "Where?!"));
            responses.Add(new Response("Creeperskull", "Aye", 25, "Aye lmao"));
            responses.Add(new Response("Nickimus", "!hello", 100, "pleb"));
            responses.Add(new Response("Ivanovich Koslov", "Heal", 10, "Heals4Days"));
            responses.Add(new Response("Sadenar", "feck", 100, "fuck*"));
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

        public bool hasAdmin(User user)
        {
            return user.HasRole(discord.GetServer(serverId).FindRoles("BOT MASTERS").First()) || user.HasRole(discord.GetServer(serverId).FindRoles("Generals").First()) || user.Name == "khave";
        }

        public bool isInTestMode(CommandEventArgs e)
        {
            return isTesting && e.User.Name != "khave";
        }

        public CooldownManager getCooldownManager()
        {
            return cooldownManager;
        }
    }
}
