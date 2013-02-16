using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LaddersAndSlidesW8UI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Game : Page
    {
        private List<Player> Players; 

        public Game()
        {
            this.InitializeComponent();
            Players = new List<Player>();
            this.Loaded += Game_Loaded;
        }

        void Game_Loaded(object sender, RoutedEventArgs e)
        {
            var previousPlayerTokenCombinedHeight = 0.0;
            foreach (var player in Players)
            {
                var image = new Image();
                image.Width = 65;
                image.Height = 65;
                var bitmapImage = new BitmapImage();
                bitmapImage.UriSource = player.ImageUri;
                image.Source = bitmapImage;
                Gutter.Children.Add(image);

                previousPlayerTokenCombinedHeight += image.ActualHeight;
                Canvas.SetTop(image, Gutter.ActualHeight - (previousPlayerTokenCombinedHeight  + 1));
            }
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Players = e.Parameter as List<Player>;
        }

        private void GameSpinner_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
