using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Threading;
using Bot.NadekoBot;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Bot.Commands
{
    class Info : BotCommand
    {

        private Timer anilistTokenRefresher { get; }
        private string anilistToken { get; set; }

        public Info() : base("info", "Search for anime/manga information from AniList", "!info <anime/manga> <search>")
        {
            anilistTokenRefresher = new Timer(async (state) =>
            {
                try
                {
                    var headers = new Dictionary<string, string> {
                        {"grant_type", "client_credentials"},
                        {"client_id", "khave-kt7w9"},
                        {"client_secret", "ZaWLL2FGyUhr33gRYB0i"},
                    };

                    using (var http = new HttpClient())
                    {
                        Utils.AddFakeHeaders(http);
                        var formContent = new FormUrlEncodedContent(headers);
                        var response = await http.PostAsync("http://anilist.co/api/auth/access_token", formContent).ConfigureAwait(false);
                        var stringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        anilistToken = JObject.Parse(stringContent)["access_token"].ToString();
                        Console.WriteLine("Successfully renewed anilist credentials!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }, null, TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(29));
        }

        public override async void onCommand(CommandEventArgs e, DiscordClient discord, string[] args)
        {
            if (e.Channel.Name != "anime")
            {
                await e.Channel.SendMessage("That is an #anime command only!");
                return;
            }

            string type = args[0];

            string query = String.Join("", args.Skip(1).ToArray());
            showInfo(type, query, e);
        }

        public async void showInfo(string type, string query, CommandEventArgs e)
        {
            if (type == "anime")
            {
                if (string.IsNullOrWhiteSpace(query))
                    return;

                var animeData = await GetAnimeData(query);

                if (animeData == null)
                {
                    await e.Channel.SendMessage("Failed finding that animu.");
                    return;
                }

                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(animeData.image_url_lge, @".\animu.png");
                        await e.Channel.SendFile(@".\animu.png");
                        if (File.Exists(@".\animu.png"))
                        {
                            File.Delete(@".\animu.png");
                        }
                    }
                    catch (Exception ex)
                    {
                        await e.Channel.SendMessage("An error occured trying to load anime image");
                    }
                }

                await e.Channel.SendMessage(animeData.title_english +
                    "\n" + animeData.Synopsis.Replace("<br>", "\n") +
                    "\nTotal Episodes: " + animeData.total_episodes +
                    "\nStatus: " + animeData.airing_status +
                    "\nGenres: " + String.Join(", ", animeData.Genres) +
                    "\nScore: " + animeData.average_score +
                    "\n" + animeData.Link);
            }else if(type == "manga")
            {
                if (string.IsNullOrWhiteSpace(query))
                    return;

                var mangaData = await GetMangaData(query);

                if (mangaData == null)
                {
                    await e.Channel.SendMessage("Failed finding that manga.");
                    return;
                }

                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(mangaData.image_url_lge, @".\manga.png");
                        await e.Channel.SendFile(@".\manga.png");
                        if (File.Exists(@".\manga.png"))
                        {
                            File.Delete(@".\manga.png");
                        }
                    }
                    catch (Exception ex)
                    {
                        await e.Channel.SendMessage("An error occured trying to load manga image");
                    }
                }

                await e.Channel.SendMessage(mangaData.title_english +
                    "\n" + mangaData.Synopsis.Replace("<br>", "\n") +
                    "\nTotal Volumes/Chapters: " + mangaData.total_volumes + "/" + mangaData.total_chapters +
                    "\nStatus: " + mangaData.publishing_status+
                    "\nGenres: " + String.Join(", ", mangaData.Genres) +
                    "\nScore: " + mangaData.average_score +
                    "\n" + mangaData.Link);
            }
            else
            {
                await e.Channel.SendMessage("No such type! Types are anime/manga");
            }
        }

        private async Task<AnimeResult> GetAnimeData(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));
            try
            {

                var link = "http://anilist.co/api/anime/search/" + Uri.EscapeUriString(query);
                using (var http = new HttpClient())
                {
                    var res = await http.GetStringAsync("http://anilist.co/api/anime/search/" + Uri.EscapeUriString(query) + $"?access_token={anilistToken}");
                    var smallObj = JArray.Parse(res)[0];
                    var aniData = await http.GetStringAsync("http://anilist.co/api/anime/" + smallObj["id"] + $"?access_token={anilistToken}");

                    return await Task.Run(() => { try { return JsonConvert.DeserializeObject<AnimeResult>(aniData); } catch { return null; } });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed anime search for " + query);
                return null;
            }
        }

        private async Task<MangaResult> GetMangaData(string query)
            {
                if (string.IsNullOrWhiteSpace(query))
                    throw new ArgumentNullException(nameof(query));
                try
                {
                    using (var http = new HttpClient())
                    {
                        var res = await http.GetStringAsync("http://anilist.co/api/manga/search/" + Uri.EscapeUriString(query) + $"?access_token={anilistToken}");
                        var smallObj = JArray.Parse(res)[0];
                        var aniData = await http.GetStringAsync("http://anilist.co/api/manga/" + smallObj["id"] + $"?access_token={anilistToken}");

                        return await Task.Run(() => { try { return JsonConvert.DeserializeObject<MangaResult>(aniData); } catch { return null; } });
                    }
                }
                catch (Exception ex)
                {
                Console.WriteLine("Failed anime search for " + query);
                return null;
                }
            }
    }
}
