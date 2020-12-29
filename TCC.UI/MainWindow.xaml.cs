using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Configuration;
using System.Diagnostics;

namespace TCC.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static TCC.Data.Analysis.Stats stats = null;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();



        public MainWindow()
        {
            InitializeComponent();

            stats = new Data.Analysis.Stats();

            this.DataContext = stats;

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            stats.Start(DateTime.Now);

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            stats.UpdateStats();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string twitterConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"];
            string twitterConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"];
            //TCC.BirdWatcher.Watcher.Launch("hUIW5cbhwM8FRHeEqzAxgIBOG", "6bgNHjiFWV9QLctPoC8qUc0mzXLU9DP2PDQrML6oryzJPHElFR");

            // explicitly do NOT use "await" here -- we want it to keep running.
            TCC.BirdWatcher.Watcher.Launch(twitterConsumerKey, twitterConsumerSecret);

            // Fun test...let the Twitter capture run ahead and load up the queue, 
            // before we start pulling them into storage.
            System.Threading.Thread.Sleep(3000);

            // Now start pulling data from queue.
            var storage = new TCC.Data.Storage.Gatherer();
            storage.Start();


            // Halting switches -- but didn't wire these into the UI.
            //await TCC.BirdWatcher.Watcher.Halt();
            //storage.Halt();

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
