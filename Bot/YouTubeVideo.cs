using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot
{
    public class YouTubeVideo
    {
        public string id, title, description, duration, requester, url;
        public DateTime publishedDate;

        public YouTubeVideo(string url, string requester)
        {
            this.url = url;
            this.requester = requester;
            this.id = url.Substring(url.IndexOf("?v=") + 1);
            getVideoInfo(this);
        }

        public static YouTubeVideo fromString(string tmp)
        {
            string[] array = tmp.Split(':');
            return new YouTubeVideo("http://www.youtube.com/watch?v=" + array[0], array[1]);
        }

        public string toString()
        {
            return this.id + ":" + this.requester;
        }

        public static YouTubeService auth()
        {
            UserCredential creds;
            using (var stream = new FileStream(@".\client_secret.json", FileMode.Open, FileAccess.Read))
            {
                creds = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                new[] { YouTubeService.Scope.YoutubeReadonly },
                "user",
                CancellationToken.None,
                new FileDataStore("YoutubeAPI")
                ).Result;
            }

            var service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = creds,
                ApplicationName = "YoutubeAPI"
            });

            return service;
        }

        public static void getVideoInfo(YouTubeVideo video)
        {
            var videoRequest = auth().Videos.List("snippet");

            videoRequest.Id = video.id;

            var response = videoRequest.Execute();
            if (response.Items.Count > 0)
            {
                video.title = response.Items[0].Snippet.Title;
                video.description = response.Items[0].Snippet.Description;
                video.publishedDate = response.Items[0].Snippet.PublishedAt.Value;
            }
            else
            {
                Console.WriteLine("ERROR: Video not found!");
                //Video not found..
            }

            videoRequest = auth().Videos.List("contentDetails");

            videoRequest.Id = video.id;

            response = videoRequest.Execute();
            if (response.Items.Count > 0)
            {
                string tmp = response.Items[0].ContentDetails.Duration;
                TimeSpan t = TimeSpan.FromSeconds(System.Xml.XmlConvert.ToTimeSpan(tmp).TotalSeconds);
                string answer = string.Format("{1:D2}:{2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
                video.duration = answer;
            }
            else
            {
                Console.WriteLine("ERROR: Video duration not found!");
                //Video not found..
            }
        }
    }
}
