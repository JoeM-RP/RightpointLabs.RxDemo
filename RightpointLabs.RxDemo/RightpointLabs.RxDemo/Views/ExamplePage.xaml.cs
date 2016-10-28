using System;

using Xamarin.Forms;

namespace RightpointLabs.RxDemo.Views
{
    public partial class ExamplePage : ContentPage
    {
        int _clickCount = 0;

        readonly Label _buttonClickedInformation;
        readonly Button _myButton;

        public ExamplePage()
        {
            // How to
            _myButton = new Button
            {
                Text = "This is a button"
            };

            _buttonClickedInformation = new Label { };

            Content = new StackLayout
            {
                Children = {
                    _myButton,
                    _buttonClickedInformation
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Register Subscriptions
            _myButton.Clicked += Button_Clicked;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Dispose of subscriptions
            _myButton.Clicked -= Button_Clicked;
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            // Long-running task might be executed here ...
            _clickCount++;

            // Update the UI manually
            _buttonClickedInformation.Text = $"Clicked {_clickCount} times";

            // Are we on the UI thread? If we forget... 💣
        }
    }
}
