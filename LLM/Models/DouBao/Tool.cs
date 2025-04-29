using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.DouBao
{
    public class Tool : ToolBase
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("function")]
        public Function? Function { get; set; }
    }
}
