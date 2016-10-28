using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Accounts;
using ReactiveUI;
using RightpointLabs.RxDemo.Models;
using Splat;

namespace RightpointLabs.RxDemo.ViewModels
{
    // Note that a ViewModel must inherit from ReactiveObject, which implements INotifyPropertyChanged, et al
    public class MainPageViewModel : ReactiveObject
    {
        public IApiService ApiService { get; set; }

        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set { this.RaiseAndSetIfChanged(ref _searchQuery, value); }
        }

        private bool _isRefreshEnabled;

        public bool IsRefreshEnabled
        {
            get { return _isRefreshEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isRefreshEnabled, value); }
        }

        private ObservableCollection<object> _searchResults;
        public ObservableCollection<object> SearchResults
        {
            get { return _searchResults; }
            set { this.RaiseAndSetIfChanged(ref _searchResults, value); }
        }

        private ReactiveCommand<List<Arrival>> _search;
        public ReactiveCommand<List<Arrival>> Search
        {
            get { return _search; }
            private set { this.RaiseAndSetIfChanged(ref _search, value); }
        }

        public MainPageViewModel(IApiService apiService = null)
        {
            IObservable<bool> canSearch;

            ApiService = apiService ?? Locator.Current.GetService<IApiService>();

            IsRefreshEnabled = true;
            SearchQuery = "Washington & Wells"; // Simulate location services finding the nearest CTA train stop
            SearchResults = new ObservableCollection<object>();


            // This describes (declaratively) the conditions in which the command is enabled. 
            // IsEnabled is more efficient now, because we only update the UI when changed
            // Notice that we can do some instrumentation here using .Do
            var searchQueryNotEmpty = this.WhenAnyValue(vm => vm.SearchQuery).Select(query => !string.IsNullOrEmpty(query)).DistinctUntilChanged();
            var searchNotExecuting = this.WhenAnyObservable(x => x.Search.IsExecuting).DistinctUntilChanged();
            var refreshEnabled = this.WhenAnyValue(x => x.IsRefreshEnabled).DistinctUntilChanged();

            canSearch = Observable.CombineLatest(searchQueryNotEmpty, searchNotExecuting, refreshEnabled,
                        (isSearchQuery, isExecuting, allowRefresh) => isSearchQuery && allowRefresh && !isExecuting)
                    .Do(cps => Debug.WriteLine("$Can perfrom search query!"))
                    .DistinctUntilChanged();


            // ReactiveCommand has built-in support for background ops and ensures that a block
            // will only execute once at a time (useful for preventing many click senarios).
            // CanExecute will be diabled automatically when this execution enters this block and 
            // IsExecuting will be set accordingly while running
            Search = ReactiveCommand.CreateAsyncTask(canSearch, async _ =>
            {
                if(apiService == null)
                    throw new TimeoutException("Api Service is currently unavailable");

                var result = await apiService.Refresh();

                // Return results here. We can further refine results from the api using LINQ here if desired
                return result.ArrivalList;
            });


            // ReactiveCommands are IObservable, whose value are the results
            // from the async method. We can take the search results loaded in the background
            // and add them to our results and show on the UI Thread
            Search.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(results =>
                {
                    SearchResults.Clear();

                    // Here, we could further refine our results using LINQ.
                    // In this case, we sort by arrival time (a calculated value)
                    // to ensure trains that are "Due" arrive firstin the list
                    foreach (var arrival in results.OrderBy( a=> a.ArrivalTime ))
                        SearchResults.Add(arrival);
                });


            // ThrownExceptions is any exception thrown from the CreateFromObservable piped
            // to this observable. Subscribing to this allows us to handle errors on the UI thread
            Search.ThrownExceptions
                .Subscribe(async ex =>
                {
                    if(Debugger.IsAttached) Debugger.Break();

                    // Show a dialog to the user - UserError is renamed to UserInteraction in v7
                    var result = await UserError.Throw("Network Error", ex);

                    // here, we can take different actions depending on the user input
                    if (result == RecoveryOptionResult.RetryOperation && Search.CanExecute(null))
                        Search.Execute(null);
                });


            // Throttle can be used to limit the flow of events. For example, in text suggestions,
            // auto complete, or a dynamic search, we may want to wait until the user appears to have
            // stopped typing before executing the command
            this.WhenAnyValue(x => x.SearchQuery)
                .Throttle(TimeSpan.FromSeconds(.75), TaskPoolScheduler.Default)
                .Do(x => Debug.WriteLine($"Throttle fired for {x}"))
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(Search);
        }
    }
}
