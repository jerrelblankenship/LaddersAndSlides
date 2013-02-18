namespace LaddersAndSlidesW8UI.Processing.Interfaces
{
    using LadderAndSlides_Domain.Domain;
    using Windows.UI.Xaml.Controls;

    public interface IPlayerProcessing
    {
        Image CreatePlayerToken(Player player);
    }
}