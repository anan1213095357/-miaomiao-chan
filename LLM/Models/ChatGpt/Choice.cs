using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{
    public class Choice
    {
        public int index { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Message? message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Delta? delta { get; set; }
        public object? logprobs { get; set; }
        public string? finish_reason { get; set; }
    }
}
