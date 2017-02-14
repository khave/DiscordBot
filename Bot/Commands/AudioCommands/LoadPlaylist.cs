using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Bot.Commands.AudioCommands.Playlists;

namespace Bot.Commands.AudioCommands
{
    class LoadPlaylist : BotCommand
    {

        MyBot myBot;

        public LoadPlaylist(MyBot myBot) : base("music load", "Load a playlist!", "!music load <name>", new string[] { "load", "playlist load" })
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            PlaylistManager playlistManager = new PlaylistManager();
            Console.WriteLine(args[0]);
            if (!playlistManager.playListsExists(args[0]))
            {
                e.Channel.SendMessage("That playlist does not exist!");
                return;
            }

            playlistManager.loadPlaylist(args[0], myBot.audioManager);
            string video = myBot.audioManager.queue.First().url;
            myBot.audioManager.queue.Dequeue();
            myBot.audioManager.SendOnlineAudio(e, video);
        }
    }
}
