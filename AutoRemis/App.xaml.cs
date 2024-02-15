using static AutoRemis.Helpers.AppStateManager;
using AutoRemis.Models;
using AutoRemis.ViewModels;
using AutoRemis.Views;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;
using AutoRemis.Interfaces;

namespace AutoRemis
{
    public partial class App
    {
        public App(IPlatformInitializer initializer) : base(initializer) { }
        private IFirebaseManager _firebaseManager;

        protected override async void OnInitialized()
        {
            InitializeComponent();
            _firebaseManager = DependencyService.Get<IFirebaseManager>();


            var user = GetUser();

            switch (user.Status)
            {
                case UserStatus.Disconnected:
                    await NavigationService.NavigateAsync("NavigationPage/AviableUpdatePopUp");
                    break;
                case UserStatus.Idle:
                    await NavigationService.NavigateAsync("NavigationPage/TestPage");
                    break;
            }

            var token = await _firebaseManager.GetFirebaseToken();

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            //Init Views
            containerRegistry.RegisterForNavigation<OnBoardingPage, OnBoardingPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<ConfirmPhonePage, ConfirmPhonePageViewModel>();
            containerRegistry.RegisterForNavigation<SideMenuPage, SideMenuPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();

            //Trip Views
            containerRegistry.RegisterForNavigation<Trip_AcceptedPage, Trip_AcceptedPageViewModel>();
            containerRegistry.RegisterForNavigation<Trip_ConfigPage, Trip_ConfigPageViewModel>();
            containerRegistry.RegisterForNavigation<Trip_WaitingPage, Trip_WaitingPageViewModel>();
            containerRegistry.RegisterForNavigation<Trip_DetailsPopUp, Trip_DetailsPopUpViewModel>();
            containerRegistry.RegisterForNavigation<Trip_ChangeMainParamsPopUp, Trip_ChangeMainParamsPopUpViewModel>();
            containerRegistry.RegisterForNavigation<Trip_StateInfoPopUp, Trip_StateInfoPopUpViewModel>();
            containerRegistry.RegisterForNavigation<Trip_CancelPopUp, Trip_CancelPopUpViewModel>();
            containerRegistry.RegisterForNavigation<Trip_InformationPopUp, Trip_InformationPopUpViewModel>();
            containerRegistry.RegisterForNavigation<Trip_InProcessPage, Trip_InProcessPageViewModel>();
            containerRegistry.RegisterForNavigation<Trip_FinishedPage, Trip_FinishedPageViewModel>();
            containerRegistry.RegisterForNavigation<Trip_RatePage, Trip_RatePageViewModel>();

            containerRegistry.RegisterForNavigation<AviableUpdatePopUp, AviableUpdatePopUpViewModel>();
            containerRegistry.RegisterForNavigation<TestPage, TestPageViewModel>();
        }
    }
}