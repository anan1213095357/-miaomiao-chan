using System;
using System.Collections.Generic;
using System.Text;

namespace LLM.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ParameterDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public ParameterDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
