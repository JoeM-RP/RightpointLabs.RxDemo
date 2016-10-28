using Xamarin.Forms;

namespace RightpointLabs.RxDemo.Views
{
    public class ArrivalCell : ViewCell
    {
        public ArrivalCell()
        {
            var root = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(15, 10),
                Margin = new Thickness(0,0,0,1)
            };
            root.SetBinding(VisualElement.BackgroundColorProperty, "Shade");

            var colorLabel = new Label()
            {
                MinimumWidthRequest = 100,
            };
            colorLabel.SetBinding(Label.TextProperty, "Route");

            var runLabel = new Label();
            runLabel.SetBinding(Label.TextProperty, "DestinationName");

            var etaLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand
            };
            etaLabel.SetBinding(Label.TextProperty, "ETA");


            root.Children.Add(colorLabel);
            root.Children.Add(runLabel);
            root.Children.Add(etaLabel);

            this.View = root;
        }
    }
}
