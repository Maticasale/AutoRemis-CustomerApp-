using AutoRemis.Models.Google;
using AutoRemis.Services;
using Xamarin.Forms;

namespace AutoRemis.CustomRenders
{
    public delegate void PlacesRetrievedEventHandler(object sender, AutoCompleteResult result);

    public class PlacesBar : Entry
    {
        public static readonly BindableProperty PlaceTypeProperty = BindableProperty.Create(nameof(Type), typeof(PlaceType), typeof(PlacesBar), PlaceType.All, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty LocationBiasProperty = BindableProperty.Create(nameof(Bias), typeof(LocationBias), typeof(PlacesBar), (object)null, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty ComponentsProperty = BindableProperty.Create(nameof(Components), typeof(Components), typeof(PlacesBar), (object)null, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty ApiKeyProperty = BindableProperty.Create(nameof(ApiKey), typeof(string), typeof(PlacesBar), string.Empty, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty MinimumSearchTextProperty = BindableProperty.Create(nameof(MinimumSearchText), typeof(int), typeof(PlacesBar), 2, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty LanguageProperty = BindableProperty.Create(nameof(Language), typeof(GoogleAPILanguage), typeof(PlacesBar), GoogleAPILanguage.Unset, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);

        public PlaceType Type
        {
            get
            {
                return (PlaceType)this.GetValue(PlacesBar.PlaceTypeProperty);
            }
            set
            {
                this.SetValue(PlacesBar.PlaceTypeProperty, (object)value);
            }
        }

        public LocationBias Bias
        {
            get
            {
                return (LocationBias)this.GetValue(PlacesBar.LocationBiasProperty);
            }
            set
            {
                this.SetValue(PlacesBar.LocationBiasProperty, (object)value);
            }
        }

        public Components Components
        {
            get
            {
                return (Components)this.GetValue(PlacesBar.ComponentsProperty);
            }
            set
            {
                this.SetValue(PlacesBar.ComponentsProperty, (object)value);
            }
        }

        public string ApiKey
        {
            get
            {
                return (string)this.GetValue(PlacesBar.ApiKeyProperty);

            }
            set
            {
                this.SetValue(PlacesBar.ApiKeyProperty, (object)value);
            }
        }

        public int MinimumSearchText
        {
            get
            {
                return (int)this.GetValue(PlacesBar.MinimumSearchTextProperty);

            }
            set
            {
                this.SetValue(PlacesBar.MinimumSearchTextProperty, (object)value);
            }
        }

        public GoogleAPILanguage Language
        {
            get
            {
                return (GoogleAPILanguage)this.GetValue(PlacesBar.LanguageProperty);

            }
            set
            {
                this.SetValue(PlacesBar.LanguageProperty, (object)value);
            }
        }

        public event PlacesRetrievedEventHandler PlacesRetrieved;

        protected virtual void OnPlacesRetrieved(AutoCompleteResult e)
        {
            PlacesRetrievedEventHandler handler = PlacesRetrieved;
            handler?.Invoke(this, e);
        }

        public PlacesBar()
        {
            TextChanged += OnTextChanged;
        }

        async void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length >= MinimumSearchText)
            {
                var predictions = await Places.GetPlaces(e.NewTextValue, ApiKey, Bias, Components, Type, Language);
                if (PlacesRetrieved != null && predictions != null)
                    OnPlacesRetrieved(predictions);
                else
                    OnPlacesRetrieved(new AutoCompleteResult());
            }
            else
            {
                OnPlacesRetrieved(new AutoCompleteResult());
            }
        }
    }
}
