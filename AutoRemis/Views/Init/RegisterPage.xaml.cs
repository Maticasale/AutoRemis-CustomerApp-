using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Models.Google;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AutoRemis.Views
{
    public partial class RegisterPage : ContentPage, INavigatedAware
    {
        private User user;

        private InitType init;
        private GoogleUser googleUser;


        private readonly INavigationService _navigationService;
        public RegisterPage(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //User Data
            user = AppStateManager.GetUser();
            init = parameters.GetValue<InitType>("LoginType");
            googleUser = parameters.GetValue<GoogleUser>("GoogleUser");

            //General UI Settings
            if (init == InitType.Google && !string.IsNullOrWhiteSpace(googleUser.Email)) 
            { 
                imgGooglePerson.IsVisible = true;
                imgGoogleMail.IsVisible = true;

                EntryUser.IsEnabled = false;
                EntryEmail.IsEnabled = false;

                EntryUser.Text = UIHelper.CapitalizeSentence(googleUser.FullName);
                EntryEmail.Text = googleUser.Email;
                user.GoogleUrlPic = googleUser.Picture;
            }   
        }

        private async void initTapped(object sender, EventArgs e)
        {
            IsBusy(true);

            if (!string.IsNullOrWhiteSpace(EntryUser.Text) && !string.IsNullOrWhiteSpace(EntryEmail.Text) && !string.IsNullOrWhiteSpace(EntryPhone.Text))
            {
                string[] name = EntryUser.Text.Split(' ');

                user.FirstName = UIHelper.CapitalizeSentence(name[0]);
                user.LastName = name.Length >= 2 ? UIHelper.CapitalizeSentence(name[1]) : "-";
                user.FullName = UIHelper.CapitalizeSentence(EntryUser.Text);
                user.Email = EntryEmail.Text; 
                user.Facebook = EntryFacebook.Text;
                user.PhoneNumber = EntryPhone.Text;
                AppStateManager.UpdateUser(user);

                await _navigationService.NavigateAsync("ConfirmPhonePage", new NavigationParameters { { "ConfirmationToken", "1234" }, { "LoginType", init }});
            }
            else
                RiseErrorMsg("¡Atencion!", init == InitType.Google ? "Necesitamos tu numero de Celular para poder continuar" : "Necesitamos que completes los campos obligatorios para poder continuar", 3, SoundHelper.SoundType.Alert);

            IsBusy(false);
        }

        private new void IsBusy(bool value)
        {
            LoadingIndicator.IsRunning = value;
            btnInit.IsEnabled = !value;
            lblInit.Text = value ? "" : "INICIAR";
        }

        private void RiseErrorMsg(string title, string msg, int time, SoundHelper.SoundType type)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Title.Text = title;
                Msg.Text = msg;

                switch (type)
                {
                    case SoundHelper.SoundType.Error:
                        imgItem.Source = "iconError.png";
                        CancellBox.BorderColor = Color.FromHex("#ff355b");
                        CancellBox.BackgroundColor = Color.FromHex("#fffbfc");
                        Title.TextColor = Color.FromHex("#ff355b");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Alert);
                        break;
                    case SoundHelper.SoundType.Alert:
                        imgItem.Source = "iconWarning.png";
                        CancellBox.BorderColor = Color.FromHex("#FFC021");
                        CancellBox.BackgroundColor = Color.FromHex("#fffefb");
                        Title.TextColor = Color.FromHex("#FFC021");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Alert);
                        break;
                    case SoundHelper.SoundType.Success:
                        imgItem.Source = "iconSuccess.png";
                        CancellBox.BorderColor = Color.FromHex("#47D764");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfefc");
                        Title.TextColor = Color.FromHex("#47D764");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Success);
                        break;
                    case SoundHelper.SoundType.Message:
                        imgItem.Source = "iconInfo.png";
                        CancellBox.BorderColor = Color.FromHex("#2F86EB");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfdff");
                        Title.TextColor = Color.FromHex("#2F86EB");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Message);
                        break;
                }

                await Task.WhenAll(CancellBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
                await Task.Delay(TimeSpan.FromSeconds(time));
                await Task.WhenAll(CancellBox.TranslateTo(0, 250, 400, easing: Easing.SinIn));

                SoundHelper.StopCurrentSound();
            });
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}
