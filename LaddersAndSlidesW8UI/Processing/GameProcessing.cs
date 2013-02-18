namespace LaddersAndSlidesW8UI.Processing
{
    using System;
    using Interfaces;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    public class GameProcessing : IGameProcessing
    {
        public double TileHeight { get; set; }
        public double TileWidth { get; set; }
        public GameStateEngine CurrentState { get; set; }

        protected Random RandomNumberGenerator { get; set; }
        protected double ArrowSpinDuration { get; set; }

        public void CalculateTileHeightWidth(Canvas gameBoard)
        {
            TileHeight = gameBoard.ActualHeight/10;
            TileWidth = gameBoard.ActualWidth/10;
        }

        public void RenderSpinner(Canvas gameSpinner, Image arrow)
        {
            Canvas.SetTop(arrow, (gameSpinner.ActualHeight / 2) - (arrow.ActualHeight/2) + (arrow.ActualHeight * .07));
            Canvas.SetLeft(arrow, ((gameSpinner.ActualWidth / 2) - (arrow.ActualWidth / 2)) + (arrow.ActualHeight * .015));
        }

        public void CalculateArrowSpin()
        {
            //generate new random number used for calculation
            RandomNumberGenerator = new Random();
            ArrowSpinDuration = RandomNumberGenerator.Next(2500) + RandomNumberGenerator.Next(500, 1500);
        }

        public void SpinArrow(Image arrow)
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
    }
}