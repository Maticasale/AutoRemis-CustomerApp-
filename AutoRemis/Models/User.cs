using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace AutoRemis.Models
{
    //Local Storage 
    public class User
    {
        public UserStatus Status { get; set; }
        public InitType Init { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public Uri GoogleUrlPic { get; set; } 
        public string Email { get; set; }
        public string Facebook { get; set; }
        public Position lastKnownPosition { get; set; }
        public Trip TripInfo { get; set; }
    }

    public class AppSettings
    {
        public string HelpCenterPhone { get; set; }
        public string GlobalApiKey { get; set; }
        public bool NotificationsRegistered { get; set; }
    }

    /// <summary>
    /// Unknown: No se sabe si es un usuario nuevo o corriente.
    /// PENDIENTE: PendingRegistration: Usuario con registro iniciado pero no terminado, ya sea porque no confirmo el numero de telefono
    /// Idle: Usuario inactivo, sera redirigido a la pagina de Mapa
    /// OnTrip: Usuario en Viaje, para vere mas info ver el Enum dentro de Trip
    /// budgeting: Estaba creando un presupuesto, sera redirigido a la pagina de presupuesto
    /// </summary>
    public enum UserStatus { Disconnected, Idle, budgeting, OnTrip }

    public enum InitType { Google, PhoneNumber, Normal }
}
