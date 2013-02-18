namespace LaddersAndSlidesW8UI.Processing
{
    using Interfaces;
    using LadderAndSlides_Domain.Domain;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;

    public class PlayerProcessing : IPlayerProcessing
    {
        public Image CreatePlayerToken(Player player)
        {
            var image = new Image {Width = 65, Height = 65};
            var bitmapImage = new BitmapImage {UriSource = player.ImageUri};

            image.Source = bitmapImage;

            return image;
        }
    }
}
