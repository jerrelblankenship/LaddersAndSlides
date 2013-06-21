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
    using LaddersAndSlides_GameEngine.Domain;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameSetup : Page
    {
        public int NumberOfPlayers { get; set; }
        public List<Player> PlayerList { get; set; } 

        public GameSetup()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NumberOfPlayers = 0;
            PlayerList = new List<Player>();
        }

        private void PlayerSelection_OnClick(object sender, RoutedEventArgs e)
        {
            var button = ((Button) e.OriginalSource);
            var buttonName = button.Name;
            button.IsEnabled = false;

            var image = new Image();
            image.Source = new BitmapImage { UriSource = new Uri("ms-appx:///Assets/Players/Green-Monster-Selected.png") };
            button.Content = image;

            switch (buttonName)
            {
                case "GreenMonster":
                    AddPlayer("ms-appx:///Assets/Players/Green-Monster.png");
                    GreenMonsterSelectedNotification.Text = "X";
                    break;
                case "BlueMonster":
                    AddPlayer("ms-appx:///Assets/Players/Blue-Monster.png");
                    BlueMonsterSelectedNotification.Text = "X";
                    break;
                case "OrangeMonster":
                    AddPlayer("ms-appx:///Assets/Players/Orange-Monster.png");
                    OrangeMonsterSelectedNotification.Text = "X";
                    break;
                case "PurpleMonster":
                    AddPlayer("ms-appx:///Assets/Players/Purple-Monster.png");
                    PurpleMonsterSelectedNotification.Text = "X";
                    break;
            }

            if (NumberOfPlayers < 4)
            {
                SelectPlayerNotification.Text = string.Format("Select Player {0}", NumberOfPlayers + 1);
            }
            else
            {
                SelectPlayerNotification.Text = "Press Start";
            }

            GameStart.IsEnabled = true;
        }

        private void AddPlayer(string playerTokenUrl)
        {
            NumberOfPlayers++;
            var player = new Player
            {
                Name = string.Format("Player {0}", NumberOfPlayers),
                ImageUri = new Uri(playerTokenUrl),
                TurnOrder = NumberOfPlayers
            };

            PlayerList.Add(player);
        }

        private void GameStart_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Game), PlayerList);
        }
    }
}
