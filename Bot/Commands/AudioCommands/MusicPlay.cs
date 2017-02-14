using Discord.Commands;
using System;
using System.Net;
using System.Text.RegularExpressions;
using YoutubeSearch;
using Discord;
using YoutubeExtractor;
using System.Collections.Generic;
using RestSharp.Extensions.MonoHttp;
using System.Linq;

namespace Bot.Commands.AudioCommands
{
    class MusicPlay : BotCommand
    {

        MyBot myBot;
        WebClient webClient;
        string title;

        public MusicPlay(MyBot myBot) : base("music play", "Play music", "music play <url>", new string[] { "play" })
        {
            this.myBot = myBot;
            this.webClient = new WebClient();
        }

        public string getVideoUrl(string searchTerm)
        {
            string html = this.webClient.DownloadString("https://www.youtube.com/results?search_query=" + searchTerm + "&page=1");
            string pattern = "<div class=\"yt-lockup-content\">.*?title=\"(?<NAME>.*?)\".*?</div></div></div></li>";
            MatchCollection result = Regex.Matches(html, pattern, RegexOptions.Singleline);
            title = result[0].Groups[1].Value;
            string url = string.Concat("http://www.youtube.com/watch?v=", VideoItemHelper.cull(result[0].Value, "watch?v=", "\""));
            return url;
        }

        public bool isYoutubeUrl(string url)
        {
            if (url.Contains("www.youtube"))
            {
                return true;
            }
            return false;
        }

        public string correctUrl(string url)
        {
            return url.Remove(4);
        }

        public static string GetTitle(string url)
        {
            var api = $"http://youtube.com/get_video_info?video_id={GetArgs(url, "v", '?')}";
            return GetArgs(new WebClient().DownloadString(api), "title", '&');
        }

        private static string GetArgs(string args, string key, char query)
        {
            var iqs = args.IndexOf(query);
            return iqs == -1
                ? string.Empty
                : HttpUtility.ParseQueryString(iqs < args.Length - 1
                    ? args.Substring(iqs + 1) : string.Empty)[key];
        }

        public override void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            if (!myBot.audioManager.isMusicChannel(e)) return;

            if (args.Length < 1)
            {
                string video = myBot.audioManager.queue.First().url;
                myBot.audioManager.queue.Dequeue();
                myBot.audioManager.SendOnlineAudio(e, video);
                return;
            }

            string url = String.Join(" ", args);            

            if (!isYoutubeUrl(url))
            {
                //e.Channel.SendMessage("Searching for first video...");
                string videoUrl = getVideoUrl(url);
                myBot.audioManager.SendOnlineAudio(e, videoUrl);
            }
            else
            {
                myBot.audioManager.SendOnlineAudio(e, url);
            }


            //myBot.audioManager.play(e, e.User, videoUrl);
        }
    }
}
