using System.Net;
using System.Text.RegularExpressions;

namespace Bot
{
    class BotMusic
    {

        WebClient webClient;
        string title { get; set; }
        string duration { get; set; }
        string url { get; set; }
        string requester { get; set; }

        public BotMusic()
        {
            this.webClient = new WebClient();
        }

        public BotMusic(string title, string duration, string url, string requester)
        {
            this.webClient = new WebClient();
            this.title = title;
            this.duration = duration;
            this.url = url;
            this.requester = requester;
        }

        public BotMusic(string url)
        {
            this.webClient = new WebClient();

        }

        public string getVideoInformation(string url)
        {
            string html = this.webClient.DownloadString(url + "&page=1");
            string pattern = "<div class=\"yt-lockup-content\">.*?title=\"(?<NAME>.*?)\".*?</div></div></div></li>";
            MatchCollection result = Regex.Matches(html, pattern, RegexOptions.Singleline);
            return result[0].Groups[1].Value;
        }
    }
}
