namespace LaddersAndSlides_GameEngine.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using LaddersAndSlidesW8UI.Processing;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    public class GameEngine
    {
        public List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; }
        public double TileHeight { get; set; }
        public double TileWidth { get; set; }
        public GameStateEngine CurrentState { get; set; }

        protected Random RandomNumberGenerator { get; set; }
        protected double ArrowSpinDuration { get; set; }
        protected int NumberOfMovesForCurrentPlayer { get; set; }
        protected DateTime WaitStart { get; set; }

        public void GetNextPlayer()
        {
            if (CurrentPlayer == null)
            {
                CurrentPlayer = Players.FirstOrDefault(x => x.TurnOrder == 1);
            }
            else
            {
                var nextTurnOrder = CurrentPlayer.TurnOrder + 1;

                CurrentPlayer = nextTurnOrder > Players.Count 
                    ? Players.FirstOrDefault(x => x.TurnOrder == 1) 
                    : Players.FirstOrDefault(x => x.TurnOrder == nextTurnOrder);
            }

            CurrentState = GameStateEngine.TurnComplete;
        }

        public void ProcessArrowEvent(Image arrow)
        {
            arrow.Projection = new PlaneProjection { CenterOfRotationY = 0.415, CenterOfRotationX = 0.5, RotationZ = ArrowSpinDuration };
            ArrowSpinDuration *= .99;

            if (RandomNumberGenerator.NextDouble() < .05 && ArrowSpinDuration < 360)
            {
                CurrentState = GameStateEngine.ArrowDelayedEvent;
                NumberOfMovesForCurrentPlayer = CalculateNumberOfPlayerMoves(ArrowSpinDuration);
                WaitStart = DateTime.Now;
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

        public int CalculateNumberOfPlayerMoves(double arrowSpinAngle)
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

        public void ProcessArrowDelayEvent()
        {
            if (DateTime.Now.Subtract(WaitStart).Seconds > .5) // Wait a half second before starting the movement of the player token.
            {
                CurrentState = GameStateEngine.PlayerEvent;
            }
        }

        public void ProcessPlayerEvent(Image currentPlayerToken, Canvas gutter, Canvas gameBoard)
        {
            if (DateTime.Now.Subtract(WaitStart).Seconds > .25)
            {
                WaitStart = DateTime.Now;

                if (!CurrentPlayer.TurnInProcess)
                {
                    CurrentPlayer.CurrentMovesRemaining = NumberOfMovesForCurrentPlayer;
                    CurrentPlayer.TurnInProcess = true;
                }

                if (currentPlayerToken.Parent == gutter)
                {
                    gutter.Children.Remove(currentPlayerToken);
                    gameBoard.Children.Add(currentPlayerToken);
                    Canvas.SetTop(currentPlayerToken, TileHeight * 9);
                    Canvas.SetLeft(currentPlayerToken, TileWidth / 2);
                    CurrentPlayer.CurrentMovesRemaining--;
                    CurrentPlayer.CurrentTileNumber = 1;
                }
                else
                {
                    var currentPositionTop = Canvas.GetTop(currentPlayerToken);
                    var currentPositionLeft = Canvas.GetLeft(currentPlayerToken);

                    if (CurrentPlayer.CurrentTileNumber % 10 == 0)
                    {
                        Canvas.SetTop(currentPlayerToken, currentPositionTop - TileHeight);

                        var alternateNumber = CurrentPlayer.CurrentTileNumber / 10;

                        CurrentPlayer.CurrentlyOnAlternateRow = alternateNumber % 2 != 0;

                    }
                    else
                    {
                        Canvas.SetTop(currentPlayerToken, currentPositionTop);

                        if (CurrentPlayer.CurrentlyOnAlternateRow)
                        {
                            Canvas.SetLeft(currentPlayerToken, currentPositionLeft - TileWidth);
                        }
                        else
                        {
                            Canvas.SetLeft(currentPlayerToken, currentPositionLeft + TileWidth);
                        }
                    }

                    CurrentPlayer.CurrentTileNumber++;
                    CurrentPlayer.CurrentMovesRemaining--;
                }

                if (CurrentPlayer.CurrentMovesRemaining <= 0)
                {
                    CurrentState = GameStateEngine.GetNextPlayer;
                    CurrentPlayer.TurnInProcess = false;

                    //var specialMove = IsSpecialMove(PlayerTile);
                    //if (specialMove)
                    //{
                    //    //HeadsUpDisplay.Text = "The player landed ended on a ladder.";
                    //    CurrentState = GameStateEngine.PlayerTransportEvent;
                    //}
                }
            }
        }
    }
}
