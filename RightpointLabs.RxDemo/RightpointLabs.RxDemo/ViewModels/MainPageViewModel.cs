using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using ReactiveUI;
using RightpointLabs.RxDemo.Models;
using Splat;

namespace RightpointLabs.RxDemo.ViewModels
{
    // Note that a ViewModel must inherit from ReactiveObject, which implements INotifyPropertyChanged, et al
    public class MainPageViewModel : ReactiveObject
    {
        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set { this.RaiseAndSetIfChanged(ref _searchQuery, value); }
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

        public MainPageViewModel()
        {
            
            SearchResults = new ObservableCollection<object>();

            // This describes (declaratively) the conditions in which the command is enabled. 
            // IsEnabled is more efficient now, because we only update the UI when changed
            // Notice that we can do some instrumentation here using .Do
            var searchQueryNotEmpty = this.WhenAnyValue(vm => vm.SearchQuery).Select(query => !string.IsNullOrEmpty(query)).DistinctUntilChanged();
            var searchNotExecuting = this.WhenAnyObservable(x => x.Search.IsExecuting).DistinctUntilChanged();

            var canSearch = Observable.CombineLatest(searchQueryNotEmpty, searchNotExecuting,
                        (isSearchQuery, isExecuting) => isSearchQuery && !isExecuting)
                    .Do(cps => Debug.WriteLine("$Can perfrom search query!"))
                    .DistinctUntilChanged();

            // ReactiveCommand has built-in support for background ops and ensures that a block
            // will only execute once at a time (useful for preventing many click senarios).
            // CanExecute will be diabled automatically when this execution enters this block and 
            // IsExecuting will be set accordingly while running
            Search = ReactiveCommand.CreateAsyncTask(canSearch, async _ =>
            {
                #region Simulate Network Problems
#if DEBUG
                //if (DateTime.Now.Second % 3 == 0)
                //    throw new TimeoutException("Unable to connect to web service");
#endif
                #endregion

                // A good spot for DI ...
                var apiService = new ApiService();
                var result = await apiService.Refresh();

                // Return results here. We can further refine results from the api using LINQ here if desired
                return result.ArrivalList;
            });


            // ReactiveCommands are IOBServable, whose value are the results
            // from the async method. We can take the search results loaded in the background
            // and add them to our results and show on the UI Thread
            Search.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(results =>
                {
                    SearchResults.Clear();

                    foreach (var arrival in results)
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
        }
    }
}
