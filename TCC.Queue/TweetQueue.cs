using System;
using System.Collections;
using System.Collections.Concurrent;
using TCC.Common;

namespace TCC.Queue
{

    /// <summary>
    /// The Queue that will receive the incoming Tweets and hold them until the 
    /// "gathering" process collects them and puts them in the database.
    /// This is designed to be replaceable with MSMQ or another queueing mechanism.
    /// </summary>
    public static class TweetQueue
    {        
        // Using the ConcurrentQueue because it's threadsafe.
        private static ConcurrentQueue<TweetSummary> tweetQueue = new ConcurrentQueue<TweetSummary>();

        /// <summary>
        /// Adds a new TweetSummary to the queue.
        /// </summary>
        /// <param name="tweetSummary"></param>
        public static void Add(TweetSummary tweetSummary)
        {
            tweetQueue.Enqueue(tweetSummary);
            ItemAdded?.Invoke(null, new ItemAddedArgs(tweetSummary));
        }


        /// <summary>
        /// Gets the next TweetSummary from the queue.
        /// </summary>
        /// <returns></returns>
        public static TweetSummary GetNext()
        {
            TweetSummary ts = null;
            tweetQueue.TryDequeue(out ts);
            return ts;
        }


        /// <summary>
        /// Clears the queue of all TweetSummaries.
        /// </summary>
        public static void Clear()
        {
            tweetQueue.Clear();
        }


        /// <summary>
        /// Returns the current count in the queue.  We can monitor this to be sure our 
        /// data gathering routine isn't being outstripped by the rate of incoming tweets.
        /// </summary>
        public static int Count
        {
            get
            {
                return tweetQueue.Count;
            }
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
