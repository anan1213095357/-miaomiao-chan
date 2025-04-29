using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.DouBao
{

    public class StreamResponse
    {
        [JsonProperty("choices")]
        public Choice[] Choices { get; set; } = [];

        public int Created { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; } = "";


        [JsonProperty("model")]
        public string Model { get; set; } = "";

        [JsonProperty("object")]
        public string Object { get; set; } = "";

        [JsonProperty("usage",NullValueHandling = NullValueHandling.Ignore)]
        public Usage? Usage { get; set; }
    }


    public class Delta
    {
        [JsonProperty("content")]
        public string Content { get; set; } = "";

        [JsonProperty("reasoning_content")]
        public string ReasoningContent { get; set; } = "";

        [JsonProperty("role")]
        public string Role { get; set; } = "";
    }



}
