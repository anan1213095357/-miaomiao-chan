using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Models.ChatGpt
{
    public class Tool : ToolBase
    {
        public string? type { get; set; }
        public Function? function { get; set; }
    }

}
