using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TheAppsPajamas.Models
{
    public class MediaAssetCatalogue
    {
        public List<Image> images { get; set; } = new List<Image>();
        public Info info { get; set; } = new Info();
    }
    public class Image
    {
        public string size { get; set; }
        public string idiom { get; set; }
        public string filename { get; set; }
        public string scale { get; set; }
        public string role { get; set; }
        public string subtype { get; set; }

        [JsonProperty(PropertyName = "minimum-system-version")]
        public string minimumsystemversion { get; set;}

        public string orientation { get; set; }

        public string extent { get; set; }
    }

    public class Info
    {
        public int version { get; set; }
        public string author { get; set; }

        public Info(){
            version = 1;
            author = "xcode";
        }
    }

}
