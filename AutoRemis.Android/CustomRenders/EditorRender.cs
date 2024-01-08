using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using AutoRemis.CustomRenders;
using AutoRemis.Droid.CustomRenders;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(EditorRender))]
namespace AutoRemis.Droid.CustomRenders
{
    class EditorRender : EditorRenderer
    {
        public EditorRender(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(Android.Graphics.Color.Transparent);
                Control.SetBackgroundDrawable(gd);
                Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
            }
        }
    }
}