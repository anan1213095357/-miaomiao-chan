using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{




    public class Tool_Call
    {
        public string? id { get; set; }
        public string? type { get; set; }
        public FunctionCall? function { get; set; }
    }

    public class FunctionCall
    {
        public string? name { get; set; }
        public string? arguments { get; set; }
    }

}
