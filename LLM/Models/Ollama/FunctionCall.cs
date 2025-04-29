using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.Ollama
{
    public class FunctionCall
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("arguments")]
        public Dictionary<string, string>? Arguments { get; set; }
    }
}
