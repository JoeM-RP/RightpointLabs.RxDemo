using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using RightpointLabs.RxDemo.Extensions;
using RightpointLabs.RxDemo.ViewModels;
using Xamarin.Forms;

namespace RightpointLabs.RxDemo.Views
{
    public partial class MainPage : IViewFor<MainPageViewModel>
    {
        private Switch _searchRefresh;
        private Entry _textEntry;
        private Button _search;
        private ListView _searchResults;

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
            this.Bind(ViewModel, x => x.SearchQuery, c => c._textEntry.Text).DisposeWith(_bindingsDisposable);
            this.Bind(ViewModel, x => x.IsRefreshEnabled, c => c._searchRefresh.IsToggled).DisposeWith(_bindingsDisposable);

            // One-way bind
            this.OneWayBind(ViewModel, x => x.SearchResults, c => c._searchResults.ItemsSource).DisposeWith(_bindingsDisposable);

            // Command Bind
            this.BindCommand(ViewModel, x => x.Search, c => c._search).DisposeWith(_bindingsDisposable);

            this.WhenAnyObservable(x => x.ViewModel.Search.IsExecuting).BindTo(_searchResults, c => c.IsRefreshing).DisposeWith(_bindingsDisposable);

            #region TODO JM: PullToRefresh properties
            //this.BindCommand(ViewModel, x => x.Search, c => c._searchResults.RefreshCommand).DisposeWith(_bindingsDisposable);
            this.WhenAnyObservable(x => x.ViewModel.Search.IsExecuting).BindTo(_searchResults, c => c.IsPullToRefreshEnabled).DisposeWith(_bindingsDisposable);
            #endregion

            // User error allows us to interact with our users and get feedback on how to handle an exception
            UserError
                .RegisterHandler(async (UserError arg) => {
                    var result = await this.DisplayAlert("Failed to get latest schedule", $"{arg.ErrorMessage}{Environment.NewLine}Retry search?", "Yes", "No");
                    return result ? RecoveryOptionResult.RetryOperation : RecoveryOptionResult.CancelOperation;
                }).DisposeWith(_bindingsDisposable);

            // Run the refresh command OnAppearing
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
            var location = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(10, 0),
                Children =
                {
                    (new Label() { Text = "Nearby:", TextColor = Color.Black}),
                    (_textEntry = new Entry { Placeholder = "Search", HorizontalOptions = LayoutOptions.FillAndExpand}),
                }
            };

            var actions = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(10,0),
                Children =
                {
                    ( new Label() { Text = "CTA Tracker", FontSize = 26, TextColor = Color.Black}),                 
                    ( new Label() {Text = "Auto", TextColor = Color.Black, HorizontalOptions = LayoutOptions.EndAndExpand}),
                    (_searchRefresh = new Switch()),
                    (_search = new Button {Text = "Refresh", IsEnabled = true}),
                }
            };

            var root = new StackLayout()
            {
                Padding = new Thickness(0, 10),
                Children =
                {        
                    (actions),            
                    (location),
                    (_searchResults = new ListView(ListViewCachingStrategy.RecycleElement)
                    {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HasUnevenRows = false,
                        ItemTemplate = new DataTemplate(typeof(ArrivalCell)),
                        SeparatorVisibility = SeparatorVisibility.None,
                    })
                }
            };

            return root;
        }
    }
}
