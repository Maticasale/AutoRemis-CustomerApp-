using Xamarin.Forms;

namespace AutoRemis.CustomRenders
{
    public class CustomEntry : Entry
    {
        public static readonly BindableProperty ShowUnderlineProperty = BindableProperty.Create(nameof(ShowUnderline), typeof(bool), typeof(CustomEntry), true);

        public bool ShowUnderline
        {
            get { return (bool)GetValue(ShowUnderlineProperty); }
            set { SetValue(ShowUnderlineProperty, value); }
        }


        public static readonly BindableProperty UnderlineColorProperty = BindableProperty.Create(nameof(UnderlineColor), typeof(Color), typeof(CustomEntry), Color.Default);

        public Color UnderlineColor
        {
            get { return (Color)GetValue(UnderlineColorProperty); }
            set { SetValue(UnderlineColorProperty, value); }
        }
    }
}
