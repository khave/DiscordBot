using System;
using System.Collections.Generic;
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

namespace Bot.Audio
{
    class AudioManager
    {

        MyBot myBot;
        public static IAudioClient _vClient;
        private static bool playingSong = false;
        public static DiscordClient _client;
        private bool forceStop = false;

        public AudioManager(MyBot myBot)
        {
            this.myBot = myBot;
            _client = myBot.discord;
            myBot.discord.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });
        }

        public async void play(CommandEventArgs e, User user, string url)
        {
            if (user.VoiceChannel == null)
            {
                e.Channel.SendMessage("ERROR: You're not in a channel");
                return;
            }
            _vClient = await _client.GetService<AudioService>().Join(user.VoiceChannel);

            if (playingSong)
            {
                stop();
            }

            playFile(e, user, url);
        }

        public async void joinVoiceChannel(CommandEventArgs e, User user)
        {
            if (user.VoiceChannel == null)
            {
                e.Channel.SendMessage("ERROR: You're not in a channel");
                return;
            }

            _vClient = await _client.GetService<AudioService>().Join(user.VoiceChannel);
        }

        public async void leaveVoiceChannel()
        {
            await _vClient.Disconnect();
        }

        public async void playFile(CommandEventArgs e, User user, string url)
        {
            using (var cli = Client.For(new YouTube()))
            {
                var videos = cli.GetAllVideos(url);
                var video = videos.FirstOrDefault(v => v.Resolution == 240);
                Console.WriteLine("Found video: " + video.Title);
                if(File.Exists(@".\music\" + video.Title.Replace(" ", "_") + ".mp3"))
                {
                    await e.Channel.SendMessage("File already exists! Playing " + video.Title);
                    SendOnlineAudio(@".\music\" + video.Title.Replace(" ", "_") + ".mp3");
                    return;
                }
                var msg = e.Channel.SendMessage("Download progressing...");
                File.WriteAllBytes(@".\music\" + video.Title.Replace(" ", "_") + ".mp3", video.GetBytes());

                await e.Channel.SendMessage("Done! Playing " + video.Title);
                SendOnlineAudio(@".\music\" + video.Title.Replace(" ", "_") + ".mp3");
            }

            /*
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);

            VideoInfo video = videoInfos
    .Where(info => info.VideoType == VideoType.Mp4 && info.Resolution == 0)
    .OrderByDescending(info => info.AudioBitrate).First();

            string[] fileEntries = Directory.GetFiles(@".\music");
            foreach (string fileName in fileEntries)
            {
                //File already exists
                Console.WriteLine("Edit distance: " + Utils.Compute(Path.GetFileNameWithoutExtension(fileName), video.Title));
                if (Utils.Compute(Path.GetFileNameWithoutExtension(fileName), video.Title) < 10)
                {
                    await e.Channel.SendMessage("File already exists! Playing audio file...");
                    SendOnlineAudio(@".\music\" + Path.GetFileName(fileName));
                    return;
                }
            }

            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            var audioDownloader = new VideoDownloader(video, @".\music\" + video.Title.Replace(" ", "_") + ".mp3");

            var msg = e.Channel.SendMessage("Download progressing...");

            audioDownloader.DownloadFinished += async (sender, args) =>
            {
                await e.Channel.SendMessage("Done! Playing audio file...");
                SendOnlineAudio(@".\music\" + video.Title.Replace(" ", "_") + ".mp3");
            };
            
            audioDownloader.Execute();
             */
        }

        public void pause()
        {
            playingSong = !playingSong;
        }

        public async Task stop()
        {
            playingSong = false;
            forceStop = true;
            KillAllFFMPEG();
            Console.WriteLine("Stopped music bot");
        }

        public void SendOnlineAudio(string pathOrUrl)
        {
            var process = Process.Start(new ProcessStartInfo
            { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
                FileName = @"C:\Users\Kristian\Source\Repos\DiscordBot\ffmpeg.exe",
                Arguments = $"-i " + pathOrUrl + " " + // Here we provide a list of arguments to feed into FFmpeg. -i means the location of the file/URL it will read from
                            "-f s16le -ar 48000 -ac 2 pipe:1", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout.
                UseShellExecute = false,
                RedirectStandardOutput = true // Capture the stdout of the process
            });
            Thread.Sleep(2000); // Sleep for a few seconds to FFmpeg can start processing data.

            int blockSize = 3840; // The size of bytes to read per frame; 1920 for mono
            byte[] buffer = new byte[blockSize];
            int byteCount;
            pause();

            while (true && !forceStop) // Loop forever, so data will always be read
            {
                if (!playingSong) continue;

                byteCount = process.StandardOutput.BaseStream // Access the underlying MemoryStream from the stdout of FFmpeg
                        .Read(buffer, 0, blockSize); // Read stdout into the buffer

                if (byteCount == 0) // FFmpeg did not output anything
                    break; // Break out of the while(true) loop, since there was nothing to read.

                _vClient.Send(buffer, 0, byteCount); // Send our data to Discord
            }
            _vClient.Wait(); // Wait for the Voice Client to finish sending data, as ffMPEG may have already finished buffering out a song, and it is unsafe to return now.
            process.Close();
            forceStop = false;
        }

        public void SendAudioWithNAudio(string filePath)
        {
            var channelCount = _client.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
            var OutFormat = new WaveFormat(48000, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
            using (var MP3Reader = new Mp3FileReader(filePath)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
            using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
            {
                resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                byte[] buffer = new byte[blockSize];
                int byteCount;

                playingSong = true;

                while (playingSong == true && (byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                {
                    if (byteCount < blockSize)
                    {
                        // Incomplete Frame
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    _vClient.Send(buffer, 0, blockSize); // Send the buffer to Discord
                }
            }

        }


        public static async Task SendAudio(string filepath, Channel voiceChannel)
        {
            // When we use the !play command, it'll start this method

            // The comment below is how you'd find the first voice channel on the server "Somewhere"
            //var voiceChannel = _client.FindServers("Somewhere").FirstOrDefault().VoiceChannels.FirstOrDefault();
            // Since we already know the voice channel, we don't need that.
            // So... join the voice channel:
            _vClient = await _client.GetService<AudioService>().Join(voiceChannel);

            // Simple try and catch.
            try
            {

                var channelCount = _client.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
                var OutFormat = new WaveFormat(48000, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.

                using (var MP3Reader = new Mp3FileReader(filepath)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
                using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
                {
                    resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                    int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                    byte[] buffer = new byte[blockSize];
                    int byteCount;
                    // Add in the "&& playingSong" so that it only plays while true. For our cheesy skip command.
                    // AGAIN
                    // WARNING
                    // YOU NEED
                    // vvvvvvvvvvvvvvv
                    // opus.dll
                    // libsodium.dll
                    // ^^^^^^^^^^^^^^^
                    // If you do not have these, this will not work.
                    playingSong = true;

                    while (playingSong == true && (byteCount = resampler.Read(buffer, 0, blockSize)) > 0 && playingSong) // Read audio into our buffer, and keep a loop open while data is present
                    {
                        if (byteCount < blockSize)
                        {
                            // Incomplete Frame
                            for (int i = byteCount; i < blockSize; i++)
                                buffer[i] = 0;
                        }

                        _vClient.Send(buffer, 0, blockSize); // Send the buffer to Discord
                    }
                    await _vClient.Disconnect();
                }
            }
            catch
            {
                System.Console.WriteLine("Something went wrong. :(");
            }
            await _vClient.Disconnect();
        }

        private void KillAllFFMPEG()
        {
            Process killFfmpeg = new Process();
            ProcessStartInfo taskkillStartInfo = new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = "/F /IM ffmpeg.exe",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            killFfmpeg.StartInfo = taskkillStartInfo;
            killFfmpeg.Start();
        }

    }
}
