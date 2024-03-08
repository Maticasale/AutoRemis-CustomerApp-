using System;
using Xamarin.Forms;
using Prism.Navigation;
using AutoRemis.Models;
using AutoRemis.Interfaces;
using AutoRemis.Models.Services;
using System.Threading.Tasks;
using AutoRemis.Models.Google;
using static AutoRemis.Helpers.UIHelper;
using static AutoRemis.Helpers.SoundHelper;
using static AutoRemis.Helpers.AppStateManager;
using AutoRemis.Services;
using Xamarin.Essentials;
using ImTools;
using System.Threading;

namespace AutoRemis.Views
{
    public partial class RegisterPage : ContentPage, INavigatedAware
    {
        private User user; 
        private AppSettings app;

        private InitType init;
        private string smsToken;
        private bool FCMRecived = false;
        private GoogleUser googleUser;

        private readonly INavigationService _navigationService;
        private readonly IFirebaseManager _firebaseManager;
        private readonly IGoogleManager _googleManager;

        public RegisterPage(INavigationService navigationService)
        {
            InitializeComponent();

            _navigationService = navigationService;
            _googleManager = DependencyService.Get<IGoogleManager>();
            _firebaseManager = DependencyService.Get<IFirebaseManager>();

            MessagingCenter.Subscribe<object>(this, "goToConfirmPage", (sender) => Device.BeginInvokeOnMainThread(Init));
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //Parameters
            init = parameters.GetValue<InitType>("LoginType");
            googleUser = parameters.GetValue<GoogleUser>("GoogleUser");

            Device.BeginInvokeOnMainThread(LoadUI);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            if (init == InitType.Google)
                _googleManager.Logout();
        }

        private void LoadUI()
        {
            //User and App Data
            user = GetUser();
            app = GetAppInfo();

            //General UI Settings
            user.Init = init;
            if (init == InitType.Google && !string.IsNullOrWhiteSpace(googleUser.Email))
            {
                imgGooglePerson.IsVisible = true;
                imgGoogleMail.IsVisible = true;

                EntryUser.IsEnabled = false;
                EntryEmail.IsEnabled = false;

                EntryUser.Text = CapitalizeSentence(googleUser.FullName);
                EntryEmail.Text = googleUser.Email;
                user.GoogleUrlPic = googleUser.Picture;
            }
        }

        private async void registerTapped(object sender, EventArgs e)
        {
            IsBusy(true);

            if (string.IsNullOrWhiteSpace(EntryUser.Text) || string.IsNullOrWhiteSpace(EntryEmail.Text) || string.IsNullOrWhiteSpace(EntryPhone.Text))
            {
                RiseErrorMsg("¡Atencion!", init == InitType.Google ? "Necesitamos tu numero de Celular para poder continuar" : "Necesitamos que completes los campos obligatorios para poder continuar", 3, SoundType.Alert);
                return;
            }

            var response = await AuthService.Register(new RegisterUser()
            {
                appVersion = VersionTracking.CurrentVersion,
                email = EntryEmail.Text,
                fullName = EntryUser.Text,
                phoneNumber = EntryPhone.Text,
                token = await _firebaseManager.GetFirebaseToken(),
                usrFcb = EntryFacebook.Text,
            });

            switch (response.ServiceState)
            {
                case ServiceType.CheckOut:
                    string[] name = EntryUser.Text.Split(' ');

                    user.FirstName = CapitalizeSentence(name[0]);
                    user.LastName = name.Length >= 2 ? CapitalizeSentence(name[1]) : "-";
                    user.FullName = CapitalizeSentence(EntryUser.Text);
                    user.Email = EntryEmail.Text;
                    user.Facebook = EntryFacebook.Text;
                    user.PhoneNumber = EntryPhone.Text;
                    UpdateUser(user);

                    app.HelpCenterPhone = response.wsNumber; 
                    app.GlobalApiKey = response.ApiKey;
                    UpdateAppInfo(app);

                    smsToken = response.smsToken;

                    var cts = new CancellationTokenSource();
                    var delayTask = Task.Delay(5000, cts.Token);

                    await Task.WhenAll(Task.FromResult(response), delayTask);

                    if (delayTask.IsCompleted && !FCMRecived)
                        RiseErrorMsg("¡Error!", "El tiempo de espera se ha agotado, por favor, vuelva a intentar", 3, SoundType.Alert);
                    else
                    {
                        await _navigationService.NavigateAsync("ConfirmPhonePage", new NavigationParameters { { "ConfirmationToken", smsToken }, { "LoginType", init } });
                        cts.Cancel();
                    }
                    break;

                case ServiceType.Invalid:
                    if (response.estado == "ERROR")
                        RiseErrorMsg("¡Error!", "Ocurrio un error general,por favor vuelve a intentarlo", 5, SoundType.Alert);

                    if (response.okZona == "0")
                        RiseErrorMsg("¡Error!", "Parece que te encuentras en una zona fuera de las operadas por ViajaYa, no podras utilizar la aplicacion aqui", 5, SoundType.Alert);
                    break;

                case ServiceType.TimeOut:
                    RiseErrorMsg("¡Error!", "Se agoto el tiempo de espera, parece que no esta llegando tu solicitud al servidor. Por favor vuelve a intentarlo", 5, SoundType.Error);
                    break;

                case ServiceType.NoConnection:
                    RiseErrorMsg("¡Error!", "Ha ocurrido un error general tratando de comunicar tu solicitud al servidor. Revisa tu conexión a internet y vuelve a intentarlo", 5, SoundType.Error);
                    break;

                case ServiceType.ResponseFailed:
                    RiseErrorMsg("¡Error!", "Ha ocurrido un error general, por favor vuelva a intentar. Si el error persiste por mucho tiempo prueba reiniciar la app", 5, SoundType.Error);
                    break;
            }  

            IsBusy(false);
        }
        
        private void Init() => FCMRecived = true;

        private new void IsBusy(bool value)
        {
            LoadingIndicator.IsRunning = value;
            btnInit.IsEnabled = !value;
            lblInit.Text = value ? "" : "INICIAR";
        }

        private void RiseErrorMsg(string title, string msg, int time, SoundType type)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Title.Text = title;
                Msg.Text = msg;

                switch (type)
                {
                    case SoundType.Error:
                        imgItem.Source = "iconError.png";
                        CancellBox.BorderColor = Color.FromHex("#ff355b");
                        CancellBox.BackgroundColor = Color.FromHex("#fffbfc");
                        Title.TextColor = Color.FromHex("#ff355b");
                        PlaySound(SoundType.Alert);
                        break;
                    case SoundType.Alert:
                        imgItem.Source = "iconWarning.png";
                        CancellBox.BorderColor = Color.FromHex("#FFC021");
                        CancellBox.BackgroundColor = Color.FromHex("#fffefb");
                        Title.TextColor = Color.FromHex("#FFC021");
                        PlaySound(SoundType.Alert);
                        break;
                    case SoundType.Success:
                        imgItem.Source = "iconSuccess.png";
                        CancellBox.BorderColor = Color.FromHex("#47D764");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfefc");
                        Title.TextColor = Color.FromHex("#47D764");
                        PlaySound(SoundType.Success);
                        break;
                    case SoundType.Message:
                        imgItem.Source = "iconInfo.png";
                        CancellBox.BorderColor = Color.FromHex("#2F86EB");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfdff");
                        Title.TextColor = Color.FromHex("#2F86EB");
                        PlaySound(SoundType.Message);
                        break;
                }

                await Task.WhenAll(CancellBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
                await Task.Delay(TimeSpan.FromSeconds(time));
                await Task.WhenAll(CancellBox.TranslateTo(0, 250, 400, easing: Easing.SinIn));

                StopCurrentSound();
                IsBusy(false);
            });
        }
    }
}
