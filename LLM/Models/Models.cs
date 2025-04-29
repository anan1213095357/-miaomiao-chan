using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLM.Models
{

    public class TongYiQianWenChatResponse : ChatResponseBase
    {
        public int status_code { get; set; }
        public string? request_id { get; set; }
        public string? code { get; set; }
        public string? message { get; set; }
        public TongYiQianWenOutput? output { get; set; }
        public TongYiQianWenUsage? usage { get; set; }
    }

    public class TongYiQianWenOutput
    {
        public object? text { get; set; }
        public object? finish_reason { get; set; }
        public TongYiQianWenChoice[]? choices { get; set; }
    }

    public class TongYiQianWenChoice
    {
        public string? finish_reason { get; set; }
        public TongYiQianWenMessage? message { get; set; }
    }

    public class TongYiQianWenMessage
    {
        public string? role { get; set; }
        public string? content { get; set; }
    }

    public class TongYiQianWenUsage
    {
        public int input_tokens { get; set; }
        public int output_tokens { get; set; }
        public int total_tokens { get; set; }
    }








}
