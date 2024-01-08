using Android.Content;
using Android.Graphics.Drawables;
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
            if (Control != null)
                Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
        }
    }
}