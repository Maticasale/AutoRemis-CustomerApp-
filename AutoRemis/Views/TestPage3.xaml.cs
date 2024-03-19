using System.Threading.Tasks;
using System.Threading;
using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace AutoRemis.Views
{
    public partial class TestPage3 : ContentPage
    {
        private Timer timer;
        private readonly object lockObject = new object();
        private bool isRunning = false;

        public TestPage3()
        {
            // Inicia el timer con un intervalo de 10 segundos
            timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private void TimerCallback(object state)
        {
            
        }
        public void StopTimer()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite); // Detiene el timer
        }

        public void RestartTimer()
        {
            // Reinicia el timer con el intervalo de 10 segundos
            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }
    }
}