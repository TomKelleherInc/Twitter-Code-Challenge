using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TCC.Common;

namespace TCC.Data.Storage
{

    /// <summary>
    /// Gathers and stores the tweet information.
    /// Could be replaced with a database or other storage.
    /// </summary>
    public class Gatherer
    {

        /// <summary>
        /// Using ConcurrentBag as a thread-safe static collection, so that all instances share the same 
        /// internal collection.
        /// </summary>
        private static ConcurrentBag<TweetSummary> allTweets = new ConcurrentBag<TweetSummary>();


        /// <summary>
        /// Starts this Gatherer by making it listen to the queue's ItemAdded event.
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            Console.WriteLine("--Gatherer getting any already-enqueued items--");
            ProcessEnqueuedItems();
            Console.WriteLine($"In Storage: {Count}");
            Console.WriteLine("--Gatherer getting any already-enqueued items--");
            TCC.Queue.TweetQueue.ItemAdded += TweetQueue_ItemAdded;
        }


        /// <summary>
        /// Listens for the queue's ItemAdded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TweetQueue_ItemAdded(object sender, Queue.TweetQueue.ItemAddedArgs e)
        {
            var summary = TCC.Queue.TweetQueue.GetNext();
            if (summary != null)
            {
                allTweets.Add(summary);
                Console.WriteLine($"In Storage: {Count}");
            }
        }

        private static void ProcessEnqueuedItems()
        {            
            while (true)
            {
                var summary = TCC.Queue.TweetQueue.GetNext();
                if (summary == null) break;

                allTweets.Add(summary);
                ItemAdded?.Invoke(null, new ItemAddedArgs(summary));
                Console.WriteLine($"In Storage: {Count}");
            }
        }


        /// <summary>
        /// Returns a copy of the data, rather than a reference to the same list.
        /// </summary>
        /// <returns></returns>
        public static List<TweetSummary> GetData()
        {
            TweetSummary[] copy = new TweetSummary[allTweets.Count];
            allTweets.CopyTo(copy, 0); ;
            return new List<TweetSummary>(copy);
        }


        /// <summary>
        /// Returns the count of TweetSummaries currently in storage.
        /// </summary>
        public static int Count
        {
            get
            { 
                return allTweets.Count;
            }
        }


        /// <summary>
        /// Halts the Gatherer by stopping it from listening to the queue's events.
        /// </summary>
        /// <returns></returns>
        public void Halt()
        {
            System.Console.WriteLine("--HALTING GATHERER--");
            TCC.Queue.TweetQueue.ItemAdded -= TweetQueue_ItemAdded;
        }


        #region "EVENTS"

        public static event EventHandler<ItemAddedArgs> ItemAdded;

        public class ItemAddedArgs : EventArgs
        {
            public TweetSummary ItemAdded { get; }

            public ItemAddedArgs(TweetSummary tweetSummary)
            {
                ItemAdded = tweetSummary;
            }
        }

        #endregion

    }
}
