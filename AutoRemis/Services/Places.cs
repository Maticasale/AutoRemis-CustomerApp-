using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Models.Google;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AutoRemis.Services
{
    public static class Places
    {
        private static AppSettings app;
        private static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
        private static HttpClient cliente;
        static Places() 
        { 
            cliente = new HttpClient(); 
            cliente.BaseAddress = new Uri("https://maps.googleapis.com/maps/");
            app = AppStateManager.GetAppInfo();
        }
        public static async Task<Place> GetPlace(string placeID, string apiKey, PlacesFieldList fields = null)
        {
            fields = fields ?? PlacesFieldList.ALL;

            try
            {
                var requestURI = CreateDetailsRequestUri(placeID, apiKey, fields);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, requestURI);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("PlacesBar HTTP request denied.");
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();

                if (result == "ERROR")
                {
                    Debug.WriteLine("PlacesSearchBar Google Places API returned ERROR");
                    return null;
                }

                return new Place(JObject.Parse(result));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlacesBar HTTP issue: {0} {1}", ex.Message, ex);
                return null;
            }
        }

        private static string CreateDetailsRequestUri(string place_id, string apiKey, PlacesFieldList fields)
        {
            var url = "https://maps.googleapis.com/maps/api/place/details/json";
            url += $"?placeid={Uri.EscapeUriString(place_id)}";
            url += $"&key={apiKey}";
            if (!fields.IsEmpty()) url += $"&fields={fields}";
            return url;
        }

        public static async Task<AutoCompleteResult> GetPlaces(string newTextValue, string apiKey, LocationBias bias, Components components, PlaceType type, GoogleAPILanguage language)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("You have not assigned a Google API key to PlacesBar");

            try
            {
                var requestURI = CreatePredictionsUri(newTextValue, apiKey, bias, components, type, language);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, requestURI);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("PlacesBar HTTP request denied.");
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();

                if (result == "ERROR")
                {
                    Debug.WriteLine("PlacesSearchBar Google Places API returned ERROR");
                    return null;
                }

                return AutoCompleteResult.FromJson(JObject.Parse(result));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlacesBar HTTP issue: {0} {1}", ex.Message, ex);
                return null;
            }
        }

        private static string CreatePredictionsUri(string newTextValue, string apiKey, LocationBias bias, Components components, PlaceType type, GoogleAPILanguage language)
        {
            var url = "https://maps.googleapis.com/maps/api/place/autocomplete/json";
            var input = Uri.EscapeUriString(newTextValue);
            var pType = PlaceTypeValue(type);

            var constructedUrl = $"{url}?input={input}&types={pType}&key={apiKey}";

            if (bias != null)
                constructedUrl = constructedUrl + bias;

            if (components != null)
                constructedUrl += components;

            if (language != GoogleAPILanguage.Unset)
                constructedUrl += "&Language=" + GoogleAPILanguageHelper.ToAPIString(language);

            return constructedUrl;
        }

        public static async Task<GoogleDirection> GetDirections(string originLatitude, string originLongitude, string destinationLatitude, string destinationLongitude)
        {
            if (!IsConnected)
                throw new Exception("No hay conexion.");
            else
            {
                GoogleDirection googleDirection = new GoogleDirection();

                var response = await cliente.GetAsync($"api/directions/json?mode=driving&transit_routing_preference=less_driving&origin={originLatitude},{originLongitude}&destination={destinationLatitude},{destinationLongitude}&key={app.GlobalApiKey}").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(json))
                        googleDirection = await Task.Run(() => JsonConvert.DeserializeObject<GoogleDirection>(json)).ConfigureAwait(false);
                }
                return googleDirection;
            }
        }

        public static async Task<string> GetPlaceID(string lat, string lng)
        {
            if (!IsConnected)
                throw new Exception("No hay conexion.");
            else
            {
                GooglePlaceID place = new GooglePlaceID();
                var response = await cliente.GetAsync($"api/geocode/json?latlng={lat},{lng}&key={app.GlobalApiKey}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(json))
                        place = await Task.Run(() => JsonConvert.DeserializeObject<GooglePlaceID>(json)).ConfigureAwait(false);
                }
                return place.results[0].place_id;
            }
        }

        private static string PlaceTypeValue(PlaceType type)
        {
            switch (type)
            {
                case PlaceType.All:
                    return "";
                case PlaceType.Geocode:
                    return "geocode";
                case PlaceType.Address:
                    return "address";
                case PlaceType.Establishment:
                    return "establishment";
                case PlaceType.Regions:
                    return "(regions)";
                case PlaceType.Cities:
                    return "(cities)";
                default:
                    return "";
            }
        }
    }
}
