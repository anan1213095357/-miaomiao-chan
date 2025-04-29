using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.Ollama
{
    public class Parameters
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, Property>? Properties { get; set; }

        [JsonProperty("required")]
        public List<string>? Required { get; set; }

        public Parameters()
        {
            Properties = new Dictionary<string, Property>();
            Required = new List<string>();
        }
    }
}
