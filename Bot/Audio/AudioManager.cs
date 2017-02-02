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

        public async void play(CommandEventArgs e, User user, string url)
        {
            if (user.VoiceChannel == null)
            {
                await e.Channel.SendMessage("ERROR: You're not in a channel");
                return;
            }
            _vClient = await _client.GetService<AudioService>().Join(user.VoiceChannel);

            if (playingSong)
            {
                await stop();
            }

            playFile(e, user, url);
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
        

        public async void playFile(CommandEventArgs e, User user, string url)
        {
            using (var cli = Client.For(new YouTube()))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var videos = cli.GetAllVideos(url);

                foreach (int resolution in resolutions)
                {

                    var video = videos.FirstOrDefault(v => v.Resolution == resolution);

                    //If you can't find a video for the specific resolution, try next resolution
                    if (video == null) continue;

                    Console.WriteLine("Found video: " + video.Title + " with resolution: " + resolution);
                    if (File.Exists(@".\music\" + video.Title.Replace(" ", "_") + ".mp3"))
                    {
                        await e.Channel.SendMessage("File already exists! Playing " + video.Title);
                        //SendOnlineAudio(@".\music\" + video.Title.Replace(" ", "_") + ".mp3");
                        return;
                    }
                    var msg = e.Channel.SendMessage("Download progressing...");
                    File.WriteAllBytes(@".\music\" + video.Title.Replace(" ", "_") + ".mp3", video.GetBytes());

                    //Found video, break loop and play
                    watch.Stop();
                    await e.Channel.SendMessage("Done! Playing " + video.Title + "\nIt took " + watch.Elapsed.TotalSeconds + " seconds");
                    //SendOnlineAudio(@".\music\" + video.Title.Replace(" ", "_") + ".mp3");
                    break;
                }
            }
        }

        public async Task stop()
        {
            playingSong = false;
            Console.WriteLine("Stopping music bot...");
        }

        public async void skip(CommandEventArgs e)
        {
            if (queue.Count != 0)
            {
                await stop();
                //Sleep for a second to stop any audio still playing
                Thread.Sleep(1000);
                string video = queue.ElementAt(0);
                queue.RemoveAt(0);
                Thread.Sleep(1000);
                await e.Channel.SendMessage("Playing next song in queue...");
                SendOnlineAudio(e, video);
            }
            else
            {
                await e.Channel.SendMessage("No more songs in queue!");
                await stop();
            }
        }

        public async void SendOnlineAudio(CommandEventArgs e, string pathOrUrl)
        {
            await joinVoiceChannel(e);
            Console.WriteLine("Joined voice");

            if(_vClient == null)
            {
                await e.Channel.SendMessage("You are not in a voice channel!");
                return;
            }

            if (playingSong)
            {
                queue.Add(pathOrUrl);
                await e.Channel.SendMessage("Added song to the queue! **[" + queue.IndexOf(pathOrUrl) + "]**");
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

            //Thread.Sleep(2000); // Sleep for a few seconds to FFmpeg can start processing data.

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

                _vClient.Send(buffer, 0, byteCount); // Send our data to Discord
            }
            Console.WriteLine("Stopped sending audio");

            _vClient.Wait(); // Wait for the Voice Client to finish sending data, as ffMPEG may have already finished buffering out a song, and it is unsafe to return now.
            _vClient.Clear();
            if (!process.HasExited)
            {
                process.Kill();
                process.Dispose();
            }
            playingSong = false;
            Console.WriteLine("Reached end of function");
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
