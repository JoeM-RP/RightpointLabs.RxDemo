using System.Reactive.Disposables;
using ReactiveUI;
using RightpointLabs.RxDemo.Extensions;
using RightpointLabs.RxDemo.ViewModels;
using Xamarin.Forms;

namespace RightpointLabs.RxDemo.Views
{
    public partial class MainPage : IViewFor<MainPageViewModel>
    {
        private Switch searchRefresh;
        private Entry textEntry;
        private Button search;
        private ListView searchResults;

        public MainPageViewModel ViewModel { get; set; }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainPageViewModel)value; }
        }

        readonly CompositeDisposable _bindingsDisposable = new CompositeDisposable();
        public MainPage()
        {
            InitializeComponent();

            this.Title = "CTA";
            this.Content = GetLandingPageView();


            this.ViewModel = new MainPageViewModel(new ApiService());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Two-way bind
            this.Bind(ViewModel, x => x.SearchQuery, c => c.textEntry.Text).DisposeWith(_bindingsDisposable);

            // One-way bind
            this.OneWayBind(ViewModel, x => x.SearchResults, c => c.searchResults.ItemsSource).DisposeWith(_bindingsDisposable);

            // Command Bind
            this.BindCommand(ViewModel, x => x.Search, c => c.search).DisposeWith(_bindingsDisposable);
            //this.BindCommand(ViewModel, x => x.Search, c => c.searchResults.RefreshCommand).DisposeWith(_bindingsDisposable);

            this.ViewModel.Search.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Dispose of all bindings and any other subscriptions
            _bindingsDisposable.Clear();
        }

        private View GetLandingPageView()
        {
            var topBar = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    (textEntry = new Entry { Placeholder = " Search", HorizontalOptions = LayoutOptions.FillAndExpand, }),
                    (search = new Button {Text = "Refresh", IsEnabled = true}),
                }
            };

            var root = new StackLayout()
            {
                Padding = new Thickness(0, 10),
                Children =
                {
                    (topBar),
                    (searchResults = new ListView
                    {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HasUnevenRows = true,
                        ItemTemplate = new DataTemplate(typeof(ArrivalCell)),
                        SeparatorColor = Color.White,
                        SeparatorVisibility = SeparatorVisibility.Default
                        //IsPullToRefreshEnabled = true
                    })
                }
            };


            return root;
        }
    }
}
