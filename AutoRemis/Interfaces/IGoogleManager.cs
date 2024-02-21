using AutoRemis.Models.Google;
using System;

namespace AutoRemis.Interfaces
{
    public interface IGoogleManager
    {
        public void Login(Action<GoogleUser, string> OnLoginComplete);

        public void Logout();
    }
}
