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
        }

        private void PlayerSelection_OnClick(object sender, RoutedEventArgs e)
        {
            var buttonName = ((Button) e.OriginalSource).Name;
            //var numberOfPlayers = 1;
            var playerList = new List<Player>();

            switch (buttonName)
            {
                case "GreenMonster":
                    var player1 = new Player
                        {
                            Name = "Player 1",
                            ImageUri = new Uri("ms-appx:///Assets/Players/Blue-Monster.png"),
                            TurnOrder = 1
                        };

                    //var player2 = new Player
                    //{
                    //    Name = "Player 2",
                    //    ImageUri = new Uri("ms-appx:///Assets/Players/Purple-Monster.png"),
                    //    TurnOrder = 2
                    //};

                    playerList.Add(player1);
                    //playerList.Add(player2);
                    break;
                //case "OrangeMonster":
                //    numberOfPlayers = 3;
                //    break;
                //case "PurpleMonster":
                //    numberOfPlayers = 4;
                //    break;
            }

            Frame.Navigate(typeof (Game), playerList);
        }
    }
}
