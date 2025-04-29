using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.QianWen
{
    public class ChatRequest : ChatRequestBase
    {
        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("messages")]
        public List<Message>? Messages { get; set; }

        [JsonProperty("tools", NullValueHandling = NullValueHandling.Ignore)]
        public List<Tool>? Tools { get; set; }

        [JsonProperty("stream")]
        public bool Stream { get; set; }

        [JsonProperty("stream_options", NullValueHandling = NullValueHandling.Ignore)]
        public IncludeUsageClass? IncludeUsageClass { get; set; }



        public ChatRequest()
        {
            Messages = [];
            Tools = [];
        }
    }

    public class IncludeUsageClass
    {
        [JsonProperty("include_usage")]
        public bool IncludeUsage { get; set; } = true;

    }

}
