using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.QianWen
{
    public class Tool_Calls
    {
        [JsonProperty("function")]
        public FunctionCall? Function { get; set; }

        [JsonProperty("id")]
        public string? ID { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }
    }
}
