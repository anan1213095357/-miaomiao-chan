using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LLM.Utilities.Ollama
{
    internal static class TypeHelper
    {
        // 辅助方法：获取 JSON Schema 类型
        public static string GetJsonSchemaType(Type type)
        {
            if (type == typeof(int) || type == typeof(long))
                return "integer";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                return "number";
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(string))
                return "string";
            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
                return "array";
            return "object";
        }
    }
}
