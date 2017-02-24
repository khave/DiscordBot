using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;

namespace Bot.Audio
{
    class AudioManager
    {

        MyBot myBot;
        public static IAudioClient _vClient;
        private bool playingSong = false;
        public static DiscordClient _client;
        private bool forceStop = false;
        private int[] resolutions = { 720, 480, 360, 240 };
        private double volume = 1.0;
        public Queue<YouTubeVideo> queue = new Queue<YouTubeVideo>();
        public List<string> votes = new List<string>();
        public int currentVotes = 0;
        private bool isSkipping = false;
        Task currentPlayTask;
        WebClient webClient;
        public YouTubeVideo currentSong;
        

        public AudioManager(MyBot myBot)
        {
            this.myBot = myBot;
            _client = myBot.discord;
            this.webClient = new WebClient();
            myBot.discord.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });
        }

        public bool isMusicChannel(CommandEventArgs e)
        {
            if (e.Channel.Name != "music")
            {
                e.Channel.SendMessage("Please only do music commands in #music");
                return false;
            }
            return true;
        }

        public void sendMessage(string message)
        {
            myBot.discord.FindServers("The Soviet Gamers").First().FindChannels("music").First().SendMessage(message);
        }

        public void setVolume(int volume)
        {
            this.volume = ((double) volume/100);
        }

        public async 
        Task
joinVoiceChannel(CommandEventArgs e)
        {
            if (e.User.VoiceChannel == null)
            {
                e.Channel.SendMessage("ERROR: You're not in a channel");
                return;
            }

            _vClient = await _client.GetService<AudioService>().Join(e.User.VoiceChannel);
        }

        //Sorry for shit code. I'll fix later.
        public bool isInSameVoice(CommandEventArgs e)
        {
            if(e.User.VoiceChannel == null)
            {
                return false;
            }
            //Console.WriteLine(e.User.VoiceChannel + " != " + _vClient.Channel);
            if(_vClient != null && e.User.VoiceChannel != _vClient.Channel)
            {
                return false;
            }
            return true;
        }

        public async void leaveVoiceChannel()
        {
            if (_vClient == null || playingSong) return;
            await _vClient.Disconnect();
        }

        public void stop()
        {
            Console.WriteLine("Stopping music bot...");
            queue.Clear();
            currentSong = null;
            playingSong = false;
        }

        public async void skip(CommandEventArgs e)
        {
            if (!isInSameVoice(e))
            {
                await e.Channel.SendMessage("You are not in the same voice as me!");
                return;
            }

            if(currentSong != null && currentSong.requester == e.User.Name)
            {
                playingSong = false;
                await e.Channel.SendMessage("Skipping song...");
                votes.Clear();
                return;
            }

            if (votes.Contains(e.User.Name))
            {
                await e.Channel.SendMessage("You have already voted to skip!");
                return;
            }
            votes.Add(e.User.Name);
            //Users in voice - the bot
            int usersInVoice = _vClient.Channel.Users.Count() - 1;
            //Get needed votes (40% of current users)
            int neededVotes = Convert.ToInt32(Math.Round(usersInVoice * 0.5));
            await e.Channel.SendMessage(e.User.Name + " has voted to skip the current song! " + votes.Count() + "/" + neededVotes);
            if(votes.Count() >= neededVotes)
            {
                playingSong = false;
                await e.Channel.SendMessage("Skipping song...");
                votes.Clear();
            }
        }

        public async void SendOnlineAudio(CommandEventArgs e, string pathOrUrl)
        {
            try
            {
                await joinVoiceChannel(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("WARNING: Bot failed!");
                e.Channel.SendMessage("ERROR: Bot failed");
                return;
            }

            if (!isInSameVoice(e))
            {
                e.Channel.SendMessage("You are not in the same voice as me!");
                return;
            }

            YouTubeVideo ytVideo = new YouTubeVideo(pathOrUrl, e.User.Name);

            if (playingSong)
            {
                queue.Enqueue(ytVideo);
                sendMessage(ytVideo.requester + " added `" + ytVideo.title + "` **" + ytVideo.duration + "** to the queue! **[" + queue.Count + "]**");
                return;
            }

            currentSong = ytVideo;

            if (_vClient == null)
            {
                e.Channel.SendMessage("You are not in a voice channel!");
                return;
            }

            playingSong = true;
            sendMessage("Playing `" + currentSong.title + "` **" + currentSong.duration + "**");

            var process = Process.Start(new ProcessStartInfo
            { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
                FileName = "cmd.exe",
                Arguments = "/C youtube-dl.exe -f 140  -o - " + pathOrUrl + " -q -i | ffmpeg.exe -i pipe:0 -f s16le -ar 48000 -ac 2 pipe:1 -af \"volume=" + volume + "\" -loglevel quiet", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout.
                UseShellExecute = false,
                RedirectStandardOutput = true, // Capture the stdout of the process
                RedirectStandardError = false,
            });

            //Thread.Sleep(1500); // Sleep for a few seconds to FFmpeg can start processing data.

            int blockSize = 3840; // The size of bytes to read per frame; 1920 for mono
            byte[] buffer = new byte[blockSize];
            int byteCount;

            while (playingSong) // Loop forever, so data will always be read
            {

                if (!playingSong || process.HasExited)
                {
                    Console.WriteLine("task canceled");
                    break;
                }

                byteCount = process.StandardOutput.BaseStream // Access the underlying MemoryStream from the stdout of FFmpeg
                        .Read(buffer, 0, blockSize); // Read stdout into the buffer


                if (byteCount == 0) // FFmpeg did not output anything
                    break; // Break out of the while(true) loop, since there was nothing to read.

                try
                {
                    _vClient.Send(buffer, 0, byteCount); // Send our data to Discord
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine("Operation cancelled!");
                    break;
                }
            }

            _vClient.Clear();
            _vClient.Wait(); // Wait for the Voice Client to finish sending data, as ffMPEG may have already finished buffering out a song, and it is unsafe to return now.

            if (!process.HasExited)
            {
                process.Kill();
                process.Dispose();
                Console.WriteLine("Killed process");
            }
            //set current votes to 0, if a vote was underway, but not enough voted to skip
            votes.Clear();

            playingSong = false;

            if (!queue.Any())
            {
                sendMessage("No songs in queue! Disconnecting...");
                leaveVoiceChannel();
                return;
            }

            string video = queue.First().url;
            queue.Dequeue();
            //Thread.Sleep(1000); //Sleep for a sec so it can stop music              
                                //Play next song
                                //await Task.Run(() => SendOnlineAudio(e, video), ct);
            SendOnlineAudio(e, video);
        }

        public void setQueue(Queue<YouTubeVideo> queue)
        {
            this.queue = queue;
        }
    }
}
