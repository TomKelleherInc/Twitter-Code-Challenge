using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TCC.Data.Analysis
{
    public class Stats : INotifyPropertyChanged
    {

        private DateTime startTime = DateTime.Now;

        public Stats()
        {
            this.startTime = DateTime.Now;
            TCC.Data.Storage.Gatherer.ItemAdded += Gatherer_ItemAdded;
        }

        public void Start(DateTime startTime)
        {
            this.startTime = startTime;
            TCC.Data.Storage.Gatherer.ItemAdded += Gatherer_ItemAdded;
        }

        public void Halt()
        {
            try
            {
                TCC.Data.Storage.Gatherer.ItemAdded -= Gatherer_ItemAdded;
            }
            catch (Exception ex)
            {
                // Just in case folks halt before starting.
                //throw;
            }
        }

        private void Gatherer_ItemAdded(object sender, Storage.Gatherer.ItemAddedArgs e)
        {
            // UpdateStats();
        }

        public void UpdateStats()
        {

            this.TotalTweetsReceived = TCC.Data.Storage.Gatherer.Count;
            var secondsPassed = DateTime.Now.Subtract(startTime).TotalSeconds;
            if (secondsPassed > 0)
            {
                decimal avg = (decimal)(TotalTweetsReceived / secondsPassed);
                this.AveragePerSecond = avg;

                // Choosing to project the approximate values for these, 
                // so we don't have to run it for a whole hour to see results.
                this.AveragePerMinute = avg * 60;
                this.AveragePerHour = avg * 3600;
            }

            if(this.TotalTweetsReceived > 0)
            {
                // Percent with emoji
                var tweetData = TCC.Data.Storage.Gatherer.GetData(); // gets a copy of the data

                // Percent with emoji
                int withEmoji = tweetData.Where(t => t.ContainsEmojis).Count();
                this.PercentWithEmoji = (decimal)withEmoji / this.TotalTweetsReceived;

                // Percent with URLs
                int withUrl = tweetData.Where(t => t.ContainsUrl).Count();
                this.PercentWithUrl = (decimal)withUrl / this.TotalTweetsReceived;

                // Percent with Photo Urls
                int withPhotoUrl = tweetData.Where(t => t.ContainsPhotoUrl).Count();
                this.PercentWithPhotoUrl = (decimal)withPhotoUrl / this.TotalTweetsReceived;

                var allEmoji = tweetData.Where(t => t.ContainsEmojis).SelectMany(t => t.Emojis).ToList();
                Dictionary<string, int> dctEmojiCounts = allEmoji.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
                this.TopEmoji = new Dictionary<string, int>(dctEmojiCounts.ToList().OrderByDescending(e => e.Value).Take(100));

                var allHashtags = tweetData.Where(t => t.ContainsHashtags).SelectMany(t => t.Hashtags).ToList();
                Dictionary<string, int> dctHashtagCounts = allHashtags.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
                this.TopHashtags = new Dictionary<string, int>(dctHashtagCounts.ToList().OrderByDescending(e => e.Value).Take(100));

                var allURLs = tweetData.Where(t => t.ContainsUrl).SelectMany(t => t.URLs).ToList();
                List<string> domains = new List<string>();
                foreach( string url in allURLs)
                {
                    Uri uri = new Uri(url);
                    domains.Add(uri.Host);
                }
                Dictionary<string, int> dctDomainCounts = domains.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
                this.TopUrlDomains = new Dictionary<string, int>(dctDomainCounts.ToList().OrderByDescending(e => e.Value).Take(100));
            }

        }


        public int TotalTweetsReceived { 
            get
            {
                return totalTweetsReceived;
            }
            private set
            {
                totalTweetsReceived = value;
                OnPropertyChanged();
            }
        }
        private int totalTweetsReceived = 0;


        public decimal AveragePerSecond
        {
            get
            {
                return averagePerSecond;
            }
            private set
            {
                averagePerSecond = value;
                OnPropertyChanged();
            }
        }
        private decimal averagePerSecond = 0;

        public decimal AveragePerMinute
        {
            get
            {
                return averagePerMinute;
            }
            private set
            {
                averagePerMinute = value;
                OnPropertyChanged();
            }
        }
        private decimal averagePerMinute = 0;


        public decimal AveragePerHour
        {
            get
            {
                return averagePerHour;
            }
            private set
            {
                averagePerHour = value;
                OnPropertyChanged();
            }
        }
        private decimal averagePerHour = 0;


        public decimal PercentWithEmoji
        {
            get
            {
                return percentWithEmoji;
            }

            set
            {
                percentWithEmoji = value;
                OnPropertyChanged();
            }
        }
        private decimal percentWithEmoji = 0;

        public decimal PercentWithUrl
        {
            get
            {
                return percentWithUrl;
            }

            set
            {
                percentWithUrl = value;
                OnPropertyChanged();
            }
        }
        private decimal percentWithUrl = 0;


        public decimal PercentWithPhotoUrl
        {
            get
            {
                return percentWithPhotoUrl;
            }

            set
            {
                percentWithPhotoUrl = value;
                OnPropertyChanged();
            }
        }
        private decimal percentWithPhotoUrl = 0;


        public Dictionary<string, int> TopEmoji
        {
            get
            {
                return topEmoji;
            }
            private set
            {
                topEmoji = value;
                OnPropertyChanged();
            }
        }
        private Dictionary<string, int> topEmoji = new Dictionary<string, int>();


        public Dictionary<string, int> TopHashtags
        {
            get
            {
                return topHashtags;
            }
            private set
            {
                topHashtags = value;
                OnPropertyChanged();
            }
        }
        private Dictionary<string, int> topHashtags = new Dictionary<string, int>();


        public Dictionary<string, int> TopUrlDomains
        {
            get
            {
                return topUrlDomains;
            }
            private set
            {
                topUrlDomains = value;
                OnPropertyChanged();
            }
        }
        private Dictionary<string, int> topUrlDomains = new Dictionary<string, int>();



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
