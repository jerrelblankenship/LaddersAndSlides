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
        private readonly IArrowLogic _arrowLogic;
        public List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; }
        public double TileHeight { get; set; }
        public double TileWidth { get; set; }
        public GameStateEngine CurrentState { get; set; }
        public GameMoveSpecial CurrentSpecialMove { get; set; }

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
            //ArrowSpinDuration = _arrowLogic.CalculateArrowSpin();

            arrow.Projection = new PlaneProjection { CenterOfRotationY = 0.415, CenterOfRotationX = 0.5, RotationZ = ArrowSpinDuration };
            ArrowSpinDuration *= .98;

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
                Width = 55,
                Height = 55,
                Name = player.Name,
                Source = bitmapImage
            };

            return image;
        }

        public int CalculateNumberOfPlayerMoves(double arrowSpinAngle)
        {
            return 100;
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

        public void CalculateTileHeightWidth(Grid gameBoard)
        {
            TileHeight = gameBoard.ActualHeight / 10 + 2;
            TileWidth = gameBoard.ActualWidth / 10;
        }

        public void CalculateArrowSpin()
        {
            //generate new random number used for calculation
            RandomNumberGenerator = new Random();
            ArrowSpinDuration = RandomNumberGenerator.Next(2500) + RandomNumberGenerator.Next(200, 800);
        }

        public void ProcessArrowDelayEvent()
        {
            if (DateTime.Now.Subtract(WaitStart).Seconds > .45) // Wait a half second before starting the movement of the player token.
            {
                CurrentState = GameStateEngine.PlayerEvent;
            }
        }

        public void ProcessPlayerEvent(Image currentPlayerToken, Canvas gutter, Grid gameBoard)
        {
            if (DateTime.Now.Subtract(WaitStart).Milliseconds > 375)
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
                    
                    Grid.SetRow(currentPlayerToken, 9);
                    Grid.SetColumn(currentPlayerToken, 0);
                    
                    CurrentPlayer.CurrentMovesRemaining--;
                    CurrentPlayer.CurrentTileNumber = 1;
                }
                else
                {
                    var currentColumn = Grid.GetColumn(currentPlayerToken);
                    var currentRow = Grid.GetRow(currentPlayerToken);

                    if (CurrentPlayer.CurrentTileNumber % 10 == 0)
                    {
                        Grid.SetRow(currentPlayerToken, currentRow - 1);
                        CurrentPlayer.CurrentlyOnAlternateRow = currentRow % 2 != 0;
                    }
                    else
                    {
                        if (CurrentPlayer.CurrentlyOnAlternateRow)
                        {
                            Grid.SetColumn(currentPlayerToken, currentColumn - 1);
                        }
                        else
                        {
                            Grid.SetColumn(currentPlayerToken, currentColumn + 1);
                        }
                    }

                    CurrentPlayer.CurrentTileNumber++;
                    CurrentPlayer.CurrentMovesRemaining--;
                }

                if (CurrentPlayer.CurrentMovesRemaining <= 0)
                {
                    CurrentState = GameStateEngine.GetNextPlayer;
                    CurrentPlayer.TurnInProcess = false;

                    var specialMove = IsSpecialMove(CurrentPlayer.CurrentTileNumber);
                    if (specialMove)
                    {
                        CurrentState = GameStateEngine.PlayerSpecialMoveTransportCalculateEvent;
                    }
                }
            }
        }

        public void CalculateSpecialMove(Image currentPlayerToken, Grid gameBoard)
        {
            var transportStartY = Canvas.GetTop(currentPlayerToken);
            var transportStartX = Canvas.GetLeft(currentPlayerToken);
            var transportDestinationX = transportStartX;
            var transportDestinationY = transportStartY;
            
            var isLadderMove = CurrentPlayer.SpecialMoveTransportDestination > CurrentPlayer.CurrentTileNumber;

            if (isLadderMove)
            {
                var currentLocation = CurrentPlayer.CurrentTileNumber;
                var tempAlternateRow = CurrentPlayer.CurrentlyOnAlternateRow;
                var numberOfMoves = CurrentPlayer.SpecialMoveTransportDestination - CurrentPlayer.CurrentTileNumber;

                for (var i = 1; i <= numberOfMoves; i++)
                {
                    if (currentLocation%10 == 0)
                    {
                        transportDestinationY = transportDestinationY - TileHeight;

                        var alternateNumber = currentLocation/10;

                        tempAlternateRow = alternateNumber%2 != 0;
                    }
                    else
                    {
                        if (tempAlternateRow)
                        {
                            transportDestinationX = transportDestinationX - TileWidth;
                        }
                        else
                        {
                            transportDestinationX = transportDestinationX + TileWidth;
                        }
                    }

                    currentLocation++;
                }
            }
            else
            {
                transportDestinationX = TileWidth / 2;
                transportDestinationY = TileHeight * 9;
                var tempAlternateRow = false;

                for (var i = 1; i < CurrentPlayer.SpecialMoveTransportDestination; i++)
                {
                    if (i % 10 == 0)
                    {
                        transportDestinationY = transportDestinationY - TileHeight;

                        var alternateNumber = i / 10;

                        tempAlternateRow = alternateNumber % 2 != 0;
                    }
                    else
                    {
                        if (tempAlternateRow)
                        {
                            transportDestinationX = transportDestinationX - TileWidth;
                        }
                        else
                        {
                            transportDestinationX = transportDestinationX + TileWidth;
                        }
                    }
                }
            }

            CurrentSpecialMove = new GameMoveSpecial
                {
                    TransportStartX = transportStartX,
                    TransportStartY = transportStartY,
                    TransportDestinationX = transportDestinationX,
                    TransportDestinationY = transportDestinationY + 4,
                    TransportTime = 0
                };

            CurrentState = GameStateEngine.PlayerSpecialMoveTransportMoveEvent;
        }

        public void MakeSpecialMove(Image currentPlayerToken, Grid gameBoard)
        {
            var xCurrent = CurrentSpecialMove.TransportStartX + (CurrentSpecialMove.TransportDestinationX - CurrentSpecialMove.TransportStartX) * CurrentSpecialMove.TransportTime;
            var yCurrent = CurrentSpecialMove.TransportStartY + (CurrentSpecialMove.TransportDestinationY - CurrentSpecialMove.TransportStartY) * CurrentSpecialMove.TransportTime;

            Canvas.SetTop(currentPlayerToken, yCurrent);
            Canvas.SetLeft(currentPlayerToken, xCurrent);
            CurrentSpecialMove.TransportTime += 0.05;

            if (CurrentSpecialMove.TransportTime >= 1)
            {
                var alternateNumber = CurrentPlayer.SpecialMoveTransportDestination / 10;

                CurrentPlayer.CurrentlyOnAlternateRow = alternateNumber % 2 != 0;
                CurrentPlayer.CurrentTileNumber = CurrentPlayer.SpecialMoveTransportDestination;

                CurrentState = GameStateEngine.GetNextPlayer;
                CurrentPlayer.TurnInProcess = false;
            }
        }

        internal bool IsSpecialMove(int playerTile)
        {
            var specialMoves = new Dictionary<int, int>
                {
                    {1, 38},
                    {4, 14},
                    {9, 31},
                    {21, 42},
                    {28, 84},
                    {51, 67},
                    {71, 91},
                    {80, 100},
                    {16,2},
                    {47,26},
                    {49,11},
                    {56,53},
                    {64,60},
                    {87,24},
                    {93,73},
                    {95,75},
                    {98,78}
                };

            var result = specialMoves.FirstOrDefault(x => x.Key == playerTile);

            if (result.Key != 0)
            {
                CurrentPlayer.SpecialMoveTransportDestination = result.Value;
            }

            return false;
            //return result.Key != 0;
        }

        internal void MovePlayerToken(Image currentPlayerToken, double currentPositionLeft)
        {
            if (CurrentPlayer.CurrentlyOnAlternateRow)
            {
                Canvas.SetLeft(currentPlayerToken, currentPositionLeft - TileWidth);
            }
            else
            {
                Canvas.SetLeft(currentPlayerToken, currentPositionLeft + TileWidth);
            }
        }

        
    }
}
