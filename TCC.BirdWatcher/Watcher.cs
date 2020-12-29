using System;
using Tweetinvi;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Tweetinvi.Models;
using TCC.Queue;

namespace TCC.BirdWatcher
{
    public static class Watcher
    {
        private static TwitterClient appClient;
        private static bool cancelled = false;

        public static async Task Launch(string TwitterConsumerKey, string TwitterConsumerSecret)
        {
            var creds = new ConsumerOnlyCredentials(TwitterConsumerKey, TwitterConsumerSecret);
            var tempClient = new TwitterClient(creds);

            // Get OAuth Bearer token
            var bearerToken = await tempClient.Auth.CreateBearerTokenAsync();
            creds = new ConsumerOnlyCredentials(TwitterConsumerKey, TwitterConsumerSecret)
            {
                BearerToken = bearerToken
            };

            appClient = new TwitterClient(creds);

            var tweetStream = appClient.StreamsV2.CreateSampleStream();
            await StartListening(tweetStream);
        }

        public async static Task Halt()
        {
            System.Console.WriteLine("--HALTING WATCHER--");
            cancelled = true;
        }


        private static int tweetsReceived = 0;

        private async static Task StartListening(Tweetinvi.Streaming.V2.ISampleStreamV2 tweetStream)
        {
            tweetStream.TweetReceived += (sender, args) =>
            {
                try
                {
                    List<string> urls = new List<string>();
                    List<string> hashtags = new List<string>();
                    urls = args.Tweet.Entities?.Urls?.Select(u => u.UnwoundUrl).ToList();
                    hashtags = args.Tweet.Entities?.Hashtags?.Select(h => h.Tag).ToList();

                    TCC.Common.TweetSummary summary = new Common.TweetSummary(args.Tweet.Text, urls, hashtags);
                    TweetQueue.Add(summary);
                    tweetsReceived++;
                    System.Console.WriteLine($"Tweets Received: {tweetsReceived}: Contains emojis? {summary.ContainsEmojis}");

                    if (cancelled)
                    {
                        System.Console.WriteLine("--HALTING WATCHER--");
                        tweetStream.StopStream();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Exception: {ex.Message}");

                    //throw;
                }
            };

            await tweetStream.StartAsync();
        }


    }
}
