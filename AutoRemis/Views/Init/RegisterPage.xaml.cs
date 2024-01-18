﻿using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Models.Google;
using Prism.Navigation;
using System;
using System.Collections.Generic;
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
            imgGooglePerson.IsVisible = init == InitType.Google;
            imgGoogleMail.IsVisible = init == InitType.Google;
            EntryUser.IsEnabled = init != InitType.Google;
            EntryEmail.IsEnabled = init != InitType.Google;
            frmPassword.IsVisible = init != InitType.Google;
            frmConfirmPassword.IsVisible = init != InitType.Google;

            switch (init)
            {
                case InitType.Google:
                    user.FirstName = googleUser.FirstName;
                    user.LastName = googleUser.LastName;
                    user.FullName = googleUser.FullName;
                    user.Email = googleUser.Email;

                    EntryUser.Text = UI.CapitalizeSentence(user.FullName);
                    EntryEmail.Text = user.Email;

                    break;

                case InitType.Normal:

                    return;

                case InitType.PhoneNumber:
                    return;
            }
        }

        private async void initTapped(object sender, EventArgs e)
        {
            IsBusy(true);

            if (!string.IsNullOrWhiteSpace(EntryUser.Text) && !string.IsNullOrWhiteSpace(EntryEmail.Text) && !string.IsNullOrWhiteSpace(EntryPhone.Text))
            {
                user.FullName = EntryUser.Text;
                user.Email = EntryEmail.Text; 
                user.Facebook = EntryFacebook.Text;
                user.PhoneNumber = EntryPhone.Text;
                AppStateManager.UpdateUser(user);

                await _navigationService.NavigateAsync("ConfirmPhonePage", new NavigationParameters {{ "ConfirmationToken", "1234" }});
            }
            else
                RiseErrorMsg("¡Atencion!", init == InitType.Google ? "Necesitamos tu numero de Celular para poder continuar" : "Necesitamos que completes los campos obligatorios para poder continuar", 3, SoundTools.SoundType.Alert);

            IsBusy(false);
        }

        private new void IsBusy(bool value)
        {
            LoadingIndicator.IsRunning = value;
            btnInit.IsEnabled = !value;
            lblInit.Text = value ? "" : "INICIAR";
        }

        private void RiseErrorMsg(string title, string msg, int time, SoundTools.SoundType type)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

                Title.Text = title;
                Msg.Text = msg;

                switch (type)
                {
                    case SoundTools.SoundType.Error:
                        imgItem.Source = "iconError.png";
                        CancellBox.BorderColor = Color.FromHex("#ff355b");
                        CancellBox.BackgroundColor = Color.FromHex("#fffbfc");
                        Title.TextColor = Color.FromHex("#ff355b");
                        SoundTools.PlaySound(SoundTools.SoundType.Alert);
                        break;
                    case SoundTools.SoundType.Alert:
                        imgItem.Source = "iconWarning.png";
                        CancellBox.BorderColor = Color.FromHex("#FFC021");
                        CancellBox.BackgroundColor = Color.FromHex("#fffefb");
                        Title.TextColor = Color.FromHex("#FFC021");
                        SoundTools.PlaySound(SoundTools.SoundType.Alert);
                        break;
                    case SoundTools.SoundType.Success:
                        imgItem.Source = "iconSuccess.png";
                        CancellBox.BorderColor = Color.FromHex("#47D764");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfefc");
                        Title.TextColor = Color.FromHex("#47D764");
                        SoundTools.PlaySound(SoundTools.SoundType.Success);
                        break;
                    case SoundTools.SoundType.Message:
                        imgItem.Source = "iconInfo.png";
                        CancellBox.BorderColor = Color.FromHex("#2F86EB");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfdff");
                        Title.TextColor = Color.FromHex("#2F86EB");
                        SoundTools.PlaySound(SoundTools.SoundType.Message);
                        break;
                }

                await Task.WhenAll(CancellBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
                await Task.Delay(TimeSpan.FromSeconds(time));
                await Task.WhenAll(CancellBox.TranslateTo(0, 250, 400, easing: Easing.SinIn));

                SoundTools.StopCurrentSound();
            });
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        private void GoBack(object sender, EventArgs e)
        {

        }
    }
}
