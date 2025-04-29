using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{


    public class Response: ChatResponseBase
    {
        public string? id { get; set; }
        public string? _object { get; set; }
        public int created { get; set; }
        public string? model { get; set; }
        public Choice[]? choices { get; set; }
        public Usage? usage { get; set; }
        public string? system_fingerprint { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
        public Prompt_Tokens_Details? prompt_tokens_details { get; set; }
        public Completion_Tokens_Details? completion_tokens_details { get; set; }
    }

    public class Prompt_Tokens_Details
    {
        public int cached_tokens { get; set; }
        public int audio_tokens { get; set; }
    }

    public class Completion_Tokens_Details
    {
        public int reasoning_tokens { get; set; }
        public int audio_tokens { get; set; }
        public int accepted_prediction_tokens { get; set; }
        public int rejected_prediction_tokens { get; set; }
    }







}
