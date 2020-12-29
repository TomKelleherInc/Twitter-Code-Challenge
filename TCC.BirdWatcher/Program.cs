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
    class Program
    {

        static async System.Threading.Tasks.Task Main(string[] args)
        {

            await Watcher.Launch("hUIW5cbhwM8FRHeEqzAxgIBOG", "6bgNHjiFWV9QLctPoC8qUc0mzXLU9DP2PDQrML6oryzJPHElFR");

            Console.WriteLine("-------------------------");
            Console.WriteLine(" BirdWatcher is watching");
            Console.WriteLine("-------------------------");

            //var creds = new ConsumerOnlyCredentials("hUIW5cbhwM8FRHeEqzAxgIBOG", "6bgNHjiFWV9QLctPoC8qUc0mzXLU9DP2PDQrML6oryzJPHElFR");
            //var appClient = new TwitterClient(creds);

            //// Get OAuth Bearer token
            //var bearerToken = await appClient.Auth.CreateBearerTokenAsync();
            //creds = new ConsumerOnlyCredentials("hUIW5cbhwM8FRHeEqzAxgIBOG", "6bgNHjiFWV9QLctPoC8qUc0mzXLU9DP2PDQrML6oryzJPHElFR")
            //{
            //    BearerToken = bearerToken
            //};

            //appClient = new TwitterClient(creds);

        }
    }
}
