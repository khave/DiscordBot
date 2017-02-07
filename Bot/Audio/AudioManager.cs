using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System.Diagnostics;
using System.Threading;
using System.IO;
using YoutubeExtractor;
using NAudio.Wave;
using VideoLibrary;
using System.Net;
using System.Collections.Generic;

namespace Bot.Audio
{
    class AudioManager
    {

        MyBot myBot;
        public static IAudioClient _vClient;
        private static bool playingSong = false;
        public static DiscordClient _client;
        private bool forceStop = false;
        private int[] resolutions = { 720, 480, 360, 240 };
        private double volume = 1.0;
        public List<string> queue = new List<string>();
        private Boolean isSkipping = false;

        public AudioManager(MyBot myBot)
        {
            this.myBot = myBot;
            _client = myBot.discord;
            myBot.discord.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });
        }

        public void setVolume(int volume)
        {
            this.volume = ((double) volume/100);
        }

        public async 
        Task
joinVoiceChannel(CommandEventArgs e)
        {

            if(_vClient != null)
            {
                return;
            }

            if (e.User.VoiceChannel == null)
            {
                await e.Channel.SendMessage("ERROR: You're not in a channel");
                return;
            }
            
            _vClient = await _client.GetService<AudioService>().Join(e.User.VoiceChannel);
        }

        public async void leaveVoiceChannel()
        {
            await _vClient.Disconnect();
            _vClient = null;
        }

        public void stop()
        {
            playingSong = false;
            Console.WriteLine("Stopping music bot...");
        }

        public async void skip(CommandEventArgs e)
        {
            if (!queue.Any())
            {
                e.Channel.SendMessage("No songs in queue!");
                return;
            }

            if (queue.Count != 0)
            {
                isSkipping = true;
                stop();
                string video = queue.ElementAt(0);
                queue.RemoveAt(0);
                Thread.Sleep(1000); //Sleep for a sec so it can stop music
                e.Channel.SendMessage("Playing next song in queue...");
                await Task.Run(() => SendOnlineAudio(e, video));
            }
        }

        public async void SendOnlineAudio(CommandEventArgs e, string pathOrUrl)
        {   
            await joinVoiceChannel(e);
            Console.WriteLine("Joined voice");

            if (_vClient == null)
            {
                e.Channel.SendMessage("You are not in a voice channel!");
                return;
            }

            if (playingSong)
            {
                queue.Add(pathOrUrl);
                e.Channel.SendMessage("Added song to the queue! **[" + queue.IndexOf(pathOrUrl) + "]**");
                return;
            }
            else
            {
                playingSong = true;
            }

            Console.WriteLine("Spawned process");

            var process = Process.Start(new ProcessStartInfo
            { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
                FileName = "cmd.exe",
                Arguments = "/C youtube-dl.exe -f 140  -o - " + pathOrUrl + " -q | ffmpeg.exe -i pipe:0 -f s16le -ar 48000 -ac 2 pipe:1 -af \"volume=" + volume + "\" -loglevel quiet", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout.
                UseShellExecute = false,
                RedirectStandardOutput = true, // Capture the stdout of the process
                RedirectStandardError = false,
            });

            //Thread.Sleep(1000); // Sleep for a few seconds to FFmpeg can start processing data.

            int blockSize = 3840; // The size of bytes to read per frame; 1920 for mono
            byte[] buffer = new byte[blockSize];
            int byteCount;

            Console.WriteLine("Started sending audio");
            while (playingSong) // Loop forever, so data will always be read
            {

                if (!playingSong || process.HasExited) break;

                byteCount = process.StandardOutput.BaseStream // Access the underlying MemoryStream from the stdout of FFmpeg
                        .Read(buffer, 0, blockSize); // Read stdout into the buffer

                if (byteCount == 0) // FFmpeg did not output anything
                    break; // Break out of the while(true) loop, since there was nothing to read.

                try {
                    _vClient.Send(buffer, 0, byteCount); // Send our data to Discord
                }catch(OperationCanceledException ex)
                {
                    Console.WriteLine("Operation cancelled!");
                    break;
                }
            }

            Console.WriteLine("Stopped sending audio");

            try {
                _vClient.Wait(); // Wait for the Voice Client to finish sending data, as ffMPEG may have already finished buffering out a song, and it is unsafe to return now.
                _vClient.Clear();
            }catch(OperationCanceledException ex)
            {
                Console.WriteLine("Operation cancelled at waiting");
            }
            if (!process.HasExited)
            {
                process.Kill();
                process.Dispose();
            }
            playingSong = false;
            Console.WriteLine("Reached end of function");
            //Check if there are songs in the queue
            if (queue.Count != 0 && isSkipping)
            {
                isSkipping = false;
                stop();
                string video = queue.ElementAt(0);
                queue.RemoveAt(0);
                Thread.Sleep(1000); //Sleep for a sec so it can stop music
                e.Channel.SendMessage("Playing next song in queue...");
                await Task.Run(() => SendOnlineAudio(e, video));
            }
            //leaveVoiceChannel();
        }

        private void KillAllFFMPEG()
        {
            Process killFfmpeg = new Process();
            ProcessStartInfo taskkillStartInfo = new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = "/F /IM cmd.exe",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            killFfmpeg.StartInfo = taskkillStartInfo;
            killFfmpeg.Start();

        }

    }
}
