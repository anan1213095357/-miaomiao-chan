using System;
using System.Collections.Generic;
using System.Text;

namespace LLM.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FunctionDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public FunctionDescriptionAttribute(string name, string description)
        {
            Description = description;
            Name = name;
        }
    }
}
