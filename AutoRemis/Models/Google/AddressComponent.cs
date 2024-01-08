using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AutoRemis.Models.Google
{
    public class AddressComponent
    {
        public string LongName { get; }
        public string ShortName { get; }

        public IReadOnlyCollection<string> Types { get; }

        public AddressComponent(string longcomp, string shortcomp, IEnumerable<string> types)
        {
            LongName = longcomp;
            ShortName = shortcomp;
            Types = types.ToList().AsReadOnly();
        }

        public static AddressComponent FromJSON(JObject json) => new AddressComponent(json["long_name"].Value<string>(), json["short_name"].Value<string>(), json["types"].Value<JArray>().Select(p => p.Value<string>()));

        public override string ToString() => string.IsNullOrWhiteSpace(LongName) ? (ShortName ?? string.Empty) : LongName;
    }
}
