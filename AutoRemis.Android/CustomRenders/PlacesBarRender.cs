using Android.Content;
using Android.Graphics.Drawables;
using AutoRemis.CustomRenders;
using AutoRemis.Droid.CustomRenders;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PlacesBar), typeof(PlacesBarRender))]
namespace AutoRemis.Droid.CustomRenders
{
    public class PlacesBarRender : EntryRenderer
    {
        public PlacesBarRender(Context context) : base(context) => AutoPackage = false;
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
                Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
        }
    }
}