using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{
    public class Parameters
    {
        public string? type { get; set; }
        public object? properties { get; set; }
        public List<string>? required { get; set; }
    }
}
