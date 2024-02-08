using AutoRemis.Models.Google;
using System;

namespace AutoRemis.Interfaces
{
    public interface IGoogleManager
    {
        void Login(Action<GoogleUser, string> OnLoginComplete);

        void Logout();
    }
}
