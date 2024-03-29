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
using AutoRemis.Helpers;
using Prism.Navigation;
using static AutoRemis.Helpers.LocationHelper;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;

namespace AutoRemis
{
    public partial class App
    {
        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            if (GetUser().Status == UserStatus.Disconnected)
                await NavigationService.NavigateAsync("NavigationPage/OnBoardingPage");
            else
                await NavigationService.NavigateAsync("NavigationPage/LoadingPage", animated: false);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.RegisterForNavigation<NavigationPage>();

            //Init Views
            containerRegistry.RegisterForNavigation<LoadingPage, LoadingPageViewModel>();
            containerRegistry.RegisterForNavigation<OnBoardingPage, OnBoardingPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<ConfirmPhonePage, ConfirmPhonePageViewModel>();

            //Home Views
            containerRegistry.RegisterForNavigation<SideMenuPage, SideMenuPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<HistoryPage, HistoryPageViewModel>();
            containerRegistry.RegisterForNavigation<HelpCenterPage, HelpCenterPageViewModel>();

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
            
            //Dialogs Views
            containerRegistry.RegisterForNavigation<AviableUpdatePopUp, AviableUpdatePopUpViewModel>();

            //TestUis
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<TestPage, TestPageViewModel>();
            containerRegistry.RegisterForNavigation<TestPage2, TestPage2ViewModel>();
            containerRegistry.RegisterForNavigation<TestPage3, TestPage3ViewModel>();
        }
    }
}