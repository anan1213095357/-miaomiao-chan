using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.DouBao
{
    public class ChatRequest : ChatRequestBase
    {
        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("messages")]
        public List<Message>? Messages { get; set; }

        [JsonProperty("tools")]
        public List<Tool>? Tools { get; set; }

        [JsonProperty("stream")]
        public bool Stream { get; set; }

        [JsonProperty("format")]
        public string? Format { get; set; }

        [JsonProperty("options")]
        public object? Options { get; set; }

        [JsonProperty("keep_alive")]
        public object? KeepAlive { get; set; }


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
