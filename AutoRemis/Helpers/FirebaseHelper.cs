using Plugin.FirebasePushNotification;
using System.Diagnostics;

namespace AutoRemis.Helpers
{
    public static class FirebaseHelper
    {
        public static void Initialize()
        {
            CrossFirebasePushNotification.Current.OnTokenRefresh += RefreshedToken;
            CrossFirebasePushNotification.Current.OnNotificationReceived += NotificationReceived;
        }

        private static void NotificationReceived(object source, FirebasePushNotificationDataEventArgs e)
        {
            Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------");
            foreach (var data in e.Data)
                Debug.WriteLine($"{data.Key} : {data.Value}", "FCM");
            Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        private static void RefreshedToken(object source, FirebasePushNotificationTokenEventArgs e)
        {
            Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Debug.WriteLine(e.Token);
            Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------");
        }
    }
}