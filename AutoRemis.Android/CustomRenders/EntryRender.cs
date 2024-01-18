using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using AutoRemis.CustomRenders;
using AutoRemis.Droid.CustomRenders;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(EntryRender))]
namespace AutoRemis.Droid.CustomRenders
{
    public class EntryRender : EntryRenderer
    {
        public EntryRender(Context context) : base(context) => AutoPackage = false;
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element is CustomEntry customEntry)
            {
                // Cambiar el color del borde y controlar la visibilidad del renglón aquí
                Control.BackgroundTintList = ColorStateList.ValueOf(customEntry.UnderlineColor.ToAndroid());

                // Controlar la visibilidad del renglón de abajo
                if (!customEntry.ShowUnderline)
                    Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
            }
        }
    }
}