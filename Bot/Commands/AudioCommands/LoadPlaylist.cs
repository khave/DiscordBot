using System;
using System.Linq;
using Discord;
using Discord.Commands;
using Bot.Commands.AudioCommands.Playlists;

namespace Bot.Commands.AudioCommands
{
    class LoadPlaylist : BotCommand
    {

        MyBot myBot;

        public LoadPlaylist(MyBot myBot) : base("music load", "Load a playlist!", "!music load <name/youtube url>", new string[] { "load", "playlist load" })
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            string playlist = String.Join(" ", args);
            PlaylistManager playlistManager = new PlaylistManager();
            if (!playlistManager.playListsExists(playlist) && !playlist.Contains("www.youtube"))
            {
                e.Channel.SendMessage("That playlist does not exist!");
                return;
            }

            playlistManager.loadPlaylist(playlist, e.User.Name, myBot.audioManager);
            string video = myBot.audioManager.queue.First().url;
            myBot.audioManager.queue.Dequeue();
            myBot.audioManager.SendOnlineAudio(e, video);
        }
    }
}
