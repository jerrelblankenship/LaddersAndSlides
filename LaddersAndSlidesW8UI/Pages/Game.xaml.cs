// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LaddersAndSlidesW8UI.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LadderAndSlides_Domain.Domain;
    using Processing;
    using Processing.Interfaces;
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
        public List<Player> Players { get; set; }

        private DispatcherTimer _timer;
        public DispatcherTimer Timer
        {
            get { return _timer ?? (_timer = new DispatcherTimer {Interval = new TimeSpan(1000)}); }
            set { _timer = value; }
        }

        private IPlayerProcessing _processingHelper;
        public IPlayerProcessing ProcessingHelper
        {
            get { return _processingHelper ?? (_processingHelper = new PlayerProcessing()); }
            set { _processingHelper = value; }
        }

        private IGameProcessing _gameProcessing;
        public IGameProcessing GameProcessing
        {
            get { return _gameProcessing ?? (_gameProcessing = new GameProcessing()); }
            set { _gameProcessing = value; }
        }

        public Game()
        {
            InitializeComponent();
            Players = new List<Player>();
            Timer.Tick += Timer_Tick;
            
            Loaded += GameLoaded;
        }

        void GameLoaded(object sender, RoutedEventArgs e)
        {
            var previousPlayerTokenCombinedHeight = 0.0;
            
            foreach (var image in Players.Select(player => ProcessingHelper.CreatePlayerToken(player)))
            {
                _gutter.Children.Add(image);

                previousPlayerTokenCombinedHeight += image.ActualHeight;
                Canvas.SetTop(image, _gutter.ActualHeight - (previousPlayerTokenCombinedHeight  + 1));
            }

            GameProcessing.CalculateTileHeightWidth(_gameBoard);
            GameProcessing.RenderSpinner(_gameSpinner, _arrow);
        }

        void Timer_Tick(object sender, object e)
        {
            switch (GameProcessing.CurrentState)
            {
                case GameStateEngine.ArrowEvent:
                    GameProcessing.SpinArrow(_arrow);
                break;
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
            if (GameProcessing.CurrentState != GameStateEngine.ArrowEvent)
            {
                GameProcessing.CurrentState = GameStateEngine.ArrowEvent;
                GameProcessing.CalculateArrowSpin();
                Timer.Start();
            }
        }
    }
}
