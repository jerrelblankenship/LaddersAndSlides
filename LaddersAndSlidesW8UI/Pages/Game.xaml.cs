namespace LaddersAndSlidesW8UI.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LaddersAndSlides_GameEngine.Domain;
    using LaddersAndSlides_GameEngine.Engine;
    using Processing;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Game : Page
    {
        private GameEngine _gameEngine;
        public GameEngine GameEngine
        {
            get { return _gameEngine ?? (_gameEngine = new GameEngine()); }
            set { _gameEngine = value; }
        }

        private DispatcherTimer _timer;
        public DispatcherTimer Timer
        {
            get { return _timer ?? (_timer = new DispatcherTimer {Interval = new TimeSpan(1000)}); }
            set { _timer = value; }
        }

        public Game()
        {
            InitializeComponent();
            
            Timer.Tick += TimerTick;
            Loaded += GameLoaded;
        }

        void GameLoaded(object sender, RoutedEventArgs e)
        {
            var previousPlayerTokenCombinedHeight = 0.0;
            
            foreach (var image in GameEngine.Players.Select(player => GameEngine.CreatePlayerToken(player)))
            {
                _gutter.Children.Add(image);

                previousPlayerTokenCombinedHeight += image.ActualHeight;
                Canvas.SetTop(image, _gutter.ActualHeight - (previousPlayerTokenCombinedHeight  + 1));
            }

            GameEngine.GetNextPlayer();
            _playerNotificationDisplayText.Text = GameEngine.CurrentPlayer.Name;
            _playerNotificationDisplayImage.Source = new BitmapImage { UriSource = GameEngine.CurrentPlayer.ImageUri };
            GameEngine.CalculateTileHeightWidth(_gameBoard);
            RenderSpinner(_gameSpinner, _arrow);
        }

        void TimerTick(object sender, object e)
        {
            var pToken = GetCurrentPlayerToken();

            switch (GameEngine.CurrentState)
            {
                case GameStateEngine.ArrowEvent:
                    GameEngine.ProcessArrowEvent(_arrow);
                    break;

                case GameStateEngine.ArrowDelayedEvent:
                    GameEngine.ProcessArrowDelayEvent();
                    break;

                case GameStateEngine.PlayerEvent:
                    GameEngine.ProcessPlayerEvent(pToken, _gutter, _gameBoard);
                    break;

                case GameStateEngine.GetNextPlayer:
                    GameEngine.GetNextPlayer();
                    _playerNotificationDisplayText.Text = GameEngine.CurrentPlayer.Name;
                    _playerNotificationDisplayImage.Source = new BitmapImage { UriSource = GameEngine.CurrentPlayer.ImageUri };
                    break;

                //case GameStateEngine.PlayerSpecialMoveTransportCalculateEvent:
                //    GameEngine.CalculateSpecialMove(pToken, _gameBoard);
                //    break;

                case GameStateEngine.PlayerSpecialMoveTransportMoveEvent:
                    GameEngine.MakeSpecialMove(pToken, _gameBoard);
                    break;
                case GameStateEngine.TurnComplete:
                    break;
            }
        }

        internal Image GetCurrentPlayerToken()
        {
            return (Image) _gutter.Children.FirstOrDefault(x => ((Image) x).Name == GameEngine.CurrentPlayer.Name) ??
                   (Image)_gameBoard.Children.FirstOrDefault(x => ((Image)x).Name == GameEngine.CurrentPlayer.Name);
        }

        public void RenderSpinner(Canvas gameSpinner, Image arrow)
        {
            Canvas.SetTop(arrow, (gameSpinner.ActualHeight / 2) - (arrow.ActualHeight / 2) + (arrow.ActualHeight * .07));
            Canvas.SetLeft(arrow, ((gameSpinner.ActualWidth / 2) - (arrow.ActualWidth / 2)) + (arrow.ActualHeight * .015));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GameEngine.Players = e.Parameter as List<Player>;
        }

        private void GameSpinner_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (GameEngine.CurrentState != GameStateEngine.ArrowEvent)
            {
                GameEngine.CurrentState = GameStateEngine.ArrowEvent;
                GameEngine.CalculateArrowSpin();
                Timer.Start();
            }
        }
    }
}
