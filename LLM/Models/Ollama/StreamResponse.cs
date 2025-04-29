using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.Ollama
{
    public class StreamResponse
    {
        public string? model { get; set; }
        public DateTime created_at { get; set; }
        public Message? message { get; set; }
        public bool done { get; set; }
    }
}
