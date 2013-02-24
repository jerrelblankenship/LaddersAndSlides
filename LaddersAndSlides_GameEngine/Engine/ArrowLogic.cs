namespace LaddersAndSlides_GameEngine.Engine
{
    using System;

    public interface IArrowLogic
    {
        double CalculateArrowSpin();
    }

    public class ArrowLogic : IArrowLogic
    {
        public Random RandomNumberGenerator { get; set; }

        public double CalculateArrowSpin()
        {
            RandomNumberGenerator = new Random();
            return RandomNumberGenerator.Next(2500) + RandomNumberGenerator.Next(200, 800);
        }
    }
}
