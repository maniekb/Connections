using Newtonsoft.Json;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Line
    {
        [JsonProperty("lineNumber")]
        public int lineNumber { get; set; }
        [JsonProperty("stops")]
        public string[] stops { get; set; }
    }
}
