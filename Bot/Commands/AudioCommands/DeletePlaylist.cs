using System;
using Discord;
using Discord.Commands;
using Bot.Commands.AudioCommands.Playlists;

namespace Bot.Commands
{
    class DeletePlaylist : BotCommand
    {

        public DeletePlaylist() : base("music delete", "Delete a playlist", "!music delete <name>", true)
        {

        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            string playlist = String.Join(" ", args);

            PlaylistManager playlistManager = new PlaylistManager();
            if (!playlistManager.playListsExists(playlist))
            {
                e.Channel.SendMessage("No such playlist exists!");
                return;
            }

            playlistManager.deletePlaylist(playlist);
            e.Channel.SendMessage("Deleted the playlist!");
        }
    }
}
