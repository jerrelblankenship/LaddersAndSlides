namespace LaddersAndSlides_GameEngine.Engine
{
    using System;
    using Domain;
    using LaddersAndSlidesW8UI.Processing;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    public class GameEngine
    {
        public double TileHeight { get; set; }
        public double TileWidth { get; set; }
        public GameStateEngine CurrentState { get; set; }

        protected Random RandomNumberGenerator { get; set; }
        protected double ArrowSpinDuration { get; set; }

        public void ProcessArrowEvent(Image arrow)
        {
            arrow.Projection = new PlaneProjection { CenterOfRotationY = 0.415, CenterOfRotationX = 0.5, RotationZ = ArrowSpinDuration };
            ArrowSpinDuration *= .99;

            if (RandomNumberGenerator.NextDouble() < .05 && ArrowSpinDuration < 360)
            {
                CurrentState = GameStateEngine.ArrowDelayedEvent;
                //ArrowEvent = false;

                //var moveNumber = CalculateNumberOfPlayerMoves(ArrowSpinDuration);
                //HeadsUpDisplay.Text = string.Format("Player Moves: {0}", moveNumber);

                //ArrowDelayEvent = true;
                //PlayerEvent = false;
                //PlayerMoves = moveNumber;
                //WaitStart = DateTime.Now;
            }
        }

        public Image CreatePlayerToken(Player player)
        {
            var bitmapImage = new BitmapImage { UriSource = player.ImageUri };

            var image = new Image
            {
                Width = 65,
                Height = 65,
                Name = player.Name,
                Source = bitmapImage
            };

            return image;
        }

        public double CalculateNumberOfPlayerMoves(double arrowSpinAngle)
        {
            if ((arrowSpinAngle >= 0 && arrowSpinAngle <= 40) ||
                (arrowSpinAngle > 320 && arrowSpinAngle <= 360))
                return 4;

            if (arrowSpinAngle > 40 && arrowSpinAngle <= 95)
                return 3;

            if (arrowSpinAngle > 95 && arrowSpinAngle <= 150)
                return 2;

            if (arrowSpinAngle > 150 && arrowSpinAngle <= 209)
                return 1;

            if (arrowSpinAngle > 209 && arrowSpinAngle <= 260)
                return 6;

            return 5;
        }

        public void CalculateTileHeightWidth(Canvas gameBoard)
        {
            TileHeight = gameBoard.ActualHeight / 10;
            TileWidth = gameBoard.ActualWidth / 10;
        }

        public void CalculateArrowSpin()
        {
            //generate new random number used for calculation
            RandomNumberGenerator = new Random();
            ArrowSpinDuration = RandomNumberGenerator.Next(2500) + RandomNumberGenerator.Next(500, 1500);
        }
    }
}
