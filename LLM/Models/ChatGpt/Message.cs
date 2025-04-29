using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{
    public class Message : MessageBase
    {
        public string? role { get; set; }
        public string? content { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object? refusal { get; set; }



        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Tool_Call>? tool_calls { get; set; }
    }
    
}
