namespace LaddersAndSlidesW8UI.Processing.Interfaces
{
    using Windows.UI.Xaml.Controls;

    public interface IGameProcessing
    {
        GameStateEngine CurrentState { get; set; }
        double TileHeight { get; set; }
        double TileWidth { get; set; }

        void CalculateTileHeightWidth(Canvas gameBoard);
        void RenderSpinner(Canvas gameSpinner, Image arrow);
        void CalculateArrowSpin();
        void SpinArrow(Image arrow);
    }
}