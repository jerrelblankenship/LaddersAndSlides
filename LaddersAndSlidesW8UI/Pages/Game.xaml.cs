using Windows.UI.Xaml.Media.Animation;

namespace LaddersAndSlidesW8UI.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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

        public Image CurrentPlayerToken;
        public Storyboard CurrentAnimation { get; set; }
        public bool AnimationInProgress = false;

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
            RenderSpinner(_gameSpinner, _arrow);
        }

        void TimerTick(object sender, object e)
        {
            CurrentPlayerToken = GetCurrentPlayerToken();

            switch (GameEngine.CurrentState)
            {
                case GameStateEngine.ArrowEvent:
                    GameEngine.ProcessArrowEvent(_arrow);
                    break;

                case GameStateEngine.ArrowDelayedEvent:
                    GameEngine.ProcessArrowDelayEvent();
                    break;

                case GameStateEngine.PlayerEvent:
                    GameEngine.ProcessPlayerEvent(CurrentPlayerToken, _gutter, _gameBoard);
                    break;

                case GameStateEngine.GetNextPlayer:
                    GameEngine.GetNextPlayer();
                    _playerNotificationDisplayText.Text = GameEngine.CurrentPlayer.Name;
                    _playerNotificationDisplayImage.Source = new BitmapImage { UriSource = GameEngine.CurrentPlayer.ImageUri };
                    break;

                case GameStateEngine.PlayerSpecialMoveTransportMoveEvent:
                    try
                    {
                        if (!AnimationInProgress)
                        {
                            SetSpecialMove();
                            if (CurrentAnimation != null)
                            {
                                AnimationInProgress = true;
                                CurrentAnimation.Begin();
                            }
                            else
                            {
                                GameEngine.CurrentState = GameStateEngine.GetNextPlayer;
                                GameEngine.CurrentPlayer.TurnInProcess = false;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        
                    }
                    
                    break;
                case GameStateEngine.TurnComplete:
                    break;
                case GameStateEngine.WinnerDeclared:
                    break;
            }
        }

        public void SetSpecialMove()
        {
            Storyboard tStory = null;
            switch (GameEngine.CurrentSpecialMove.StartingTileNumber)
            {
                case 1:
                    tStory = _ladderMove1_38;
                    break;
                case 4:
                    tStory = _ladderMove4_14;
                    break;
                case 9:
                    tStory = _ladderMove9_31;
                    break;
                case 16:
                    tStory = _slideMove16_2;
                    break;
                case 21:
                    tStory = _ladderMove21_42;
                    break;
                case 28:
                    tStory = _ladderMove28_84;
                    break;
                case 47:
                    tStory = _slideMove47_26;
                    break;
                case 49:
                    tStory = _slideMove49_11;
                    break;
                case 51:
                    tStory = _ladderMove51_67;
                    break;
                case 56:
                    tStory = _slideMove56_53;
                    break;
                case 64:
                    tStory = _slideMove64_60;
                    break;
                case 71:
                    tStory = _ladderMove71_91;
                    break;
                case 80:
                    tStory = _ladderMove80_100;
                    break;
                case 87:
                    tStory = _slideMove87_24;
                    break;
                case 93:
                    tStory = _slideMove93_73;
                    break;
                case 95:
                    tStory = _slideMove95_75;
                    break;
                case 98:
                    tStory = _slideMove98_78;
                    break;
            }

            if (tStory != null)
            {
                Storyboard.SetTargetName(tStory, CurrentPlayerToken.Name);
                CurrentAnimation = tStory;
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

        private void _specialMove_OnCompleted(object sender, object e)
        {
            CurrentAnimation.Stop();
            GameEngine.MakeSpecialMove(CurrentPlayerToken, _gameBoard);
            AnimationInProgress = false;
        }
    }
}
