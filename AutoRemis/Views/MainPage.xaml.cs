using System;
using System.ComponentModel;
using Xamarin.Forms;
using FFImageLoading.Svg.Forms;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AutoRemis.Views
{
    [DesignTimeVisible(false)]
    public partial class MainPage
    {
        public bool IsPlaying { get; set; }

        public MainPage()
        {
            InitializeComponent();
            CargarGIF();
        }

        private void CargarGIF()
        {
            //Task.Delay(2000);
            //Debug.WriteLine("Cargano Gif");
            Device.BeginInvokeOnMainThread(() =>
            {
                // Ruta del archivo GIF en la carpeta Resources/drawable (asegúrate de que la ruta sea correcta)
                var gifPath = "gifRadar.gif";

                // Crea una instancia de SvgCachedImage (puedes ajustar las propiedades según tus necesidades)
                var svgCachedImage = new SvgCachedImage
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Aspect = Aspect.AspectFit,
                    Source = gifPath,
                };

                // Agrega la instancia al StackLayout (o cualquier otro contenedor que estés utilizando)
                stk.Children.Add(svgCachedImage);
            });
        }
    }
}
