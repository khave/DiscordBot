using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System.IO;
using System.Threading;

namespace Bot
{
    class YoutubeAPI
    {

        private static YouTubeService ytService = auth();

        public static YouTubeService auth()
        {
            UserCredential creds;
            using (var stream = new FileStream(@".\youtube_client_secret.json", FileMode.Open, FileAccess.Read))
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
                video.duration = "" + response.Items[0].FileDetails.DurationMs*1000;
                video.publishedDate = response.Items[0].Snippet.PublishedAt.Value;
            }
            else
            {
                //Video not found..
            }
        }



    }
    }
