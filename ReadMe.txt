------------------------------------------------------
 ReadMe and Release Notes and Closing Thoughts...
------------------------------------------------------

TO RUN:  Set your "TwitterConsumerKey" and "TwitterConsumerSecret" keys in the app.config.


Project Roles:
- TCC.Birdwatcher: Uses the Tweetinvi Nuget package to connect to the sample stream. With each incoming tweet it
  creates a TweetSummary and passes it to the TCC.Queue system (the "database").
- TCC.Queue: Grabs the incoming Tweets into a queue, that the TCC.Data.Capture routine then harvests. This is
  done to avoid having locking conditions, or to allow for temporary periods when the incoming Twitter stream
  might outstrip the system's speed at digesting the data.
- TCC.Data.Capture: The "database" -- which pulls the tweets from the queue and stores them.
- TCC.Data.Analysis: Calculates the stats.  Provides a class that inherits INotifyPropertyChanged so that changes
  can be reflected in the WPF UI through binding.
- TCC.UI:  The WPF UI that displays the results.
- TCC.Common:  Common classes used by other projects.

Notes:
- Made the "BirdWatcher" a Singleton, so that if more than one was launched it wouldn't double up the records.
- Made the Data.Storage a Singleton so that we don't accidentally make two distinct storage locations.
- Rather than parse the emoji-json file, I used a complex Regular Expression based on the UNICODE EMOJI v13.1.
  (see https://stackoverflow.com/a/65429849/283160)

Known Issues:
- The connection to Twitter can occassionally fail.  To be more robust this ought to detect that and self-heal.
- Better would have to created interfaces for the Data.Storage class and Data.Capture class, so they can be
   more easily replaced with databases, MSMQ, etc.
- Used ConcurrentQueue to provide for thread-safe "simultaneous" adding and pulling to tweet queue, but have not
  explicitly tested that this code is truly thread-safe!  
- With V2 of the Twitter API it is possible to request only the desired fields, rather than the entire tweet.  I did
  not do this, but it would be preferable in production to minimize pressure on the Twitter API and local data processing.
- No Unit tests included.  Alas.  Time ran out.
- To ensure that this works well and the statistics calculations don't bog down with 100,000s of tweets, I should have left
  this running overnight.  Some of the LINQ techniques I used to calculate the "Top" stats (emoji, hashtags) might need
  improvement with large datasets.
