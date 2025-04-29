using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.QianWen
{
    public class Message : MessageBase
    {
        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("role")]
        public string? Role { get; set; }

        [JsonProperty("tool_calls", NullValueHandling = NullValueHandling.Ignore)]
        public Tool_Calls[]? ToolCalls { get; set; }
    }
}
