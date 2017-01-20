using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;

namespace Bot.Audio
{
    class AudioManager
    {

        MyBot myBot;
        
        public AudioManager(MyBot myBot)
        {
            this.myBot = myBot;
            myBot.discord.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });
        }

        public void joinVoiceChannel(CommandEventArgs e, User user)
        {
            if(user.VoiceChannel == null)
            {
                e.Channel.SendMessage("not in channel");
            }
            e.Channel.SendMessage("Channel found: " + user.VoiceChannel.Name);

            var voiceChannel = myBot.discord.FindServers("General").FirstOrDefault().VoiceChannels.FirstOrDefault(); // Finds the first VoiceChannel on the server 'Music Bot Server'

            var _vClient = myBot.discord.GetService<AudioService>() // We use GetService to find the AudioService that we installed earlier. In previous versions, this was equivelent to _client.Audio()
                    .Join(voiceChannel); // Join the Voice Channel, and return the IAudioClient.
        }
    }
}
