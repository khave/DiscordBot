using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using NAudio.Wave;
using System.Diagnostics;
using System.Threading;

namespace Bot.Audio
{
    class AudioManager
    {

        MyBot myBot;
        public static IAudioClient _vClient;
        private static bool playingSong = false;
        public static DiscordClient _client;

        public AudioManager(MyBot myBot)
        {
            this.myBot = myBot;
            _client = myBot.discord;
            myBot.discord.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });
        }

        public async void joinVoiceChannel(CommandEventArgs e, User user)
        {
            if (user.VoiceChannel == null)
            {
                e.Channel.SendMessage("ERROR: You're not in a channel");
                return;
            }

            _vClient = await _client.GetService<AudioService>().Join(user.VoiceChannel);
            SendOnlineAudio(@"C:\Users\Kristian\Source\Repos\DiscordBot\airHorn.mp4");
            await _vClient.Disconnect();
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

            while (true) // Loop forever, so data will always be read
            {
                byteCount = process.StandardOutput.BaseStream // Access the underlying MemoryStream from the stdout of FFmpeg
                        .Read(buffer, 0, blockSize); // Read stdout into the buffer

                if (byteCount == 0) // FFmpeg did not output anything
                    break; // Break out of the while(true) loop, since there was nothing to read.

                _vClient.Send(buffer, 0, byteCount); // Send our data to Discord
            }
            _vClient.Wait(); // Wait for the Voice Client to finish sending data, as ffMPEG may have already finished buffering out a song, and it is unsafe to return now.
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
                    while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0 && playingSong) // Read audio into our buffer, and keep a loop open while data is present
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
    }
}
