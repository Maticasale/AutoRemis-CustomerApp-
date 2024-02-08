using AutoRemis.Models;
using Xamarin.Forms;
using Newtonsoft.Json;
using Prism.Common;
using Prism.Navigation;
using Xamarin.Essentials;

namespace AutoRemis.Helpers
{
    public class AppStateManager
    {
        public static void UpdateUser(User user) => Preferences.Set("UserInfo", JsonConvert.SerializeObject(user));
        public static User GetUser()
        {
            User user;

            var jsonUser = Preferences.Get("UserInfo", "");

            if (!string.IsNullOrEmpty(jsonUser))
                user = JsonConvert.DeserializeObject<User>(jsonUser);
            else
            {
                user = new User() { Status = UserStatus.Disconnected , TripInfo = new Trip() };
                UpdateUser(user);
            }

            return user;
        }
        public static Page GetCurrentPage() => PageUtilities.GetCurrentPage(Application.Current.MainPage);
    }
}
