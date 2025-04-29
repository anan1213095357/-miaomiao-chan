using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.Ollama
{
    public class ToolCall
    {
        [JsonProperty("function")]
        public FunctionCall? Function { get; set; }
    }


}
