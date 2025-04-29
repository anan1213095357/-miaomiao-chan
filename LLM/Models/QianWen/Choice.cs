using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.QianWen
{
    public class Choice
    {
        [JsonProperty("finish_reason",NullValueHandling = NullValueHandling.Ignore)]
        public string? FinishReason { get; set; }
        public int Index { get; set; }

        [JsonProperty("logprobs", NullValueHandling = NullValueHandling.Ignore)]
        public object? Logprobs { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public Message? Message { get; set; }

        [JsonProperty("delta", NullValueHandling = NullValueHandling.Ignore)]
        public Delta? Delta { get; set; }
    }
}
