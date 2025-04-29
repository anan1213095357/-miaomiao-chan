using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{
    public class Delta
    {
        public string? role { get; set; }
        public string? content { get; set; }
        public object? refusal { get; set; }
    }
}
