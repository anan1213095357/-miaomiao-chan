using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.DouBao
{
    public class Property
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}
