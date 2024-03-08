using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AutoRemis.Models.Google
{
    public class Place
    {
        public string ID { get; set; }
        public string Place_ID { get; set; }
        public string Reference { get; set; }
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<AddressComponent> AddressComponents { get; set; }
        public string FormattedAddress { get; set; }
        public string PhoneNumberFormatted { get; set; }
        public string PhoneNumberInternational { get; set; }
        public List<string> Types { get; set; }
        public string Website { get; set; }
        public string Vicinity { get; set; }
        public int? UTCOffset { get; set; }
        public string Raw { get; set; }
        public Place(JObject jsonObject)
        {
            ID = jsonObject["result"]["id"]?.Value<string>();
            Place_ID = jsonObject["result"]["place_id"]?.Value<string>() ?? string.Empty;
            Reference = jsonObject["result"]["reference"]?.Value<string>() ?? string.Empty;
            Name = jsonObject["result"]["name"]?.Value<string>() ?? string.Empty;
            Latitude = jsonObject["result"]?["geometry"]?["location"]?["lat"]?.Value<double>();
            Longitude = jsonObject["result"]?["geometry"]?["location"]?["lng"]?.Value<double>();
            AddressComponents = jsonObject["result"]["address_components"]?.Value<JArray>()?.Select(p => AddressComponent.FromJSON(p.Value<JObject>()))?.ToList() ?? new List<AddressComponent>();
            FormattedAddress = jsonObject["result"]["formatted_address"]?.Value<string>() ?? string.Empty;
            PhoneNumberFormatted = jsonObject["result"]["formatted_phone_number"]?.Value<string>() ?? string.Empty;
            PhoneNumberInternational = jsonObject["result"]["international_phone_number"]?.Value<string>() ?? string.Empty;
            Types = jsonObject["result"]["types"]?.Value<JArray>()?.Select(p => p.Value<string>())?.ToList() ?? new List<string>();
            Website = jsonObject["result"]["website"]?.Value<string>() ?? string.Empty;
            Vicinity = jsonObject["result"]["vicinity"]?.Value<string>() ?? string.Empty;
            UTCOffset = jsonObject["result"]["utc_offset"]?.Value<int>();

            Raw = jsonObject.ToString();
        }

        public AddressComponent GetAddressComponentOrNull(string type)
        {
            foreach (var component in AddressComponents)
                if (component.Types.Contains(type)) return component;
            return null;
        }

        public AddressComponent AdminArea => GetAddressComponentOrNull("administrative_area_level_1");
        public AddressComponent SubAdminArea => GetAddressComponentOrNull("administrative_area_level_2");
        public AddressComponent SubSubAdminArea => GetAddressComponentOrNull("administrative_area_level_3");
        public AddressComponent Locality => GetAddressComponentOrNull("locality");
        public AddressComponent SubLocality => GetAddressComponentOrNull("sublocality_level_1") ?? GetAddressComponentOrNull("sublocality");
        public AddressComponent Thoroughfare => GetAddressComponentOrNull("route");
        public AddressComponent SubThoroughfare => GetAddressComponentOrNull("street_number");
        public AddressComponent PostalCode => GetAddressComponentOrNull("postal_code");
        public AddressComponent Country => GetAddressComponentOrNull("country");
        public AddressComponent StreetName => GetAddressComponentOrNull("route");
        public AddressComponent StreetNumber => GetAddressComponentOrNull("street_number");
    }
}