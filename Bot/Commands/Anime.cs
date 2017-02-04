using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Xml;
using System.Net;
using System.Net.Http;
using System.IO;
using Bot.NadekoBot;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace Bot.Commands
{
    class Anime : BotCommand
    {

        string[] tags;

        public Anime() : base("anime", "Find a random anime picture!", "!anime <OPTIONAL tags>")
        {
            //TODO: More tags
            tags = new string[] { "black eyes", "black hair", "blue eyes", "blush", "brown eyes", "brown hair",
            "circlet", "fate/grand order", "harukawa maki", "hnk", "ikari mio", "jeanne alter", "long hair",
            "yamika", "white hair", "uta no prince-sama", "twintails", ""};
        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            string tag;
            if (args.Length < 1)
            {
                tag = tags[Utils.getRandInt(0, tags.Length - 1)];
            }
            else {
                tag = String.Join("_", args);
            }

           
            var url = await searchAnime(tag).ConfigureAwait(false);
            await e.Channel.SendMessage("Finding anime image...");

            if (url == null)
                await e.Channel.SendMessage("No results");
            else
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(url, @".\anime.png");
                        await e.Channel.SendFile(@".\anime.png");
                        if (File.Exists(@".\anime.png"))
                        {
                            File.Delete(@".\anime.png");
                        }
                    }
                    catch (Exception ex)
                    {
                        await e.Channel.SendMessage("An error occured :(");
                    }
                }
            }
            
        }

   
        //Taken from NadekoBot
        //LINK: https://github.com/Kwoth/NadekoBot
        //Thanks to Kwoth
        //TODO: Add manga search

        public async Task<string> searchAnime(string tag)
        {
            tag = tag.Replace(" ", "_");
            string website = $"https://safebooru.org/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}";
            try {
                var toReturn = await Task.Run(async () =>
                {
                    using (var http = new HttpClient())
                    {
                        AddFakeHeaders(http);
                        var data = await http.GetStreamAsync(website).ConfigureAwait(false);
                        var doc = new XmlDocument();
                        doc.Load(data);

                        var node = doc.LastChild.ChildNodes[Utils.getRandInt(0, 100)];

                        var url = node.Attributes["file_url"].Value;
                        if (!url.StartsWith("http"))
                            url = "https:" + url;
                        return url;
                    }
                }).ConfigureAwait(false);
                return toReturn;
            } catch (Exception e) {
                return null;
                     }
        }

        private void AddFakeHeaders(HttpClient http)
        {
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1");
            http.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        }
    }
}
