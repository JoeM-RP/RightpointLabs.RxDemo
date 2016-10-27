using Xamarin.Forms;

namespace RightpointLabs.RxDemo
{
    public partial class App : Application
    {
        #region App Data

        public const string MapId = "40730";
        public const string Host = "http://lapi.transitchicago.com/api/";
        public const string ApiKey = "63300e5686d747f1b0e4437759a4797f";
        public const string Version = "1.0";
        public const string Max = "5";

        #endregion

        private static Color BrandPrimary => Color.FromRgb(2, 92, 151);
        private static Color BrandAccent => Color.FromRgb(245, 126, 39);

        public App()
        {
            InitializeComponent();

            MainPage = new Views.MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            InitializeResources();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private void InitializeResources()
        {
            Application app = App.Current;

            if (app == null) return;

            ResourceDictionary resources = app.Resources ?? new ResourceDictionary();

            #region Styles

            // Buttons
            Style buttonBase = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter {Property = VisualElement.BackgroundColorProperty, Value = BrandPrimary},
                    new Setter {Property = Button.TextColorProperty, Value = Color.White},
                },
                Triggers =
                {
                    new Trigger(typeof(Button))
                    {
                        Property = VisualElement.IsEnabledProperty,
                        Value = false,
                        Setters =
                        {
                            new Setter
                            {
                                Property = VisualElement.BackgroundColorProperty,
                                Value = BrandPrimary.MultiplyAlpha(.4)
                            }
                        }
                    }
                }
            };
            Device.OnPlatform(
                iOS: () =>
                {
                    buttonBase.Setters.Add(new Setter { Property = Button.TextColorProperty, Value = Color.White });
                },
                Android: () =>
                {
                    // Add any platform specific styles here
                },
                WinPhone: () =>
                {
                    buttonBase.Setters.Add(new Setter() { Property = VisualElement.HeightRequestProperty, Value = 50 });
                }
            );
            resources.Add(buttonBase);

            // Activity Indicators
            Style activityIndicatorBase = new Style(typeof(ActivityIndicator))
            {
                Setters =
                {
                    new Setter {Property = ActivityIndicator.ColorProperty, Value = BrandAccent},
                }
            };
            resources.Add(activityIndicatorBase);


            // Pages
            Style pageBase = new Style(typeof(Page))
            {
                Setters =
                {
                    new Setter
                    {
                        Property = Page.PaddingProperty,
                        Value = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0)
                    }
                }
            };
            resources.Add(pageBase);

            // Labels
            Style labelBase = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter
                    {
                        Property = Label.VerticalTextAlignmentProperty,
                        Value = TextAlignment.Center
                    }
                }
            };
            resources.Add(labelBase);

            #endregion

            #region Converters

            #endregion

            app.Resources = resources;
        }
    }
}