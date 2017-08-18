using System;
using System.Collections.Generic;
using ReactiveUI;
using RightpointLabs.RxDemo.ViewModels;
using Xamarin.Forms;
using Xamvvm;

namespace RightpointLabs.RxDemo.Views
{
    public partial class MainPage : ContentPage, IBasePageRxUI<MainPageViewModel>
    {
        public MainPage()
        {
            InitializeComponent();

			Interactions.Errors.RegisterHandler(
			async interaction =>
			{
				var action = await DisplayAlert(
					$"Error: {interaction.Input.Message}",
					"Sandbox ran in to an issue and couldn't complete the operation. If you're experiencing connectivity issues, not all functionality will be available",
					"Retry",
					"Cancel");

				interaction.SetOutput(action ? ErrorRecoveryOption.Retry : ErrorRecoveryOption.Cancel);
			});
		}

		public MainPageViewModel ViewModel { get; set; }
		object IViewFor.ViewModel { get; set; }

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
    }
}
