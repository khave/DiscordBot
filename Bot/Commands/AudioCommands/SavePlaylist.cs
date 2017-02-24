using System;
using Discord;
using Discord.Commands;
using Bot.Commands.AudioCommands.Playlists;

namespace Bot.Commands.AudioCommands
{
    class SavePlaylist : BotCommand
    {

        MyBot myBot;

        public SavePlaylist(MyBot myBot) : base("music save", "Save a playlist for later use!", "!music save <name>", new string[] { "save", "playlist save" })
        {
            this.myBot = myBot;
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            string playlist = String.Join(" ", args);
            PlaylistManager playlistManager = new PlaylistManager();
            playlistManager.savePlaylist(playlist, myBot.audioManager.queue);
            myBot.audioManager.sendMessage("Saved the playlist!");
        }
    }
}
