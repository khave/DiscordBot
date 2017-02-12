using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Xml;

namespace Bot.Commands
{
    class Yandere : BotCommand
    {

        public Yandere() : base("yandere", "Search yandere pictures", "!yandere <search term>")
        {

        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            if (e.Channel.Name != "nsfw")
            {
                await e.Channel.SendMessage("That is a NSFW command only!");
                return;
            }

            string tag = String.Join("_", args);


            var url = await searchYandere(tag).ConfigureAwait(false);
            await e.Channel.SendMessage("Finding yandere image...");

            if (url == null)
                await e.Channel.SendMessage("No results");
            else
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(url, @".\yandere.png");
                        await e.Channel.SendFile(@".\yandere.png");
                        if (File.Exists(@".\yandere.png"))
                        {
                            File.Delete(@".\yandere.png");
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

        public async Task<string> searchYandere(string tag)
        {
            tag = tag.Replace(" ", "_");
            string website = $"https://yande.re/post.xml?limit=100&tags={tag}";
            try
            {
                var toReturn = await Task.Run(async () =>
                {
                    using (var http = new HttpClient())
                    {
                        Utils.AddFakeHeaders(http);
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
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
