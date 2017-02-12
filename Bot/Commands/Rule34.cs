using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Net.Http;
using System.Xml;
using System.IO;
using System.Net;

namespace Bot.Commands
{
    class Rule34 : BotCommand
    {

        public Rule34() : base("rule34", "Search rule34 pictures", "!rule34 <search term>", new string[] { "34", "rule", "r34" })
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
           

            var url = await searchRule34(tag).ConfigureAwait(false);
            await e.Channel.SendMessage("Finding rule 34 image...");

            if (url == null)
                await e.Channel.SendMessage("No results");
            else
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(url, @".\rule34.png");
                        await e.Channel.SendFile(@".\rule34.png");
                        if (File.Exists(@".\rule34.png"))
                        {
                            File.Delete(@".\rule34.png");
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

        public async Task<string> searchRule34(string tag)
        {
            tag = tag.Replace(" ", "_");
            string website = $"https://rule34.xxx/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}";
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
