using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Net;
using System.IO;

namespace Bot.Commands
{
    class GoogleSearch : BotCommand
    {

        public GoogleSearch() : base("google", "Search for a picture on google!", "!google <args>", new string[] { "search" })
        {

        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            if(e.Channel.Name != "nsfw")
            {
                await e.Channel.SendMessage("That is a NSFW command only!");
                return;
            }

            if(e.User.Name == "Sadenar")
            {
                await e.Channel.SendMessage("You must be atleast 15 or older to use this command! Kappa");
                return;
            }

            string topic = String.Join(" ", args);

            await e.Channel.SendMessage("Searching for image...");

            string html = GetHtmlCode(topic);
            List<string> urls = GetUrls(html);
            var rnd = new Random();
            int randomUrl;
            try {
                 randomUrl = rnd.Next(0, urls.Count - 1);
            }catch(ArgumentOutOfRangeException ex)
            {
                //Dunno why this happens.
                await e.Channel.SendMessage("An error occured :(");
                return;
            }

            string url = urls[randomUrl];
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, @".\google.png");
                    await e.Channel.SendFile(@".\google.png");
                    if (File.Exists(@".\google.png"))
                    {
                        File.Delete(@".\google.png");
                    }
                }
                catch (Exception ex)
                {
                    await e.Channel.SendMessage("An error occured :(");
                }
            }
        }

        private string GetHtmlCode(string topic)
        {
            string url = "https://www.google.com/search?q=" + topic + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }

        private List<string> GetUrls(string html)
        {
            var urls = new List<string>();

            int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

            while (ndx >= 0)
            {
                ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                ndx++;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls.Add(url);
                ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
            }
            return urls;
        }



    }
}
