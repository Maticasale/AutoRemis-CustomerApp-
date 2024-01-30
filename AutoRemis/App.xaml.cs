using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.ViewModels;
using AutoRemis.Views;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace AutoRemis
{
    public partial class App
    {
        User user;
        public App(IPlatformInitializer initializer) : base(initializer) { }
        protected override async void OnInitialized()
        {
            InitializeComponent();

            switch (AppStateManager.GetUser().Status)
            {
                case UserStatus.Disconnected:
                    await NavigationService.NavigateAsync("NavigationPage/OnBoardingPage");
                    break;
                case UserStatus.Idle:
                    await NavigationService.NavigateAsync("NavigationPage/Trip_FinishedPage");
                    break;
            }


            FirebaseHelper.Initialize();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();            
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            containerRegistry.RegisterForNavigation<OnBoardingPage, OnBoardingPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<ConfirmPhonePage, ConfirmPhonePageViewModel>();
            containerRegistry.RegisterForNavigation<SideMenuPage, SideMenuPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();

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

            containerRegistry.RegisterForNavigation<TestPage, TestPageViewModel>();
        }
    }
}