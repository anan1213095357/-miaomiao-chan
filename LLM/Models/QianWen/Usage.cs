using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.QianWen
{


    public class Usage
    {
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonProperty("total_tokens")]
        public int TotalTokens { get; set; }

        [JsonProperty("prompt_tokens_details")]
        public Prompt_Tokens_Details? PromptTokensDetails { get; set; }
    }

    public class Prompt_Tokens_Details
    {
        [JsonProperty("cached_tokens")]
        public int CachedTokens { get; set; }
    }


}
