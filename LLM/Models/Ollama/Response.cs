using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.Ollama
{

    public class Response : ChatResponseBase
    {
        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("created_at")]
        public string? CreatedAt { get; set; }

        [JsonProperty("message")]
        public Message? Message { get; set; }
    }
}
