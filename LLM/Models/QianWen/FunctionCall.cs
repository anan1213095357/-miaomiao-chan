using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.QianWen
{
    public class FunctionCall
    {
        [JsonProperty("name")]
        public string? Name { get; set; }


        [JsonProperty("arguments")]
        public string? Arguments { get; set; }
    }
}
