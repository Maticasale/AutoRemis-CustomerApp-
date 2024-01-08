using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AutoRemis.Models.Google
{
    public class AutoCompletePrediction
    {
        public string Description { get; set; }
        public string ID { get; set; }
        public string Place_ID { get; set; }
        public string Reference { get; set; }
        public string MainText { get; set; }
        public string SecondaryText { get; set; }
        public List<string> Terms { get; set; }
        public List<string> Types { get; set; }

        public static AutoCompletePrediction FromJson(JObject json)
        {
            var r = new AutoCompletePrediction
            {
                Description = json["description"].Value<string>(),
                //ID            = json["id"].Value<string>(),
                Place_ID = json["place_id"].Value<string>(),
                Reference = json["reference"].Value<string>(),
                MainText = json["structured_formatting"]["main_text"].Value<string>(),
                SecondaryText = json["structured_formatting"]["secondary_text"].Value<string>(),
                Terms = json["terms"].Value<JArray>().Select(p => p["value"].Value<string>()).ToList(),
                Types = json["types"].Value<JArray>().Select(p => p.Value<string>()).ToList()
            };

            return r;
        }
    }
}
