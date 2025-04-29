using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.QianWen
{
    public class Response: ChatResponseBase
    {
        [JsonProperty("choices", NullValueHandling = NullValueHandling.Ignore)]
        public Choice[]? Choices { get; set; }

        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public int Created { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string? ID { get; set; }

        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string? Model { get; set; }

        [JsonProperty("object", NullValueHandling = NullValueHandling.Ignore)]
        public string? Object { get; set; }

        [JsonProperty("usage", NullValueHandling = NullValueHandling.Ignore)]
        public Usage? Usage { get; set; }
    }
}
