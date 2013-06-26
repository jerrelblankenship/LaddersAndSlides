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
        public List<SpecialMove> SpecialMoves { get; set; } 
        public Player CurrentPlayer { get; set; }
        public GameStateEngine CurrentState { get; set; }
        public SpecialMove CurrentSpecialMove { get; set; }

        protected Random RandomNumberGenerator { get; set; }
        protected double ArrowSpinDuration { get; set; }
        protected int NumberOfMovesForCurrentPlayer { get; set; }
        protected DateTime WaitStart { get; set; }

        public GameEngine()
        {
            CreateListSpecialMoves();
        }

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
            arrow.Projection = new PlaneProjection { CenterOfRotationY = 0.5, CenterOfRotationX = 0.5, RotationZ = ArrowSpinDuration };
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
                Source = bitmapImage,
                RenderTransform = new CompositeTransform()
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

        public void ProcessPlayerEvent(Image currentPlayerToken, Grid gutter, Grid gameBoard)
        {
            if (DateTime.Now.Subtract(WaitStart).Milliseconds > 100)
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
                    CurrentPlayer.CurrentGridColumn = 0;
                    CurrentPlayer.CurrentGridRow = 9;
                }
                else
                {
                    var currentColumn = Grid.GetColumn(currentPlayerToken);
                    var currentRow = Grid.GetRow(currentPlayerToken);

                    if (CurrentPlayer.CurrentTileNumber + CurrentPlayer.CurrentMovesRemaining > 100)
                    {
                        CurrentPlayer.CurrentMovesRemaining = 0;
                    }
                    else
                    {
                        MakePlayerMove(currentPlayerToken, currentRow, currentColumn);
                    }
                }

                if (CurrentPlayer.CurrentMovesRemaining <= 0)
                {
                    CurrentState = GameStateEngine.GetNextPlayer;
                    CurrentPlayer.TurnInProcess = false;

                    var specialMove = IsSpecialMove(CurrentPlayer.CurrentTileNumber);
                    
                    if (specialMove)
                    {
                        CurrentState = GameStateEngine.PlayerSpecialMoveTransportMoveEvent;
                    }
                    else if (CurrentPlayer.CurrentTileNumber == 100)
                    {
                        CurrentState = GameStateEngine.WinnerDeclared;
                    }
                }
            }
        }

        internal void MakePlayerMove(Image currentPlayerToken, int currentRow, int currentColumn)
        {
            if (CurrentPlayer.CurrentTileNumber%10 == 0)
            {
                Grid.SetRow(currentPlayerToken, currentRow - 1);
                CurrentPlayer.CurrentlyOnAlternateRow = currentRow%2 != 0;
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
            CurrentPlayer.CurrentGridColumn = Grid.GetColumn(currentPlayerToken);
            CurrentPlayer.CurrentGridRow = Grid.GetRow(currentPlayerToken);
        }

        public void MakeSpecialMove(Image currentPlayerToken, Grid gameBoard)
        {
            Grid.SetColumn(currentPlayerToken, CurrentSpecialMove.EndingGridColumn);
            Grid.SetRow(currentPlayerToken, CurrentSpecialMove.EndingGridRow);
            CurrentPlayer.CurrentTileNumber = CurrentSpecialMove.EndingTileNumber;
            CurrentPlayer.CurrentlyOnAlternateRow = CurrentSpecialMove.EndingGridRow % 2 == 0;
            CurrentState = GameStateEngine.GetNextPlayer;
            CurrentPlayer.TurnInProcess = false;
        }

        internal bool IsSpecialMove(int playerTile)
        {
            var result = SpecialMoves.FirstOrDefault(x => x.StartingTileNumber == playerTile);

            if (result != null)
            {
                CurrentSpecialMove = result;
                CurrentPlayer.SpecialMoveTransportDestination = result.EndingTileNumber;
            }

            return result != null;
        }

        public void CreateListSpecialMoves()
        {
            SpecialMoves = new List<SpecialMove>();

            var specialMoves = new Dictionary<int, int>
                {
                    {1, 38}, {4, 14}, {9, 31}, {21, 42},
                    {28, 84}, {51, 67}, {71, 91}, {80, 100},
                    {16,2}, {47,26}, {49,11}, {56,53},
                    {64,60}, {87,24}, {93,73}, {95,75},
                    {98,78}
                };

            foreach (var specialMove in specialMoves)
            {
                Dictionary<string, int> startingGridLocation = GetGridColumnRow(specialMove.Key);
                Dictionary<string, int> endingGridLocation = GetGridColumnRow(specialMove.Value);

                var sMove = new SpecialMove
                    {
                        StartingTileNumber = specialMove.Key,
                        StartingGridColumn = startingGridLocation["column"],
                        StartingGridRow = startingGridLocation["row"],
                        EndingTileNumber = specialMove.Value,
                        EndingGridColumn = endingGridLocation["column"],
                        EndingGridRow = endingGridLocation["row"]
                    };

                SpecialMoves.Add(sMove);
            }
        }

        private Dictionary<string, int> GetGridColumnRow(int tileNumber)
        {
            var gridLocation = new Dictionary<string, int>();

            switch (tileNumber)
            {
                case 1:
                    gridLocation.Add("column", 0);
                    gridLocation.Add("row", 9);
                    break;
                case 2:
                    gridLocation.Add("column", 1);
                    gridLocation.Add("row", 9);
                    break;
                case 4:
                    gridLocation.Add("column", 3);
                    gridLocation.Add("row", 9);
                    break;
                case 9:
                    gridLocation.Add("column", 8);
                    gridLocation.Add("row", 9);
                    break;
                case 11:
                    gridLocation.Add("column", 9);
                    gridLocation.Add("row", 8);
                    break;
                case 14:
                    gridLocation.Add("column", 6);
                    gridLocation.Add("row", 8);
                    break;
                case 16:
                    gridLocation.Add("column", 4);
                    gridLocation.Add("row", 8);
                    break;
                case 21:
                    gridLocation.Add("column", 0);
                    gridLocation.Add("row", 7);
                    break;
                case 24:
                    gridLocation.Add("column", 3);
                    gridLocation.Add("row", 7);
                    break;
                case 26:
                    gridLocation.Add("column", 5);
                    gridLocation.Add("row", 7);
                    break;
                case 28:
                    gridLocation.Add("column", 7);
                    gridLocation.Add("row", 7);
                    break;
                case 31:
                    gridLocation.Add("column", 9);
                    gridLocation.Add("row", 6);
                    break;
                case 38:
                    gridLocation.Add("column", 2);
                    gridLocation.Add("row", 6);
                    break;
                case 42:
                    gridLocation.Add("column", 1);
                    gridLocation.Add("row", 5);
                    break;
                case 47:
                    gridLocation.Add("column", 6);
                    gridLocation.Add("row", 5);
                    break;
                case 49:
                    gridLocation.Add("column", 8);
                    gridLocation.Add("row", 5);
                    break;
                case 51:
                    gridLocation.Add("column", 9);
                    gridLocation.Add("row", 4);
                    break;
                case 53:
                    gridLocation.Add("column", 7);
                    gridLocation.Add("row", 4);
                    break;
                case 56:
                    gridLocation.Add("column", 4);
                    gridLocation.Add("row", 4);
                    break;
                case 60:
                    gridLocation.Add("column", 0);
                    gridLocation.Add("row", 4);
                    break;
                case 64:
                    gridLocation.Add("column", 3);
                    gridLocation.Add("row", 3);
                    break;
                case 67:
                    gridLocation.Add("column", 6);
                    gridLocation.Add("row", 3);
                    break;
                case 71:
                    gridLocation.Add("column", 9);
                    gridLocation.Add("row", 2);
                    break;
                case 73:
                    gridLocation.Add("column", 7);
                    gridLocation.Add("row", 2);
                    break;
                case 75:
                    gridLocation.Add("column", 5);
                    gridLocation.Add("row", 2);
                    break;
                case 78:
                    gridLocation.Add("column", 2);
                    gridLocation.Add("row", 2);
                    break;
                case 80:
                    gridLocation.Add("column", 0);
                    gridLocation.Add("row", 2);
                    break;
                case 84:
                    gridLocation.Add("column", 3);
                    gridLocation.Add("row", 1);
                    break;
                case 87:
                    gridLocation.Add("column", 6);
                    gridLocation.Add("row", 1);
                    break;
                case 91:
                    gridLocation.Add("column", 9);
                    gridLocation.Add("row", 0);
                    break;
                case 93:
                    gridLocation.Add("column", 7);
                    gridLocation.Add("row", 0);
                    break;
                case 95:
                    gridLocation.Add("column", 5);
                    gridLocation.Add("row", 0);
                    break;
                case 98:
                    gridLocation.Add("column", 2);
                    gridLocation.Add("row", 0);
                    break;
                case 100:
                    gridLocation.Add("column", 0);
                    gridLocation.Add("row", 0);
                    break;
            }

            return gridLocation;
        }
    }
}
