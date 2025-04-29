using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.Ollama
{
    public class EmbeddingsResponse
    {
        public string? model { get; set; }
        public float[][]? embeddings { get; set; }
        public long total_duration { get; set; }
        public long load_duration { get; set; }
        public long prompt_eval_count { get; set; }
    }
}
