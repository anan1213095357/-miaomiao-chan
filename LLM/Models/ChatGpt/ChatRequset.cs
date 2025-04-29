using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{

    public class ChatRequset : ChatRequestBase
    {
        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("messages")]
        public List<Message> Messages { get; set; } = [];

        [JsonProperty("stream")]
        public bool Stream { get; set; } = false;

        [JsonProperty("stream_options", NullValueHandling = NullValueHandling.Ignore)]
        public StreamOptions? StreamOptions { get; set; }

        [JsonProperty("tools", NullValueHandling = NullValueHandling.Ignore)]
        public List<Tool>? Tools { get; set; }

    }



}
