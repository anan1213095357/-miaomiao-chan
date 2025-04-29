using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{

    public class StreamOptions
    {
        [JsonProperty("include_usage")]
        public bool IncludeUsage { get; set; } = true;
    }
}
