namespace LaddersAndSlides_GameEngineTests.Engine
{
    using LaddersAndSlides_GameEngine.Engine;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Should;

    [TestClass]
    public class GameEngine_IsSpecialMoveTests
    {
        private GameEngine _engine;

        [TestInitialize]
        public void Setup()
        {
            _engine = new GameEngine();
        }

        [TestMethod]
        public void IsSpecialMove_returns_true_when_tile_is_start_of_special_move()
        {
            var result = _engine.IsSpecialMove(9);
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void IsSpecialMove_returns_false_when_tile_is_not_start_of_special_move()
        {
            var result = _engine.IsSpecialMove(10);
            result.ShouldBeFalse();
        }
    }
}
