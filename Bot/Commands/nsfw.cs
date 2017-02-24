using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Net.Http;
using System.Xml;
using System.IO;
using System.Net;

namespace Bot.Commands
{
    class nsfw : BotCommand
    {

        string[] tags;

        public nsfw() : base("nsfw", "Find a nsfw picture!", "!nsfw <OPTIONAL tags>")
        {
            //TODO: More tags
            tags = new string[] { "black eyes", "black hair", "blue eyes", "blush", "brown eyes", "brown hair",
            "circlet", "fate/grand order", "harukawa maki", "hnk", "ikari mio", "jeanne alter", "long hair",
            "yamika", "white hair", "uta no prince-sama", "twintails", ""};
        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            if (e.Channel.Name != "nsfw")
            {
                await e.Channel.SendMessage("That is a NSFW command only!");
                return;
            }

            string tag;
            if (args[0] == "")
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
                        client.DownloadFile(url, @".\nsfw.png");
                        await e.Channel.SendFile(@".\nsfw.png");
                        if (File.Exists(@".\nsfw.png"))
                        {
                            File.Delete(@".\nsfw.png");
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

        public async Task<string> searchAnime(string tag)
        {
            tag = tag.Replace(" ", "_");
            string website = $"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}";
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
