namespace LaddersAndSlides_GameEngineTests.Engine
{
    using System.Collections.Generic;
    using LaddersAndSlidesW8UI.Processing;
    using LaddersAndSlides_GameEngine.Domain;
    using LaddersAndSlides_GameEngine.Engine;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Should;

    [TestClass]
    public class GameEngine_GetNextPlayerTests
    {
        private GameEngine _gameEngine;
        private Player _player1;
        private Player _player2;
        private Player _player3;

        private const string Player1Name = "Player1";
        private const string Player2Name = "Player2";
        private const string Player3Name = "Player3";

        [TestInitialize]
        public void Setup()
        {
            _player1 = new Player {CurrentMovesRemaining = 0, CurrentTileNumber = 0, Name = Player1Name, TurnOrder = 1};
            _player2 = new Player {CurrentMovesRemaining = 0, CurrentTileNumber = 0, Name = Player2Name, TurnOrder = 2};
            _player3 = new Player {CurrentMovesRemaining = 0, CurrentTileNumber = 0, Name = Player3Name, TurnOrder = 3};

            var players = new List<Player> { _player1, _player2, _player3 };
            _gameEngine = new GameEngine { Players = players };

        }

        [TestMethod]
        public void GetNextPlayer_returns_player1_when_starting_game()
        {
            //Method under test
            _gameEngine.GetNextPlayer();

            _gameEngine.CurrentPlayer.ShouldNotBeNull("Current Player should not be null");
            _gameEngine.CurrentPlayer.Name.ShouldBeSameAs(Player1Name);
            _gameEngine.CurrentState.ShouldEqual(GameStateEngine.ArrowEvent);
        }

        [TestMethod]
        public void GetNextPlayer_returns_second_player_when_player1_previous_player()
        {
            _gameEngine.CurrentPlayer = _player1;

            //Method under test
            _gameEngine.GetNextPlayer();

            _gameEngine.CurrentPlayer.ShouldNotBeNull("Current Player should not be null");
            _gameEngine.CurrentPlayer.ShouldEqual(_player2);
            _gameEngine.CurrentState.ShouldEqual(GameStateEngine.ArrowEvent);
        }

        [TestMethod]
        public void GetNextPlayer_returns_player1_when_everyone_has_had_a_turn()
        {
            _gameEngine.CurrentPlayer = _player3;

            //Method under test
            _gameEngine.GetNextPlayer();

            _gameEngine.CurrentPlayer.ShouldNotBeNull("Current Player should not be null");
            _gameEngine.CurrentPlayer.ShouldEqual(_player1, string.Format("Expected: '{0}' But Actual: '{1}'", _player1.Name, _gameEngine.CurrentPlayer.Name));
            _gameEngine.CurrentState.ShouldEqual(GameStateEngine.ArrowEvent);
        }

        [TestMethod]
        public void GetNextPlayer_returns_last_player_in_list_when_her_turn()
        {
            _gameEngine.CurrentPlayer = _player2;

            //Method under test
            _gameEngine.GetNextPlayer();

            _gameEngine.CurrentPlayer.ShouldNotBeNull("Current Player should not be null");
            _gameEngine.CurrentPlayer.ShouldEqual(_player3, string.Format("Expected: '{0}' But Actual: '{1}'", _player3.Name, _gameEngine.CurrentPlayer.Name));
            _gameEngine.CurrentState.ShouldEqual(GameStateEngine.ArrowEvent);
        }
    }
}
