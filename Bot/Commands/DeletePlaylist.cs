using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            PlaylistManager playlistManager = new PlaylistManager();
            if (!playlistManager.playListsExists(args[0]))
            {
                e.Channel.SendMessage("No such playlist exists!");
                return;
            }

            playlistManager.deletePlaylist(args[0]);
            e.Channel.SendMessage("Deleted the playlist!");
        }
    }
}
