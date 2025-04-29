using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{
    public class Function
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public Parameters? parameters { get; set; }
    }

}
