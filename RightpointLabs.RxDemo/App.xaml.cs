using System;
using ReactiveUI;
using RightpointLabs.RxDemo.ViewModels;
using Xamarin.Forms;
using Xamvvm;

namespace RightpointLabs.RxDemo
{
	public static class Interactions
	{
		public static readonly Interaction<Exception, ErrorRecoveryOption> Errors = new Interaction<Exception, ErrorRecoveryOption>();
	}

	public class AppShell : BasePageModelRxUI
	{

	}

    public partial class App : Application
    {
		#region App Data

		public const string MapId = "40730";
		public const string Host = "http://lapi.transitchicago.com/api/";
		public const string ApiKey = "63300e5686d747f1b0e4437759a4797f";
		public const string Version = "1.0";
		public const string Max = "15";

		#endregion

		public App()
        {
            InitializeComponent();

			var factory = new XamvvmFormsRxUIFactory(this);
			XamvvmCore.SetCurrentFactory(factory);

			factory.RegisterNavigationPage<AppShell>(() => this.GetPageFromCache<MainPageViewModel>());

			MainPage = this.GetPageAsNewInstance<AppShell>() as Page;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
