using Android.App;
using Android.Util;
using Android.Content;
using Firebase.Messaging;
using System.Diagnostics;
using Xamarin.Essentials;
using System.Collections.Generic;
using AutoRemis.Models;
using Xamarin.Forms;
using Android.Gms.Extensions;
using System.Threading.Tasks;

using AutoRemis.Interfaces;
using System;
using System.Globalization;

namespace AutoRemis.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseManager : FirebaseMessagingService, IFirebaseManager
    {
        const string TAG = "FCM";
        AndroidNotificationManager androidNotification = new AndroidNotificationManager();
        public override void OnMessageReceived(RemoteMessage message)
        {
            var Msg = new FirebaseMessage();
            var parameters = new Dictionary<string, object>();

            Log.Debug(TAG, "------------------------------------------------------------------------------------------------------------------------------------------------------------");
            //Log.Debug(TAG, "Title: " + message.GetNotification().Title);
            //Log.Debug(TAG, "Body: " + message.GetNotification().Body);
            //Log.Debug(TAG, "Chanel ID: " + message.GetNotification().ChannelId);


            //androidNotification.CrearNotificacionLocal(message.GetNotification().Title, message.GetNotification().Body);
            var notification = message.GetNotification();
            foreach (var d in message.Data)
            {
                if (!parameters.ContainsKey(d.Key))
                {
                    Log.Debug(TAG, $"{d.Key}: {d.Value}");
                    parameters.Add(d.Key, d.Value);

                    Msg.cuerpo = (d.Key.ToString() == "cuerpo") ? d.Value.ToString() : Msg.cuerpo;
                    Msg.de = (d.Key.ToString() == "de") ? d.Value.ToString() : Msg.de;
                    Msg.tipo = (d.Key.ToString() == "tipo") ? d.Value.ToString() : Msg.tipo;
                    Msg.fecha = (d.Key.ToString() == "fecha") ? DateTime.ParseExact(d.Value.ToString(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm") : Msg.fecha;
                    Msg.idMsg = (d.Key.ToString() == "idMsg") ? d.Value.ToString() : Msg.idMsg;
                    Msg.idFCM = (d.Key.ToString() == "idFCM") ? d.Value.ToString() : Msg.idFCM;
                }
            }
            Log.Debug(TAG, "------------------------------------------------------------------------------------------------------------------------------------------------------------");

            MessagingCenter.Send<object, FirebaseMessage>(this, "FCM", Msg);

        }
        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            //var user = GetUser();
            //user.TokenFCM = token;
            //UpdateUser(user);
        }

        public async Task<string> GetFirebaseToken() => (string)await FirebaseMessaging.Instance.GetToken();

    }
}